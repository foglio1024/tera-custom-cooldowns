using System;
using System.Net;
using System.Text;
using System.Windows;
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
#if !DEBUG
            try { new Thread(() => UploadCrashDump(ex)).Start(); }
            catch { /*ignored*/ }
#endif

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

        private static void DumpCrashToFile(Exception ex)
        {
            //if (ex is TaskCanceledException tce)
            //{
            //    // TODO
            //}
            var sb = new StringBuilder("\r\n\r\n");
            sb.AppendLine($"##### {App.AppVersion} - {DateTime.Now:dd/MM/yyyy HH:mm:ss} #####");
            sb.Append($"Version: {PacketAnalyzer.Factory.Version}");
            if (SessionManager.Server != null)
            {
                sb.Append($" - Region: {SessionManager.Server.Region}");
                sb.Append($" - Server:{SessionManager.Server.ServerId}");
            }
            sb.AppendLine();
            sb.AppendLine($"{ex.Message}");
            sb.AppendLine($"{ex.StackTrace}");
            sb.AppendLine($"Source: {ex.Source}");
            sb.AppendLine($"Data: {ex.Data}");
            if (ex.InnerException != null) sb.AppendLine($"InnerException: \n{ex.InnerException}");
            sb.AppendLine($"TargetSite: {ex.TargetSite}");
            Log.F(sb.ToString(), "crash.log");
        }
        private static void UploadCrashDump(Exception ex)
        {

            using (var c = Utils.GetDefaultWebClient())
            {
                c.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                c.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                c.Encoding = Encoding.UTF8;

                var full = $"{ex.Message}\r\n{ex.StackTrace}\r\n{ex.Source}\r\n{ex}\r\n{ex.Data}\r\n{ex.InnerException}\r\n{ex.TargetSite}";
                var js = new JObject
                {
                    { "tcc_version" , new JValue(App.AppVersion) },
                    { "tcc_hash", Utils.GenerateFileHash(typeof(App).Assembly.Location) },
                    { "exception", new JValue(ex.Message)},
                    { "full_exception", new JValue(full)},
                    { "inner_exception",new JValue(ex.InnerException != null ? ex.InnerException.Message : "undefined") },
                    { "game_version", new JValue(PacketAnalyzer.Factory.ReleaseVersion)},
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
                    }
                };

                c.UploadString(new Uri("https://us-central1-tcc-usage-stats.cloudfunctions.net/crash_report"),
                               Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(js.ToString())));
            }
        }
    }
}
