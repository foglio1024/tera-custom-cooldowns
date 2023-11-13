using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Nostrum;
using Nostrum.WPF.ThreadSafe;
using TCC.Interop;
using TCC.UI;
using TCC.UI.Windows;
using TCC.Utilities;
using TCC.Utils;
using TCC.ViewModels;
using TeraDataLite;
using MessageBoxImage = TCC.Data.MessageBoxImage;

namespace TCC;

//TODO: big refactor here
public class GameEventManager : ThreadSafeObservableObject
{
    static GameEventManager? _instance;
    public static GameEventManager Instance => _instance ??= new GameEventManager();

    // TODO: not sure about other regions reset days
    readonly Dictionary<RegionEnum, TeraServerTimeInfo> _serverTimezones = new()
    {
        { RegionEnum.EU,  new TeraServerTimeInfo("Central Europe Standard Time", 6, DayOfWeek.Wednesday, DayOfWeek.Thursday) },
        { RegionEnum.NA,  new TeraServerTimeInfo("Central Standard Time",        6, DayOfWeek.Tuesday,   DayOfWeek.Thursday) },
        { RegionEnum.RU,  new TeraServerTimeInfo("Russian Standard Time",        6, DayOfWeek.Wednesday, DayOfWeek.Thursday) },
        { RegionEnum.TW,  new TeraServerTimeInfo("China Standard Time",          6, DayOfWeek.Wednesday, DayOfWeek.Thursday) },
        { RegionEnum.JP,  new TeraServerTimeInfo("Tokyo Standard Time",          6, DayOfWeek.Wednesday, DayOfWeek.Thursday) },
        { RegionEnum.THA, new TeraServerTimeInfo("Indochina Time",               6, DayOfWeek.Wednesday, DayOfWeek.Thursday) },
        { RegionEnum.KR,  new TeraServerTimeInfo("Korea Standard Time",          6, DayOfWeek.Wednesday, DayOfWeek.Thursday) }
    };

    public const double SecondsInDay = 60 * 60 * 24;
    const string BaseUrl = "https://tcc-web-99a64.firebaseapp.com/bam"; //  todo: replace this

    public int ResetHour;
    public RegionEnum CurrentRegion { get; set; }
    public int ServerHourOffsetFromLocal;
    public int ServerHourOffsetFromUtc;

    public DateTime CurrentServerTime => DateTime.Now.AddHours(ServerHourOffsetFromLocal);

    GameEventManager()
    {
        var s = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        s.Tick += CheckNewDay;
        s.Start();
    }

    void CheckCloseEvents()
    {
        var closeEventsCount = WindowManager.ViewModels.DashboardVM.EventGroups.Count(evGroup => evGroup.Events.Any(x => x.IsClose));
        if (closeEventsCount == 0) return;
        if (App.Settings.FloatingButtonSettings.ShowNotificationBubble) WindowManager.ViewModels.FloatingButtonVM.WarnCloseEvents(closeEventsCount);

    }

    void CheckNewDay(object? sender, EventArgs e)
    {
        if (CurrentServerTime is { Hour: 0, Minute: 0 })
            WindowManager.ViewModels.DashboardVM.LoadEvents(CurrentServerTime.DayOfWeek, CurrentRegion.ToString());
        if (CurrentServerTime.Second == 0 && CurrentServerTime.Minute % 3 == 0)
            CheckCloseEvents();
    }

