using Newtonsoft.Json.Linq;
using Nostrum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TCC.Analysis;
using TCC.Data;
using TCC.Utils;
using TeraPacketParser.TeraCommon.Game;

namespace TCC.Notice
{
    public static class NoticeChecker
    {
        private const string Url = "https://raw.githubusercontent.com/Foglio1024/Tera-custom-cooldowns/master/messages.json";
        private static List<NoticeBase> _notices;
        private static Timer _checkTimer;

        public static void Init()
        {
            _notices = new List<NoticeBase>();
            _checkTimer = new Timer(60 * 5 * 1000);
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
                var list = _notices.Where(n => !n.Fired && n.Trigger == trigger).ToList();
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

                foreach (var jNotice in jMsg["notices"])
                {
                    var type = jNotice["Type"].Value<string>();
                    var details = jNotice["Details"];

                    // todo: maybe use a factory
                    var notice = new NoticeBase
                    {
                        Title = details[nameof(NoticeBase.Title)].Value<string>(),
                        Content = details[nameof(NoticeBase.Content)].Value<string>(),
                        Trigger = (NoticeTrigger)jNotice[nameof(NoticeBase.Trigger)].Value<int>()
                    };

                    notice = type switch
                    {
                        nameof(NotificationNotice) => new NotificationNotice(notice)
                        {
                            Duration = details[nameof(NotificationNotice.Duration)].Value<int>(),
                            NotificationType = (NotificationType)details[nameof(NotificationNotice.NotificationType)].Value<int>()
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
