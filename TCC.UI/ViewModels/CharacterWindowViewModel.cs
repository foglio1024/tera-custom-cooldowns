using System;
using TCC.Data;

namespace TCC.ViewModels
{
    public class CharacterWindowViewModel : BaseINPC
    {

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


        private double scale = SettingsManager.CharacterWindowSettings.Scale;
        public double Scale
        {
            get { return scale; }
            set
            {
                if (scale == value) return;
                scale = value;
                RaisePropertyChanged("Scale");
            }
        }
        public CharacterWindowViewModel()
        {
            WindowManager.TccVisibilityChanged += (s, ev) =>
            {
                RaisePropertyChanged("IsTeraOnTop");
                if (IsTeraOnTop)
                {
                    CharacterWindowManager.Instance.Dispatcher.Invoke(() =>
                    {
                        WindowManager.CharacterWindow.Topmost = false;
                        WindowManager.CharacterWindow.Topmost = true;
                    });
                }
            };
        }
    }

}

