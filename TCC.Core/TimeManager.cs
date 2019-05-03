using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TCC.Data;
using TCC.Interop;
using TCC.Parsing;
using TCC.Settings;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC
{
    public class TimeManager : TSPropertyChanged
    {
        // TODO: not sure about other regions reset days
        private readonly Dictionary<RegionEnum, TeraServerTimeInfo> _serverTimezones = new Dictionary<RegionEnum, TeraServerTimeInfo>
        {
            {RegionEnum.EU, new TeraServerTimeInfo("Central Europe Standard Time", 6, DayOfWeek.Wednesday, DayOfWeek.Thursday) },
            {RegionEnum.NA, new TeraServerTimeInfo("Central Standard Time", 6, DayOfWeek.Tuesday, DayOfWeek.Thursday) },
            {RegionEnum.RU, new TeraServerTimeInfo("Russian Standard Time", 6, DayOfWeek.Wednesday, DayOfWeek.Thursday) },
            {RegionEnum.TW, new TeraServerTimeInfo("China Standard Time", 6, DayOfWeek.Wednesday, DayOfWeek.Thursday) },
            {RegionEnum.JP, new TeraServerTimeInfo("Tokyo Standard Time", 6, DayOfWeek.Wednesday, DayOfWeek.Thursday) },
            {RegionEnum.THA, new TeraServerTimeInfo("Indochina Time", 6, DayOfWeek.Wednesday, DayOfWeek.Thursday) },
            {RegionEnum.KR, new TeraServerTimeInfo("Korea Standard Time", 6, DayOfWeek.Wednesday, DayOfWeek.Thursday) },
        };

        public const double SecondsInDay = 60 * 60 * 24;
        private const string BaseUrl = "https://tcc-web-99a64.firebaseapp.com/bam";

        private static TimeManager _instance;
        public int ResetHour;
        public static TimeManager Instance => _instance ?? (_instance = new TimeManager());
        public RegionEnum CurrentRegion { get; set; }
        public int ServerHourOffsetFromLocal;
        public int ServerHourOffsetFromUtc;

        public DateTime CurrentServerTime => DateTime.Now.AddHours(ServerHourOffsetFromLocal);

        private TimeManager()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            var s = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            s.Tick += CheckNewDay;
            s.Start();
        }

        private void CheckCloseEvents()
        {
            var closeEventsCount = WindowManager.Dashboard.VM.EventGroups.Count(evGroup => evGroup.Events.Any(x => x.IsClose));
            if (closeEventsCount == 0) return;
            if (SettingsHolder.ShowNotificationBubble) WindowManager.FloatingButton.StartNotifying(closeEventsCount);

        }

        private void CheckNewDay(object sender, EventArgs e)
        {
            if (CurrentServerTime.Hour == 0 && CurrentServerTime.Minute == 0)
                WindowManager.Dashboard.VM.LoadEvents(CurrentServerTime.DayOfWeek, CurrentRegion.ToString());
            if (CurrentServerTime.Second == 0 && CurrentServerTime.Minute % 3 == 0)
                CheckCloseEvents();
        }

        public void SetServerTimeZone(string lang)
        {
            if (string.IsNullOrEmpty(lang)) return;
            CurrentRegion = Utils.RegionEnumFromLanguage(lang);// region.StartsWith("EU") ? "EU" : region;

            SettingsHolder.LastLanguage = lang;
            if (!_serverTimezones.ContainsKey(CurrentRegion))
            {
                CurrentRegion = RegionEnum.EU;
                SettingsHolder.LastLanguage = "EU-EN";
                TccMessageBox.Show("TCC",
                    "Current region could not be detected, so TCC will load EU-EN database. To force a specific language, use Region Override setting in Misc Settings.",
                    MessageBoxButton.OK);
            }
            var timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == _serverTimezones[CurrentRegion].Timezone);
            ResetHour = _serverTimezones[CurrentRegion].ResetHour;

            if (timezone != null)
            {
                ServerHourOffsetFromUtc = timezone.IsDaylightSavingTime(DateTime.UtcNow + timezone.BaseUtcOffset)
                    ? timezone.BaseUtcOffset.Hours + 1
                    : timezone.BaseUtcOffset.Hours;
                ServerHourOffsetFromLocal = -TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours + ServerHourOffsetFromUtc;
            }

            if (WindowManager.Dashboard.VM.Markers.FirstOrDefault(x => x.Name.Equals(CurrentRegion + " server time")) == null)
            {
                WindowManager.Dashboard.VM.Markers.Add(new TimeMarker(ServerHourOffsetFromLocal, CurrentRegion + " server time"));
            }

            CheckReset();
            WindowManager.Dashboard.VM.LoadEvents(DateTime.Now.DayOfWeek, CurrentRegion.ToString());

        }

        private void CheckReset()
        {
            var todayReset = DateTime.Today.AddHours(ResetHour + ServerHourOffsetFromLocal);
            if (SettingsHolder.LastRun > todayReset || DateTime.Now < todayReset) return;

            WindowManager.Dashboard.VM.ResetDailyData();

            var weeklyDungeonsReset = DateTime.Now.DayOfWeek == _serverTimezones[CurrentRegion].DungeonsWeeklyResetDay && PacketAnalyzer.Factory.ReleaseVersion >= 8000;
            var weeklyVanguardReset = DateTime.Now.DayOfWeek == _serverTimezones[CurrentRegion].VanguardResetDay;

            if (weeklyDungeonsReset) WindowManager.Dashboard.VM.ResetWeeklyDungeons();
            if (weeklyVanguardReset) WindowManager.Dashboard.VM.ResetVanguardWeekly();

            WindowManager.Dashboard.VM.SaveCharacters();
            SettingsHolder.LastRun = DateTime.Now;
            SettingsWriter.Save();
        }


        private async Task<long> DownloadGuildBamTimestamp()
        {
            try
            {
                var sb = new StringBuilder(BaseUrl);
                sb.Append("?srv=");
                sb.Append(SessionManager.Server.ServerId);
                sb.Append("&reg=");
                sb.Append(CurrentRegion);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var c = new WebClient();
                c.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");

                var data = await c.DownloadStringTaskAsync(sb.ToString());
                return long.Parse(data);

            }
            catch
            {

                ChatWindowManager.Instance.AddTccMessage("Failed to retrieve guild BAM info.");
                WindowManager.FloatingButton.NotifyExtended("Guild BAM", "Failed to retrieve guild BAM info.", NotificationType.Error);

                return 0;
            }
        }

        public void UploadGuildBamTimestamp()
        {
            var sb = new StringBuilder(BaseUrl);
            sb.Append("?srv=");
            sb.Append(SessionManager.Server.ServerId);
            sb.Append("&reg=");
            sb.Append(CurrentRegion);
            sb.Append("&post");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var c = new WebClient();
            c.Headers.Set("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");

            try
            {
                c.UploadDataAsync(new Uri(sb.ToString()), new byte[] { });
            }
            catch
            {
                ChatWindowManager.Instance.AddTccMessage("Failed to upload guild BAM info.");
                WindowManager.FloatingButton.NotifyExtended("Guild BAM", "Failed to upload guild BAM info.", NotificationType.Error);

            }

        }
        public DateTime RetrieveGuildBamDateTime()
        {
            var t = DownloadGuildBamTimestamp();
            t.Wait();
            var ts = t.Result;
            return Utils.FromUnixTime(ts);
        }

        public void SetGuildBamTime(bool force)
        {
            foreach (var eg in WindowManager.Dashboard.VM.EventGroups.ToSyncList().Where(x => x.RemoteCheck))
            {
                foreach (var ev in eg.Events.ToSyncList())
                {
                    ev.UpdateFromServer(force);
                }
            }
        }

        public void ExecuteGuildBamWebhook(bool testMessage = false)
        {
            if (string.IsNullOrEmpty(SettingsHolder.WebhookUrlGuildBam)) return;
            if (!SettingsHolder.WebhookEnabledGuildBam) return;

            SendWebhook(SettingsHolder.WebhookMessageGuildBam, SettingsHolder.WebhookUrlGuildBam, testMessage);
        }
        //=====================================by HQ 20181224=====================================
        //public void ExecuteFieldBossWebhook(int bossId, int status, bool testMessage = false)
        //{
        //    if (TimeManager.Instance.CurrentRegion == RegionEnum.KR) // by HQ 20190321
        //    {
        //        if (!string.IsNullOrEmpty(SettingsHolder.Webhook))
        //        {
        //            var sb = new StringBuilder("{");
        //            sb.Append("\""); sb.Append("content"); sb.Append("\"");
        //            sb.Append(":");

        //            if ((bossId == 501) && (status == 1))
        //            {
        //                sb.Append("\""); sb.Append("하자르 등장 " + DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd HH:mm tt"));
        //            }
        //            if ((bossId == 501) && (status == 2))
        //            {
        //                sb.Append("\""); sb.Append("하자르 퇴치 " + DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd HH:mm tt"));
        //            }
        //            if ((bossId == 4001) && (status == 1))
        //            {
        //                sb.Append("\""); sb.Append("캘로스 등장 " + DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd HH:mm tt"));
        //            }
        //            if ((bossId == 4001) && (status == 2))
        //            {
        //                sb.Append("\""); sb.Append("캘로스 퇴치 " + DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd HH:mm tt"));
        //            }
        //            if ((bossId == 5001) && (status == 1))
        //            {
        //                sb.Append("\""); sb.Append("오르탄 등장 " + DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd HH:mm tt"));
        //            }
        //            if ((bossId == 5001) && (status == 2))
        //            {
        //                sb.Append("\""); sb.Append("오르탄 퇴치 " + DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd HH:mm tt"));
        //            }


        //            if (testMessage) sb.Append(" (Test message)");
        //            sb.Append("\"");
        //            sb.Append(",");
        //            sb.Append("\""); sb.Append("username"); sb.Append("\"");
        //            sb.Append(":");
        //            sb.Append("\""); sb.Append("TCC"); sb.Append("\"");
        //            sb.Append(",");
        //            sb.Append("\""); sb.Append("avatar_url"); sb.Append("\"");
        //            sb.Append(":");
        //            sb.Append("\""); sb.Append("http://i.imgur.com/8IltuVz.png"); sb.Append("\"");
        //            sb.Append("}");

        //            try
        //            {
        //                if (!testMessage)
        //                {
        //                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //                    using (var client = new WebClient())
        //                    {
        //                        client.Encoding = Encoding.UTF8;
        //                        client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36");
        //                        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

        //                        var Webhook = "";
        //                        Webhook = "https://discordapp.com/api/webhooks/527967085490339850/n0vLSOYWZJM4soIixVHw9gQgtNKtRSU7K-PtPrjcN9squDEbTUunrggFk_fVCQK-u2im";
        //                        Log.F("FieldBoss.log", $"\n[{nameof(ExecuteFieldBossWebhook)}] {Webhook}");
        //                        client.UploadString(Webhook, "POST", sb.ToString());
        //                    }
        //                }
        //            }
        //            catch (Exception)
        //            {
        //                WindowManager.FloatingButton.NotifyExtended("FieldBoss", "Failed to execute Discord webhook.", NotificationType.Error);
        //                ChatWindowManager.Instance.AddTccMessage("Failed to execute Discord webhook.");
        //            }
        //        }
        //    }
        //}
        //=============================================================================================
        public void ExecuteFieldBossSpawnWebhook(string monsterName, string regionName, string defaultMessage, bool testMessage = false)
        {
            var content = SettingsHolder.WebhookMessageFieldBossSpawn;
            if (content.Contains("{bossName}")) content = content.Replace("{bossName}", monsterName);
            if (content.Contains("{regionName}")) content = content.Replace("{regionName}", regionName);
            if (content.Contains("{time}")) content = content.Replace("{time}", DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd HH:mm tt"));

            if (content == "") content = defaultMessage;
            SendWebhook(content, SettingsHolder.WebhookUrlFieldBoss, testMessage);
        }
        public void ExecuteFieldBossDieWebhook(string monsterName, string defaultMessage, string userName, string guildName ,bool testMessage = false)
        {
            var content = SettingsHolder.WebhookMessageFieldBossDie;
            if (content.Contains("{bossName}")) content = content.Replace("{bossName}", monsterName);
            if (content.Contains("{time}")) content = content.Replace("{time}", DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd HH:mm tt"));
            if (content.Contains("{userName}")) content = content.Replace("{userName}", userName);
            if (content.Contains("{guildName}")) content = content.Replace("{guildName}", guildName);
            if (content == "") content = defaultMessage;
            SendWebhook(content, SettingsHolder.WebhookUrlFieldBoss, testMessage);
        }

        private static void SendWebhook(string content, string url, bool test = false)
        {
            Discord.FireWebhook(url, $"{content}{(test ? " (test message)" : "")}");
            //var msg = new JObject
            //{
            //    {"content", $"{content}{(test ? " (test message)" : "")}"},
            //    {"username", "TCC" },
            //    {"avatar_url", "http://i.imgur.com/8IltuVz.png" }
            //};

            //try
            //{
            //    using (var client = Utils.GetDefaultWebClient())
            //    {
            //        client.Encoding = Encoding.UTF8;
            //        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            //        client.UploadString(url, "POST", msg.ToString());
            //    }
            //}
            //catch (Exception)
            //{
            //    WindowManager.FloatingButton.NotifyExtended("TCC", "Failed to execute Discord webhook.", NotificationType.Error);
            //    ChatWindowManager.Instance.AddTccMessage("Failed to execute Discord webhook.");
            //}
        }
    }
}
