using Newtonsoft.Json.Linq;
using Nostrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TCC.Data;
using TCC.Utils;
using TeraPacketParser.Analysis;
using TeraPacketParser.TeraCommon.Game;

namespace TCC.Notice
{
    public static class NoticeChecker
    {
        private const string Url = "https://raw.githubusercontent.com/Foglio1024/Tera-custom-cooldowns/master/messages.json";
        private static List<NoticeBase> _notices = new();
        private static readonly Timer _checkTimer = new(60 * 5 * 1000);

        public static void Init()
        {
            App.ReadyEvent += Ready;
            
            _checkTimer.Elapsed += OnTimerElapsed;
            _checkTimer.Start();

            Check();

            FireNotices(NoticeTrigger.Startup);
        }

        public static void Ready()
        {
            Game.LoggedChanged += OnLoggedChanged;
            PacketAnalyzer.Sniffer.NewConnection += OnNewConnection;
            FireNotices(NoticeTrigger.Ready);
        }

        private static void OnNewConnection(Server obj)
        {
            FireNotices(NoticeTrigger.Connection);
        }

        private static void OnLoggedChanged()
        {
            if (!Game.Logged) return;
            FireNotices(NoticeTrigger.Login);
        }

        private static void FireNotices(NoticeTrigger trigger)
        {
            App.BaseDispatcher.Invoke(() =>
            {
                var list = _notices.Where(n => n.Enabled && !n.Fired && n.Trigger == trigger).ToList();
                foreach (var n in list)
                {
                    n.Fire();
                }
            });
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Check();
        }

        private static void Check()
        {
            try
            {
                using var c = MiscUtils.GetDefaultWebClient();
                var sMsg = c.DownloadString(new Uri(Url));
                var jMsg = JObject.Parse(sMsg);

                var newNotices = new List<NoticeBase>();

                var jNotices = jMsg["notices"];
                if (jNotices == null) return;
                foreach (var jNotice in jNotices)
                {
                    var jType = jNotice["Type"];
                    if(jType == null) continue;
                    var type = jType.Value<string>();

                    var jDetails = jNotice["Details"];
                    if(jDetails == null) continue;

                    // todo: maybe use a factory
                    var jEnabled = jNotice[nameof(NoticeBase.Enabled)];
                    var jTrigger = jNotice[nameof(NoticeBase.Trigger)];
                    var jTitle = jDetails[nameof(NoticeBase.Title)];
                    var jContent = jDetails[nameof(NoticeBase.Content)];

                    if (jEnabled == null || jTrigger == null || jTitle == null || jContent == null) continue;
                    var notice = new NoticeBase
                    {
                        Enabled = jEnabled.Value<bool>(),
                        Trigger = (NoticeTrigger)jTrigger.Value<int>(),
                        Title = jTitle.Value<string>()!,
                        Content = jContent.Value<string>()!
                    };

                    var duration = jDetails[nameof(NotificationNotice.Duration)]?.Value<int>() ?? 0;
                    var intNotifType = jDetails[nameof(NotificationNotice.NotificationType)]?.Value<int>() ?? 0;
                    var notifType = (NotificationType) intNotifType;

                    notice = type switch
                    {
                        nameof(NotificationNotice) => new NotificationNotice(notice)
                        {
                            Duration = duration,
                            NotificationType = notifType
                        },
                        nameof(MessageBoxNotice) => new MessageBoxNotice(notice),
                        _ => notice
                    };

                    newNotices.Add(notice);
                }

                _notices = _notices.Where(existing => newNotices.All(n => n.Content != existing.Content)).ToList();

                foreach (var newNotice in newNotices)
                {
                    _notices.Add(newNotice);
                }

            }
            catch (Exception e)
            {
                Log.F($"Failed to check for messages. {e}");
            }
        }
    }
}
