using System.Timers;
using TCC.ViewModels;

namespace TCC.Data.Chat
{
    public class LFG : TSPropertyChanged
    {
        private uint _id;
        private string _name;
        private string _message;
        private bool _raid;
        private string _dungeonName;
        private int _membersCount;
        private Timer _removeTimer;

        public uint Id
        {
            get => _id; set
            {
                if (_id == value) return;
                _id = value;
                NPC(nameof(Id));
            }
        }
        public string Name
        {
            get => _name; set
            {
                if (_name == value) return;
                _name = value;
                NPC(nameof(Name));
            }
        }
        public string Message
        {
            get => _message; set
            {
                if (_message == value) return;
                _message = value;
                UpdateDungeonName();
                NPC(nameof(Message));
            }
        }
        public bool Raid
        {
            get => _raid; set
            {
                if (_raid == value) return;
                _raid = value;
                NPC(nameof(Raid));
            }
        }

        public string DungeonName
        {
            get => _dungeonName; set
            {
                if (_dungeonName == value) return;
                _dungeonName = value;
                NPC(nameof(DungeonName));
            }
        }

        public int MembersCount
        {
            get => _membersCount; set
            {
                if (_membersCount == value) return;
                _membersCount = value;
                NPC(nameof(MembersCount));
                NPC(nameof(MembersCountLabel));
            }
        }
        public string MembersCountLabel => MembersCount == 0 ? "" : MembersCount.ToString();

        public LFG(uint id, string name, string msg, bool raid)
        {
            Dispatcher = ChatWindowManager.Instance.GetDispatcher();

            Id = id;
            Name = name;
            Message = msg;
            Raid = raid;
            MembersCount = 0;
            UpdateDungeonName();

            _removeTimer = new Timer(3 * 60 * 1000);
            _removeTimer.Elapsed += _removeTimer_Elapsed;
            _removeTimer.Start();

        }

        private void _removeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ChatWindowManager.Instance.RemoveLfg(this);
        }
        public void Refresh()
        {
            try
            {
                _removeTimer?.Stop();
                _removeTimer?.Start();
                NPC();
            }
            catch
            {
                // ignored
            }
        }
        private void UpdateDungeonName()
        {
            var a = Message.Split(' ');
            if (a[0].Length <= 4)
            {
                DungeonName = a[0];
            }
            else
            {
                DungeonName = "LFG";
            }
        }
        public void Dispose()
        {
            _removeTimer.Stop();
            _removeTimer.Dispose();
        }
        public override string ToString()
        {
            return $"[{Id}] {Name}: {Message}";
        }
    }
}