using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using FoglioUtils;
using FoglioUtils.Extensions;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Parsing;
using TCC.Utilities;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC
{
    public static class GlobalExceptionHandler
    {
        public static void HandleGlobalException(object sender, UnhandledExceptionEventArgs e)
        {
            FocusManager.Dispose();
            HandleGlobalExceptionImpl(e);
        }

        private static void HandleGlobalExceptionImpl(UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var js = BuildJsonDump(ex);//await App.BaseDispatcher.InvokeAsync(() => BuildJsonDump(ex));
            DumpCrashToFile(js, ex);
            UploadCrashDump(js);


            if (ex is COMException com && (com.HResult == 88980406 /*not sure if getting this value like this is correct*/
                                        || com.Message.Contains("UCEERR_RENDERTHREADFAILURE")))
            {
                TccMessageBox.Show("TCC",
                    "An error in render thread occured. This is usually caused by outdated video card drivers. TCC will now close.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                TccMessageBox.Show("TCC",
                    "An error occured and TCC will now close. Report this issue to the developer attaching crash.log from TCC folder.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            App.ReleaseMutex();
            ProxyInterface.Instance.Disconnect();
            if (WindowManager.TrayIcon != null) WindowManager.TrayIcon.Dispose();

            try { WindowManager.Dispose(); } catch {/* ignored*/}

            try
            {
                Firebase.RegisterWebhook(App.Settings.WebhookUrlGuildBam, false);
                Firebase.RegisterWebhook(App.Settings.WebhookUrlFieldBoss, false);
            }
            catch { }
            Environment.Exit(-1);

        }
        private static string FormatFullException(Exception ex)
        {
            var fullSb = new StringBuilder();
            fullSb.AppendLine("---- Exception ----");
            fullSb.AppendLine(ex.GetType().FullName);
            fullSb.AppendLine("---- Message ----");
            fullSb.AppendLine(ex.Message);
            fullSb.AppendLine("---- Source ----");
            fullSb.AppendLine(ex.Source);
            fullSb.AppendLine("---- StackTrace ----");
            fullSb.AppendLine(ex.StackTrace);

            if (ex.Data.Count != 0)
            {
                fullSb.AppendLine("---- Data ----");
                foreach (DictionaryEntry o in ex.Data)
                {
                    fullSb.AppendLine($"{o.Key} : {o.Value ?? "null"}");
                }
            }

            if (ex.InnerException == null) return fullSb.ToString();
            fullSb.AppendLine("---- InnerException ----");
            fullSb.AppendLine(FormatFullException(ex.InnerException));

            return fullSb.ToString();
        }
        private static /*async*/ /*Task<JObject>*/ JObject BuildJsonDump(Exception ex)
        {
            var ret = new JObject
            {
                { "tcc_version" , new JValue(App.AppVersion) },
                { "id" , new JValue(App.Settings.LastAccountNameHash != null ? App.Settings.LastAccountNameHash: "") },
                { "tcc_hash", HashUtils.GenerateFileHash(typeof(App).Assembly.Location) },
                { "exception", new JValue(ex.Message)},
                { "exception_type", new JValue(ex.GetType().FullName)},
                { "exception_source", new JValue(ex.Source)},
                { "stack_trace", new JValue(ex.StackTrace)},
                { "full_exception", new JValue(FormatFullException(ex))},
                { "thread_traces", GetThreadTraces() },
                { "game_version", new JValue(PacketAnalyzer.Factory == null ? 0 : PacketAnalyzer.Factory.ReleaseVersion)},
                { "region", new JValue(Game.Server != null ? Game.Server.Region : "")},
                { "server_id", new JValue(Game.Server != null ? Game.Server.ServerId.ToString() : "")},
                { "settings_summary", new JObject
                    {
                        { "windows", new JObject
                            {
                                { "cooldown", App.Settings.CooldownWindowSettings.Enabled },
                                { "buffs", App.Settings.BuffWindowSettings.Enabled },
                                { "character", App.Settings.CharacterWindowSettings.Enabled },
                                { "class", App.Settings.ClassWindowSettings.Enabled },
                                { "chat", App.Settings.ChatEnabled},
                                { "group", App.Settings.GroupWindowSettings.Enabled }
                            }
                        },
                        {
                            "generic", new JObject
                            {
                                { "proxy_enabled", App.Settings.EnableProxy },
                                { "mode", App.ToolboxMode ? "toolbox" : "standalone" }
                            }
                        }
                    }
                },
                {
                    "stats", new JObject
                    {
                        { "os", $"{Environment.OSVersion} {Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")?.GetValue("ProductName")}" },
                        { "current_USER_objects" , GetUSERObjectsCount() },
                        { "used_memory" , Process.GetCurrentProcess().PrivateMemorySize64 },
                        { "uptime", DateTime.Now  - Process.GetCurrentProcess().StartTime},
                    }
                }
            };
            if (ex.InnerException != null)
            {

                var innEx =BuildInnerExceptionJObject(ex.InnerException);
                ret["inner_exception"] = innEx;
            }

            if (ex is PacketParseException ppe)
            {
                ret.Add("packet_opcode_name", new JValue(ppe.OpcodeName));
                ret.Add("packet_data", new JValue(ppe.RawData.ToStringEx()));
            }
            return ret;
        }

        private static JObject BuildInnerExceptionJObject(Exception ex)
        {
            var ret = new JObject();
            ret["exception"] = new JValue(ex.Message);
            ret["exception_type"] = new JValue(ex.GetType().FullName);
            ret["exception_source"] = new JValue(ex.Source);
            ret["stack_trace"] = new JValue(ex.StackTrace);
            if (ex.InnerException != null)
            {
                var innEx = BuildInnerExceptionJObject(ex.InnerException);
                ret["inner_exception"] = innEx;
            }
            return ret;
        }

#pragma warning disable 618
        private static JObject /*Task<JObject>*/ GetThreadTraces()
        {
            //return await Task.Factory.StartNew(() =>
            //{
            //});
            //return App.BaseDispatcher.InvokeAsync(() =>
            //{
                var ret = new JObject();

                WindowManager.RunningDispatchers.Values.Append(App.BaseDispatcher).ToList().ForEach(d =>
                {
                    var t = d.Thread;
                    t.Suspend();
                    ret[t.Name] = new StackTrace(t, false).ToString();
                    t.Resume();
                });

                if (PacketAnalyzer.AnalysisThread == null) return ret;
                PacketAnalyzer.AnalysisThread.Suspend();
                ret["Analysis"] = new StackTrace(PacketAnalyzer.AnalysisThread, false).ToString();
                PacketAnalyzer.AnalysisThread.Resume();

                return ret;
            //}).Result;
        }
#pragma warning restore 618


        private static void DumpCrashToFile(JObject js, Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"id: {js["id"]}");
            sb.AppendLine($"tcc_hash: {js["tcc_hash"]}");
            sb.AppendLine($"game_version: {js["game_version"]}");
            sb.AppendLine($"region: {js["region"]}");
            sb.AppendLine($"server_id: {js["server_id"]}");
            sb.AppendLine($"settings_summary: {js["settings_summary"]}");
            sb.AppendLine($"stats: {js["stats"]}");
            sb.AppendLine($"exception: {js["exception_type"]} {js["exception"]}");
            sb.AppendLine($"{js["full_exception"].ToString().Replace("\\n", "\n")}");
            if (ex is PacketParseException)
            {
                sb.AppendLine($"opcode: {js["packet_opcode_name"]}");
                sb.AppendLine($"data: {js["packet_data"]}");
            }
            sb.AppendLine($"threads");
            sb.AppendLine($"{js["thread_traces"]}");
            Log.F(sb.ToString(), "crash.log");
        }
        private static  void UploadCrashDump(JObject js)
        {
            Log.CW("Uploading crash dump");
            try
            {
                using (var c = MiscUtils.GetDefaultWebClient())
                {
                    c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                    c.Encoding = Encoding.UTF8;

                    c.UploadString(new Uri("https://us-central1-tcc-usage-stats.cloudfunctions.net/crash_report"), Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));
                    Log.CW("Crash dump uploaded");
                }
            }
            catch (Exception e)
            {
                Log.CW($"Failed to upload crash dump: {e}");
            }
        }

        [DllImport("user32.dll")]
        private static extern uint GetGuiResources(IntPtr hProcess, uint uiFlags);

        private static uint GetUSERObjectsCount()
        {
            return GetGuiResources(Process.GetCurrentProcess().Handle, 1);
        }
    }
}
