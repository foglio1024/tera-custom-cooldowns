using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCC.Tera.Data;
using TCC.ViewModels;
using TCC.Windows;

namespace TCC
{
    public sealed class KeyboardHook : IDisposable
    {
        public delegate void TopmostSwitch();

        private static KeyboardHook _instance;
        private readonly Window _window = new Window();
        private int _currentId;

        private bool _isRegistered;
        private bool _isInitialized;
        private KeyboardHook()
        {
            // register the event of the inner native window.
            _window.KeyPressed += delegate (object sender, KeyPressedEventArgs args) { KeyPressed?.Invoke(this, args); };
        }


        public static KeyboardHook Instance => _instance ?? (_instance = new KeyboardHook());
        public event TopmostSwitch SwitchTopMost;

        public bool SetHotkeys(bool value)
        {
            if (value && !_isRegistered)
            {
                Register();
                return true;

            }
            if (!value && _isRegistered) { ClearHotkeys(); return true; }
            return false;
        }

        private static void hook_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.Key == SettingsManager.LfgHotkey.Key && e.Modifier == SettingsManager.LfgHotkey.Modifier)
            {
                if (!Proxy.IsConnected) return;

                if (!WindowManager.LfgListWindow.IsVisible) Proxy.RequestLfgList();
                else WindowManager.LfgListWindow.CloseWindow();
            }
            if (e.Key == SettingsManager.SettingsHotkey.Key && e.Modifier == SettingsManager.SettingsHotkey.Modifier)
            {
                if (WindowManager.Settings.IsVisible) WindowManager.Settings.HideWindow();
                else WindowManager.Settings.ShowWindow();
            }
            if (e.Key == SettingsManager.InfoWindowHotkey.Key && e.Modifier == SettingsManager.InfoWindowHotkey.Modifier)
            {
                if (WindowManager.InfoWindow.IsVisible) InfoWindowViewModel.Instance.ShowWindow();
                else WindowManager.InfoWindow.HideWindow();
            }
            if (e.Key == SettingsManager.ShowAllHotkey.Key && e.Modifier == SettingsManager.ShowAllHotkey.Modifier)
            {
                WindowManager.TempShowAll();
            }
            if (e.Key == SettingsManager.LootSettingsHotkey.Key && e.Modifier == SettingsManager.LootSettingsHotkey.Modifier)
            {
                if (!GroupWindowViewModel.Instance.AmILeader) return;
                if (!Proxy.IsConnected) return;
                Proxy.LootSettings();
            }
            
        }


        public void Update()
        {
            ClearHotkeys();
            Register();
        }

        public void RegisterKeyboardHook()
        {
            if (_isInitialized) return;
            WindowManager.FloatingButton.Dispatcher.Invoke(() =>
            {
                // register the event that is fired after the key press.
                Instance.KeyPressed += hook_KeyPressed;
                SessionManager.ChatModeChanged += CheckHotkeys;
                FocusManager.ForegroundChanged += CheckHotkeys;
                _isInitialized = true;
            });
        }
        public void UnRegisterKeyboardHook()
        {
            if (!_isInitialized) return;
            WindowManager.FloatingButton.Dispatcher.Invoke(() =>
            {
                // register the event that is fired after the key press.
                Instance.KeyPressed -= hook_KeyPressed;
                if (_isRegistered) { ClearHotkeys(); }
                SessionManager.ChatModeChanged -= CheckHotkeys;
                FocusManager.ForegroundChanged -= CheckHotkeys;
                _isInitialized = false;
            });
        }
        private void CheckHotkeys()
        {
            WindowManager.FloatingButton.Dispatcher.Invoke(() =>
            {
                SetHotkeys(!SessionManager.InGameChatOpen && FocusManager.IsActive);
            });
        }

        private void Register()
        {
            RegisterHotKey(SettingsManager.LfgHotkey.Modifier, SettingsManager.LfgHotkey.Key);
            RegisterHotKey(SettingsManager.InfoWindowHotkey.Modifier, SettingsManager.InfoWindowHotkey.Key);
            RegisterHotKey(SettingsManager.SettingsHotkey.Modifier, SettingsManager.SettingsHotkey.Key);
            RegisterHotKey(SettingsManager.LootSettingsHotkey.Modifier, SettingsManager.LootSettingsHotkey.Key);
            //RegisterHotKey(SettingsManager.ShowAllHotkey.Modifier, SettingsManager.ShowAllHotkey.Key);

            _isRegistered = true;
        }

        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        ///     Registers a hot key in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void RegisterHotKey(HotkeysData.ModifierKeys modifier, Keys key)
        {
            if (key == Keys.None)
            {
                return; //allow disable hotkeys using "None" key
            }
            // increment the counter.
            _currentId++;

            // register the hot key.
            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
            {
            }
        }

        /// <summary>
        ///     A hot key has been pressed.
        /// </summary>
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        /// <summary>
        ///     Represents the window that is used internally to get the messages.
        /// </summary>
        private sealed class Window : NativeWindow, IDisposable
        {
            private static readonly int WmHotkey = 0x0312;

            public Window()
            {
                // create the handle for the window.
                CreateHandle(new CreateParams());
            }

            #region IDisposable Members

            public void Dispose()
            {
                DestroyHandle();
            }

            #endregion

            /// <summary>
            ///     Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                // check if we got a hot key pressed.
                if (m.Msg == WmHotkey)
                {
                    // get the keys.
                    var key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    var modifier = (HotkeysData.ModifierKeys)((int)m.LParam & 0xFFFF);

                    // invoke the event to notify the parent.
                    KeyPressed?.Invoke(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;
        }

        #region IDisposable Members

        public void Dispose()
        {
            // unregister all the registered hot keys.
            ClearHotkeys();

            // dispose the inner native window.
            _window.Dispose();
        }

        private void ClearHotkeys()
        {
            for (var i = _currentId; i > 0; i--) { var v = UnregisterHotKey(_window.Handle, i); }
            _currentId = 0;
            _isRegistered = false;
        }

        #endregion
    }

    /// <summary>
    ///     Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class KeyPressedEventArgs : EventArgs
    {
        internal KeyPressedEventArgs(HotkeysData.ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public HotkeysData.ModifierKeys Modifier { get; }

        public Keys Key { get; }
    }

}
