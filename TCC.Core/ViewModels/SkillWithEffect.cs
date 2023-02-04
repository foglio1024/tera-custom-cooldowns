using System;
using System.Windows.Threading;
using Nostrum;
using Nostrum.WPF.ThreadSafe;
using TCC.Data;
using TCC.Data.Skills;

namespace TCC.ViewModels
{
    public class SkillWithEffect : ThreadSafeObservableObject, IDisposable
    {
        public Cooldown Cooldown
        {
            get;
        }

        public Cooldown Effect
        {
            get;
        }


        public SkillWithEffect(Dispatcher d, Skill sk, bool canFlashOverride = true) : base(d)
        {
            Cooldown = new Cooldown(sk, canFlashOverride) { CanFlash = canFlashOverride };
            Effect = new Cooldown(sk, false);
        }

        public void StartEffect(ulong duration)
        {
            Effect.Start(duration);
        }
        public void RefreshEffect(ulong duration)
        {
            Effect.Refresh(duration, CooldownMode.Normal);
        }
        public void StopEffect()
        {
            Effect.Stop();
        }

        public void StartCooldown(ulong duration)
        {
            Cooldown.Start(duration);
        }
        public void RefreshCooldown(ulong duration, uint id = 0)
        {
            var skId = id != 0 ? id : Cooldown.Skill.Id;
            Cooldown.Refresh(skId, duration, CooldownMode.Normal);
        }
        public void StopCooldown()
        {
            Cooldown.Stop();
        }
        public void Dispose()
        {
            Cooldown.Dispose();
            Effect.Dispose();
        }
    }
}