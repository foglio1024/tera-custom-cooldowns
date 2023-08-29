using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Newtonsoft.Json.Linq;
using Nostrum;
using TCC.Data;
using TCC.Interop;
using TCC.Interop.Proxy;
using TCC.UI;
using TCC.UI.Windows;
using TCC.Utils;
using TCC.Utils.Exceptions;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC.Utilities
{
    public static class GlobalExceptionHandler
    {
        private static readonly int[] _excludedWin32codes =
        {
            1816  // "Not enough quota"
        };

        public static void OnGlobalException(object sender, UnhandledExceptionEventArgs e)
        {
            FocusManager.Dispose();
            HandleGlobalException(e);
        }

        private static void HandleGlobalException(UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var js = ExceptionReportBuilder.BuildJsonCrashReport(ex);
            WriteCrashReportToFile(js, ex);

            switch (ex)
            {
                case COMException { HResult: 88980406 }:
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
                case OutOfMemoryException:
                    {
                        TccMessageBox.Show("TCC", SR.OutOfMemoryError, MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }
                default:
                    {
                        var res = TccMessageBox.Show("TCC", SR.FatalErrorAskUpload, MessageBoxButton.YesNo, MessageBoxImage.Error);
                        if (res == MessageBoxResult.Yes)
                        {
                            UploadCrashReport(js);
                        }
                        break;
                    }
            }

            StubInterface.Instance.Disconnect();
            Firebase.Dispose();

            if (ex is not DeadlockException)
            {
                // These actions require main thread to be alive
                App.ReleaseMutex();
                WindowManager.TrayIcon.Dispose();
                try { WindowManager.Dispose(); } catch {/* ignored*/}
            }

            Environment.Exit(-1);
        }

        private static void WriteCrashReportToFile(JObject js, Exception ex)
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

        private static void UploadCrashReport(JObject js)
        {
            try
            {
                using var c = MiscUtils.GetDefaultWebClient();
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Headers[HttpRequestHeader.UserAgent] = "TCC/Windows";
                c.Encoding = Encoding.UTF8;
                c.UploadString(new Uri("https://www.foglio1024.it/api/crash-reports"), Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));
            }
            catch (Exception e)
            {
                //Log.CW($"Failed to upload crash report: {e}");
            }
        }
    }
}