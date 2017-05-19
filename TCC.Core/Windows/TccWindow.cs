using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace TCC.Windows
{
    public class TccWindow : Window
    {
        protected IntPtr _handle;

        private bool clickThru;

        public bool ClickThru
        {
            get { return clickThru; }
            set {
                if (clickThru == value) return;
                clickThru = value;

                if (clickThru) FocusManager.MakeTransparent(_handle);
                else FocusManager.UndoTransparent(_handle);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ClickThru"));
            }
        }


        protected void InitWindow()
        {
            _handle = new WindowInteropHelper(this).Handle;
            FocusManager.MakeUnfocusable(_handle);
            FocusManager.HideFromToolBar(_handle);
            Topmost = true;
            ContextMenu = new ContextMenu();

            var HideButton = new MenuItem() { Header = "Hide" };
            HideButton.Click += (s, ev) =>
            {
                SetVisibility(Visibility.Hidden);
            };
            ContextMenu.Items.Add(HideButton);

            var ClickThruButton = new MenuItem() { Header = "Click through" };
            ClickThruButton.Click += (s, ev) =>
            {
                SetClickThru(true);
            };
            ContextMenu.Items.Add(ClickThruButton);

        }

        public void SetClickThru(bool t)
        {
            ClickThru = t;
        }

        public void SetVisibility(Visibility v)
        {
            this.Dispatcher.Invoke(() =>
            {
                Visibility = v;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Visibility"));
            });
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
