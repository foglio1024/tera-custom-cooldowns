using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.Parsing;
using TCC.Settings;
using TCC.Windows;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC
{
    public static class GlobalExceptionHandler
    {
        public static void HandleGlobalException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            DumpCrashToFile(ex);
            //#if !DEBUG
            try { new Thread(() => UploadCrashDump(ex)).Start(); }
            catch { /*ignored*/ }
            //#endif

            TccMessageBox.Show("TCC",
                "An error occured and TCC will now close. Report this issue to the developer attaching crash.log from TCC folder.",
                MessageBoxButton.OK, MessageBoxImage.Error);

            App.ReleaseMutex();
            ProxyInterface.Instance.Disconnect();
            if (WindowManager.TrayIcon != null) WindowManager.TrayIcon.Dispose();

            try { WindowManager.Dispose(); }
            catch {/* ignored*/}

            try
            {
                Firebase.RegisterWebhook(SettingsHolder.WebhookUrlGuildBam, false);
                Firebase.RegisterWebhook(SettingsHolder.WebhookUrlFieldBoss, false);
            }
            catch { }
            Environment.Exit(-1);
        }

        private static string FormatFullException(Exception ex)
        {
            var fullSb = new StringBuilder();
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
        private static JObject BuildJsonDump(Exception ex)
        {

            return new JObject
            {
                { "tcc_version" , new JValue(App.AppVersion) },
                { "id" , new JValue(SessionManager.CurrentAccountName != null ? Utils.GenerateHash(SessionManager.CurrentAccountName) : "") },
                { "tcc_hash", Utils.GenerateFileHash(typeof(App).Assembly.Location) },
                { "exception", new JValue(ex.Message)},
                { "full_exception", new JValue(FormatFullException(ex))},
                { "inner_exception",new JValue(ex.InnerException != null ? ex.InnerException.Message : "undefined") },
                { "game_version", new JValue(PacketAnalyzer.Factory == null ? 0 : PacketAnalyzer.Factory.ReleaseVersion)},
                { "region", new JValue(SessionManager.Server != null ? SessionManager.Server.Region : "")},
                { "server_id", new JValue(SessionManager.Server != null ? SessionManager.Server.ServerId.ToString() : "")},
                { "settings_summary", new JObject
                    {
                        { "windows", new JObject
                            {
                                { "cooldown", SettingsHolder.CooldownWindowSettings.Enabled },
                                { "buffs", SettingsHolder.BuffWindowSettings.Enabled },
                                { "character", SettingsHolder.CharacterWindowSettings.Enabled },
                                { "class", SettingsHolder.ClassWindowSettings.Enabled },
                                { "chat", SettingsHolder.ChatEnabled},
                                { "group", SettingsHolder.GroupWindowSettings.Enabled }
                            }
                        },
                        {
                            "generic", new JObject
                            {
                                { "proxy_enabled", SettingsHolder.EnableProxy },
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
        }

        private static void DumpCrashToFile(Exception ex)
        {
            //if (ex is TaskCanceledException tce)
            //{
            //    // TODO
            //}
            var sb = new StringBuilder();
            var js = BuildJsonDump(ex);
            sb.AppendLine($"id: {js["id"]}");
            sb.AppendLine($"tcc_hash: {js["tcc_hash"]}");
            sb.AppendLine($"game_version: {js["game_version"]}");
            sb.AppendLine($"region: {js["region"]}");
            sb.AppendLine($"server_id: {js["server_id"]}");
            sb.AppendLine($"settings_summary: {js["settings_summary"]}");
            sb.AppendLine($"stats: {js["stats"]}");
            sb.AppendLine($"{js["full_exception"].ToString().Replace("\\n", "\n")}");
            Log.F(sb.ToString(), "crash.log");
        }
        private static void UploadCrashDump(Exception ex)
        {
            using (var c = Utils.GetDefaultWebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;

                var js = BuildJsonDump(ex);

                c.UploadString(new Uri("https://us-central1-tcc-usage-stats.cloudfunctions.net/crash_report"),
                               Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));
            }
        }

        [DllImport("user32.dll")]
        static extern uint GetGuiResources(IntPtr hProcess, uint uiFlags);

        private static uint GetUSERObjectsCount()
        {
            return GetGuiResources(Process.GetCurrentProcess().Handle, 1);
        }
    }
}
