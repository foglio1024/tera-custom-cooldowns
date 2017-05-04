using System;
using TCC.Data;

namespace TCC.ViewModels
{
    public class CharacterWindowViewModel : BaseINPC
    {
        public Player Player
        {
            get => SessionManager.CurrentPlayer;
        }

        public string STname
        {
            get
            {
                switch (Player.Class)
                {
                    case Class.Warrior:
                        return "RE";
                    case Class.Lancer:
                        return "RE";
                    case Class.Engineer:
                        return "WP";
                    case Class.Fighter:
                        return "RG";
                    case Class.Assassin:
                        return "CH";
                    case Class.Glaiver:
                        return "RG";
                    default:
                        return "";
                }
            }
        }
        public bool IsTeraOnTop
        {
            get => WindowManager.IsTccVisible;
        }
        private void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Class")
            {
                RaisePropertyChanged("STname");
            }
        }

        private bool topMost;
        public bool TopMost
        {
            get => topMost;
            set
            {
                if (topMost != value)
                {
                    topMost = value;
                    RaisePropertyChanged("TopMost");
                }
            }
        }


        public CharacterWindowViewModel()
        {
            Player.PropertyChanged += Player_PropertyChanged;
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                RaisePropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    TopMost = false;
                    TopMost = true;
                }
            };
        }
    }

}

