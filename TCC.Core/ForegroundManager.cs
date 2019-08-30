using System;
using System.Windows.Threading;
using TCC.Test;

namespace TCC
{
    public class ForegroundManager
    {
        private readonly DispatcherTimer _dimTimer;

        public bool ForceUndim
        {
            get => _forceUndim;
            set
            {
                if (_forceUndim == value) return;
                _forceUndim = value;
                NotifyDimChanged();
            }
        }

        private bool _forceVisible = Tester.Enabled;
        private bool _forceUndim = Tester.Enabled;
        public event Action VisibilityChanged;
        public event Action DimChanged;
        public event Action ClickThruChanged;

        public ForegroundManager()
        {
            Game.LoadingScreenChanged += NotifyVisibilityChanged;
            Game.LoggedChanged += NotifyVisibilityChanged;
            Game.EncounterChanged += NotifyDimChanged;
            Game.GameUiModeChanged += OnGameUiModeChanged;

            FocusManager.ForegroundChanged += NotifyVisibilityChanged;
            Game.SkillStarted += OnSkillStarted;

            _dimTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _dimTimer.Tick += (_, __) =>
            {
                _dimTimer.Stop();
                NotifyDimChanged();
            };
        }

        private void OnGameUiModeChanged()
        {
            App.BaseDispatcher.Invoke(() => ClickThruChanged?.Invoke());
        }

        private void OnSkillStarted()
        {
            _dimTimer.Stop();
            _dimTimer.Start();
            NotifyDimChanged();
        }

        private void NotifyVisibilityChanged()
        {
            //Console.WriteLine($"[Foreground Manager] Notifying Visibility = {Visible} (Logged:{SessionManager.Logged}, LoadingScreen:{SessionManager.LoadingScreen}, Foreground:{FocusManager.IsForeground})");
            App.BaseDispatcher?.BeginInvoke(new Action(() =>
                VisibilityChanged?.Invoke()), DispatcherPriority.Background);
        }

        private void NotifyDimChanged()
        {
            //Console.WriteLine($"[Foreground Manager] Notifying Dim = {Dim} (TimerEnabled:{_dimTimer.IsEnabled}, Encounter:{SessionManager.Encounter})");
            App.BaseDispatcher?.BeginInvoke(new Action(() =>
                DimChanged?.Invoke()), DispatcherPriority.Background);
        }


        public bool Dim => !_dimTimer.IsEnabled &&
                                !Game.Encounter &&
                                !ForceUndim;

        public bool Visible => Game.Logged &&
                               !Game.LoadingScreen &&
                                FocusManager.IsForeground ||
                                _forceVisible;

        public bool ForceVisible
        {
            get => _forceVisible;
            set
            {
                if (_forceVisible == value) return;
                _forceVisible = value;
                NotifyVisibilityChanged();
            }
        }

        public void RefreshDim()
        {
            if (App.Loading) return;
            App.BaseDispatcher?.Invoke(() =>
        {
            ForceUndim = true;
            DimChanged?.Invoke();
            ForceUndim = false;
            DimChanged?.Invoke();
        });
        }

        public void RefreshVisible()
        {
            App.BaseDispatcher?.Invoke(() =>
            {
                _forceVisible = true;
                VisibilityChanged?.Invoke();
                _forceVisible = false;
                VisibilityChanged?.Invoke();
            });
        }
    }
}
