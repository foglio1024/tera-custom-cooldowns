using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Nostrum;
using Nostrum.Extensions;
using Nostrum.WinAPI;
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Microsoft.Diagnostics.Runtime;
using TCC.Analysis;
using TCC.Data;
using TCC.Exceptions;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.UI;
using TCC.UI.Windows;
using TCC.Utils;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC
{
    public static class GlobalExceptionHandler
    {
        private static readonly int[] _excludedWin32codes =
        {
            1816  // "Not enough quota"
        };
        public static void HandleGlobalException(object sender, UnhandledExceptionEventArgs e)
        {
            FocusManager.Dispose();
            HandleGlobalExceptionImpl(e);
        }

        private static void HandleGlobalExceptionImpl(UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var js = BuildJsonDump(ex); //await App.BaseDispatcher.InvokeAsync(() => BuildJsonDump(ex));
            DumpCrashToFile(js, ex);

            switch (ex)
            {
                case COMException com when (com.HResult == 88980406 || com.Message.Contains("UCEERR_RENDERTHREADFAILURE")):
                {
                    TccMessageBox.Show("TCC", SR.RenderThreadError, MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }
                case ClientVersionDetectionException cvde:
                {
                    Log.F($"Failed to detect client version from file: {cvde}");
                    TccMessageBox.Show(SR.CannotDetectClientVersion(StubInterface.Instance.IsStubAvailable), MessageBoxType.Error);
                    break;
                }
                case Win32Exception w32ex when _excludedWin32codes.Contains(w32ex.NativeErrorCode):
                {
                    Log.F(w32ex.ToString());
                    TccMessageBox.Show("TCC", SR.FatalError, MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }
                default:
                {
                    UploadCrashDump(js);
                    TccMessageBox.Show("TCC", SR.FatalError, MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }
            }


            StubInterface.Instance.Disconnect();
            Firebase.Dispose();

            if (!(ex is DeadlockException))
            {
                // These actions require main thread to be alive
                App.ReleaseMutex();
                if (WindowManager.TrayIcon != null) WindowManager.TrayIcon.Dispose();
                try { WindowManager.Dispose(); } catch {/* ignored*/}
            }


            Environment.Exit(-1);

        }
        private static string FormatFullException(Exception ex)
        {
            var fullSb = new StringBuilder();
            fullSb.AppendLine("---- Exception ----");
            fullSb.AppendLine(ex.GetType().FullName);
            fullSb.AppendLine("---- Message ----");
            if (ex is Win32Exception w32ex)
            {
                fullSb.AppendLine($"{ex.Message} HRESULT:{w32ex.HResult} ErrorCode:{w32ex.ErrorCode} NativeCode:{w32ex.NativeErrorCode}");
            }
            else
            {
                fullSb.AppendLine(ex.Message);
            }
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

        private static JValue BuildExceptionMessage(Exception ex)
        {
            if (ex is Win32Exception w32ex)
            {
                return new JValue($"{ex.Message} HRESULT:{w32ex.HResult} ErrorCode:{w32ex.ErrorCode} NativeCode:{w32ex.NativeErrorCode}");
            }

            return new JValue(ex.Message);

        }
        private static /*async*/ /*Task<JObject>*/ JObject BuildJsonDump(Exception ex)
        {
            var ret = new JObject
            {
                { "tcc_version" , new JValue(App.AppVersion) },
                { "id" , new JValue(App.Settings.LastAccountNameHash != null ? App.Settings.LastAccountNameHash: "") },
                { "tcc_hash", HashUtils.GenerateFileHash(typeof(App).Assembly.Location) },
                { "exception", BuildExceptionMessage(ex)},
                { "exception_type", new JValue(ex.GetType().FullName)},
                { "exception_source", new JValue(ex.Source)},
                { "stack_trace", new JValue(ex.StackTrace)},
                { "full_exception", new JValue(FormatFullException(ex))},
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
                                { "npc", App.Settings.NpcWindowSettings.Enabled},
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

                var innEx = BuildInnerExceptionJObject(ex.InnerException);
                ret["inner_exception"] = innEx;
            }

            if (ex is PacketParseException ppe)
            {
                ret.Add("packet_opcode_name", new JValue(ppe.OpcodeName));
                ret.Add("packet_data", new JValue(ppe.RawData.ToStringEx()));
            }

            if (ex is DeadlockException de)
            {
                ret.Add("thread_traces", GetThreadTraces(de));
            }

            return ret;
        }

        private static JObject BuildInnerExceptionJObject(Exception ex)
        {
            var ret = new JObject
            {
                ["exception"] = BuildExceptionMessage(ex),
                ["exception_type"] = new JValue(ex.GetType().FullName),
                ["exception_source"] = new JValue(ex.Source),
                ["stack_trace"] = new JValue(ex.StackTrace)
            };
            if (ex.InnerException == null) return ret;
            var innEx = BuildInnerExceptionJObject(ex.InnerException);
            ret["inner_exception"] = innEx;
            return ret;
        }

#pragma warning disable 618
        private static JObject /*Task<JObject>*/ GetThreadTraces(DeadlockException de)
        {

            using var dataTarget = DataTarget.CreateSnapshotAndAttach(Process.GetCurrentProcess().Id);

            var version = dataTarget.ClrVersions[0];
            var runtime = version.CreateRuntime();

            var ret = new JObject();
            var threads = App.RunningDispatchers.Values.Append(App.BaseDispatcher).Where(d => de.ThreadNames.Contains(d.Thread.Name)).Select(d => d.Thread).Append(PacketAnalyzer.AnalysisThread);
            foreach (var thread in threads)
            {
                if (thread?.Name == null) continue;
                var runtimeThread = runtime.Threads.FirstOrDefault(t => t.ManagedThreadId == thread.ManagedThreadId);
                if (runtimeThread != null)
                {
                    ret[thread.Name] = GetStackTraceClrmd(runtimeThread);
                }
                //ret[thread.Name] = GetStackTrace(thread).ToString();
            }
            //App.RunningDispatchers.Values.Append(App.BaseDispatcher).ToList().ForEach(d =>
            //{
            //    var t = d.Thread;
            //    t.Suspend();
            //    ret[t.Name] = new StackTrace(t, false).ToString();
            //    t.Resume();
            //});

            //if (PacketAnalyzer.AnalysisThread == null) return ret;
            //PacketAnalyzer.AnalysisThread.Suspend();
            //ret["Analysis"] = new StackTrace(PacketAnalyzer.AnalysisThread, false).ToString();
            //PacketAnalyzer.AnalysisThread.Resume();

            return ret;
        }

        private static string GetStackTraceClrmd(ClrThread runtimeThread)
        {
            var sb = new StringBuilder();
            foreach (var frame in runtimeThread.EnumerateStackTrace())
            {
                if (frame.Method == null) continue;
                sb.AppendLine($"   in {frame.Method}");
            }

            return sb.ToString();
        }
#pragma warning restore 618
#if false
        private static StackTrace GetStackTrace(Thread targetThread)
        {
            using var fallbackThreadReady = new ManualResetEvent(false);
            using var exitedSafely = new ManualResetEvent(false);
            var fallbackThread = new Thread(delegate ()
            {
                fallbackThreadReady.Set();
                while (!exitedSafely.WaitOne(200))
                {
                    try
                    {
                        targetThread.Resume();
                    }
                    catch (Exception)
                    {
                        /*Whatever happens, do never stop to resume the target-thread regularly until the main-thread has exited safely.*/
                    }
                }
            })
            { Name = "GetStackFallbackThread" };

            try
            {
                fallbackThread.Start();
                fallbackThreadReady.WaitOne();
                //From here, you have about 200ms to get the stack-trace.
                targetThread.Suspend();
                StackTrace trace = null;
                try
                {
                    trace = new StackTrace(targetThread, true);
                }
                catch (ThreadStateException)
                {
                    //failed to get stack trace, since the fallback-thread resumed the thread
                    //possible reasons:
                    //1.) This thread was just too slow (not very likely)
                    //2.) The deadlock ocurred and the fallbackThread rescued the situation.
                    //In both cases just return null.
                }
                try
                {
                    targetThread.Resume();
                }
                catch (ThreadStateException) {/*Thread is running again already*/}
                return trace;
            }
            finally
            {
                //Just signal the backup-thread to stop.
                exitedSafely.Set();
                //Join the thread to avoid disposing "exited safely" too early. And also make sure that no leftover threads are cluttering iis by accident.
                fallbackThread.Join();
            }
        }
#endif
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
            sb.AppendLine($"{js["full_exception"]?.ToString().Replace("\\n", "\n")}");
            if (ex is PacketParseException)
            {
                sb.AppendLine($"opcode: {js["packet_opcode_name"]}");
                sb.AppendLine($"data: {js["packet_data"]}");
            }
            sb.AppendLine($"threads");
            sb.AppendLine($"{js["thread_traces"]}");
            Log.F(sb.ToString(), "crash.log");
        }
        private static void UploadCrashDump(JObject js)
        {
            Log.CW("Uploading crash dump");
            try
            {
                using var c = MiscUtils.GetDefaultWebClient();
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;

                c.UploadString(new Uri("https://us-central1-tcc-usage-stats.cloudfunctions.net/crash_report"), Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));
                Log.CW("Crash dump uploaded");
            }
            catch (Exception e)
            {
                Log.CW($"Failed to upload crash dump: {e}");
            }
        }


        private static uint GetUSERObjectsCount()
        {
            return User32.GetGuiResources(Process.GetCurrentProcess().Handle, 1);
        }
    }
}
