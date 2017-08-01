using System;
using System.Timers;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC.Data
{
    public class LFG : TSPropertyChanged
    {
        int _id;
        string _name;
        string _message;
        bool _raid;
        string _dungeonName;
        ushort _membersCount;
        Timer _removeTimer;

        public int Id
        {
            get => _id; set
            {
                if (_id == value) return;
                _id = value;
                NotifyPropertyChanged(nameof(Id));
            }
        }
        public string Name
        {
            get => _name; set
            {
                if (_name == value) return;
                _name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }
        public string Message
        {
            get => _message; set
            {
                if (_message == value) return;
                _message = value;
                UpdateDungeonName();
                NotifyPropertyChanged(nameof(Message));
            }
        }
        public bool Raid
        {
            get => _raid; set
            {
                if (_raid == value) return;
                _raid = value;
                NotifyPropertyChanged(nameof(Raid));
            }
        }

        public string DungeonName
        {
            get => _dungeonName; set
            {
                if (_dungeonName == value) return;
                _dungeonName = value;
                NotifyPropertyChanged(nameof(DungeonName));
            }
        }

        public ushort MembersCount
        {
            get => _membersCount; set
            {
                if (_membersCount == value) return;
                _membersCount = value;
                NotifyPropertyChanged(nameof(MembersCount));
                NotifyPropertyChanged(nameof(MembersCountLabel));
            }
        }
        public string MembersCountLabel
        {
            get { return MembersCount == 0 ? "" : MembersCount.ToString(); }
        }
        public LFG(int id, string name, string msg, bool raid)
        {
            _dispatcher = WindowManager.ChatWindow.Dispatcher;

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
            ChatWindowViewModel.Instance.RemoveLfg(this);
        }
        public void Refresh()
        {
            try
            {
                _removeTimer?.Stop();
                _removeTimer?.Start();
                NotifyPropertyChanged("Refresh");
            }
            catch (Exception) { }
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
            return string.Format("[{0}] {1}: {2}", Id, Name, Message);
        }
    }
}