    public void SetServerTimeZone(string lang)
    {
        if (string.IsNullOrEmpty(lang)) return;
        CurrentRegion = TccUtils.RegionEnumFromLanguage(lang);// region.StartsWith("EU") ? "EU" : region;

        //App.Settings.LastLanguage = lang; //TODO: needed?
        if (!_serverTimezones.ContainsKey(CurrentRegion))
        {
            CurrentRegion = RegionEnum.EU;
            App.Settings.LastLanguage = "EU-EN";
            TccMessageBox.Show("TCC", SR.CannotDetectCurrentRegion, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        var timezone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.Id == _serverTimezones[CurrentRegion].Timezone);
        ResetHour = _serverTimezones[CurrentRegion].ResetHour;

        if (timezone != null)
        {
            ServerHourOffsetFromUtc = timezone.IsDaylightSavingTime(DateTime.UtcNow + timezone.BaseUtcOffset)
                ? timezone.BaseUtcOffset.Hours + 1
                : timezone.BaseUtcOffset.Hours;
            ServerHourOffsetFromLocal = -TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours + ServerHourOffsetFromUtc;
        }

        if (WindowManager.ViewModels.DashboardVM.Markers.FirstOrDefault(x => x.Name.Equals(CurrentRegion + " server time")) == null)
        {
            WindowManager.ViewModels.DashboardVM.Markers.Add(new TimeMarker(ServerHourOffsetFromLocal, CurrentRegion + " server time"));
        }

        CheckReset();
        WindowManager.ViewModels.DashboardVM.LoadEvents(DateTime.Now.DayOfWeek, CurrentRegion.ToString());

    }

    void CheckReset()
    {
        var todayReset = DateTime.Today.AddHours(ResetHour + ServerHourOffsetFromLocal);
        if (App.Settings.LastRun > todayReset || DateTime.Now < todayReset) return;

        WindowManager.ViewModels.DashboardVM.ResetDailyData();

        var weeklyDungeonsReset = DateTime.Now.DayOfWeek == _serverTimezones[CurrentRegion].DungeonsWeeklyResetDay;
        var weeklyVanguardReset = DateTime.Now.DayOfWeek == _serverTimezones[CurrentRegion].VanguardResetDay;

        if (weeklyDungeonsReset) WindowManager.ViewModels.DashboardVM.ResetWeeklyDungeons();
        if (weeklyVanguardReset) WindowManager.ViewModels.DashboardVM.ResetVanguardWeekly();

        WindowManager.ViewModels.DashboardVM.SaveCharacters();
        App.Settings.LastRun = DateTime.Now;
        App.Settings.Save();
    }

    async Task<long> DownloadGuildBamTimestamp()
    {
        try
        {
            var sb = new StringBuilder(BaseUrl);
            sb.Append("?srv=");
            sb.Append(Game.Server.ServerId);
            sb.Append("&reg=");
            sb.Append(CurrentRegion);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var c = MiscUtils.GetDefaultHttpClient();

            var data = await c.GetStringAsync(sb.ToString());
            return long.Parse(data);

        }
        catch
        {

            Log.Chat(SR.CannotRetrieveGbamInfo);
            Log.N("Guild BAM", SR.CannotRetrieveGbamInfo, NotificationType.Error);

            return 0;
        }
    }

    public void UploadGuildBamTimestamp()
    {
        try
        {
            var c = MiscUtils.GetDefaultHttpClient();
            c.PostAsync($"{BaseUrl}?srv={Game.Server.ServerId}&reg={CurrentRegion}&post", new ByteArrayContent(new byte[]{}));
        }
        catch
        {
            Log.Chat(SR.CannotUploadGbamInfo);
            Log.N("Guild BAM", SR.CannotUploadGbamInfo, NotificationType.Error);
        }

    }
    public async Task<DateTime> RetrieveGuildBamDateTime()
    {
        var t = await DownloadGuildBamTimestamp();
        return TimeUtils.FromUnixTime(t);
    }

    public static void ExecuteGuildBamWebhook(bool testMessage = false)
    {
        if (!App.Settings.WebhookEnabledGuildBam) return;
        if (string.IsNullOrEmpty(App.Settings.WebhookUrlGuildBam)) return;

        SendWebhook(App.Settings.WebhookMessageGuildBam, App.Settings.WebhookUrlGuildBam, testMessage);
    }
    public static void ExecuteFieldBossSpawnWebhook(string monsterName, string regionName, string defaultMessage, bool testMessage = false)
    {
        var content = App.Settings.WebhookMessageFieldBossSpawn;
        if (content.Contains("{bossName}")) content = content.Replace("{bossName}", monsterName);
        if (content.Contains("{regionName}")) content = content.Replace("{regionName}", regionName);
        if (content.Contains("{time}")) content = content.Replace("{time}", DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd HH:mmtt"));

        if (content == "") content = defaultMessage;
        SendWebhook(content, App.Settings.WebhookUrlFieldBoss, testMessage);
    }
    public static void ExecuteFieldBossDieWebhook(string monsterName, string defaultMessage, string userName, string guildName, bool testMessage = false)
    {
        var content = App.Settings.WebhookMessageFieldBossDie;
        if (content.Contains("{bossName}")) content = content.Replace("{bossName}", monsterName);
        if (content.Contains("{time}")) content = content.Replace("{time}", DateTime.UtcNow.ToLocalTime().ToString("yyyy/MM/dd HH:mmtt"));
        if (content.Contains("{userName}")) content = content.Replace("{userName}", userName);
        if (content.Contains("{guildName}")) content = content.Replace("{guildName}", guildName);
        if (content == "") content = defaultMessage;
        SendWebhook(content, App.Settings.WebhookUrlFieldBoss, testMessage);
    }

    static void SendWebhook(string content, string url, bool test = false)
    {
        Discord.FireWebhook(url, $"{content}{(test ? " (test message)" : "")}", App.AppVersion, App.Settings.LastAccountNameHash);
    }

    struct TeraServerTimeInfo
    {
        public readonly string Timezone;
        public readonly int ResetHour;
        public readonly DayOfWeek VanguardResetDay;
        public readonly DayOfWeek DungeonsWeeklyResetDay;

        public TeraServerTimeInfo(string tz, int rh, DayOfWeek vanguardReset, DayOfWeek dungeonWeeklyReset)
        {
            Timezone = tz;
            ResetHour = rh;
            VanguardResetDay = vanguardReset;
            DungeonsWeeklyResetDay = dungeonWeeklyReset;
        }
    }
    //=====================================by HQ 20181224=====================================
    //public void ExecuteFieldBossWebhook(int bossId, int status, bool testMessage = false)
    //{
    //    if (TimeManager.Instance.CurrentRegion == RegionEnum.KR) // by HQ 20190321
    //    {
    //        if (!string.IsNullOrEmpty(App.Settings.Webhook))
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
    //                WindowManager.ViewModels.NotificationArea.Enqueue("FieldBoss", "Failed to execute Discord webhook.", NotificationType.Error);
    //                ChatWindowManager.Instance.AddTccMessage("Failed to execute Discord webhook.");
    //            }
    //        }
    //    }
    //}
    //=============================================================================================

}