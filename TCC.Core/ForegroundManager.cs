using System;
using System.Timers;
using System.Windows.Threading;

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
                if(_forceUndim == value) return;
                _forceUndim = value;
                NotifyDimChanged();
            }
        }

        private bool _forceVisible = false;
        private bool _forceUndim = false;
        public event Action VisibilityChanged;
        public event Action DimChanged;
        public event Action ClickThruChanged;

        public ForegroundManager()
        {
            SessionManager.LoadingScreenChanged += NotifyVisibilityChanged;
            SessionManager.LoggedChanged += NotifyVisibilityChanged;
            SessionManager.EncounterChanged += NotifyDimChanged;
            SessionManager.GameUiModeChanged += OnGameUiModeChanged;

            FocusManager.ForegroundChanged += NotifyVisibilityChanged;
            SkillManager.SkillStarted += OnSkillStarted;

            _dimTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(3)};
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
            App.BaseDispatcher?.Invoke(() =>
                VisibilityChanged?.Invoke());
        }

        private void NotifyDimChanged()
        {
            Console.WriteLine($"[Foreground Manager] Notifying Dim = {Dim} (TimerEnabled:{_dimTimer.IsEnabled}, Encounter:{SessionManager.Encounter})");
            App.BaseDispatcher?.Invoke(() =>
                DimChanged?.Invoke());
        }

        public bool Dim => !_dimTimer      .IsEnabled && 
                            !SessionManager.Encounter &&
                            !ForceUndim;

        public bool Visible => SessionManager.Logged &&
                               !SessionManager.LoadingScreen &&
                                FocusManager  .IsForeground ||
                                _forceVisible;

        public bool ForceVisible
        {
            get => _forceVisible;
            set
            {
                if(_forceVisible == value) return;
                _forceVisible = value;
                NotifyVisibilityChanged();
            }
        }

        public void RefreshDim()
        {
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
