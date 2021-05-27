using System.Diagnostics;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels.ClassManagers
{
    public class SorcererLayoutVM : BaseClassLayoutVM
    {
        private Stopwatch _sw;
        private long _latestCooldown;

        public SkillWithEffect ManaBoost { get; set; }

        public Cooldown Fusion { get; set; }
        public Skill PrimeFlame { get; set; }
        public Skill Iceberg { get; set; }
        public Skill ArcaneStorm { get; set; }
        public Skill FusionSkill { get; set; }
        public Skill FusionSkillBoost { get; set; }

        private Skill CurrentFusionSkill
        {
            get
            {
                if (Fire && Ice && Arcane) return FusionSkill;
                if (Fire && Ice) return PrimeFlame;
                if (Ice && Arcane) return Iceberg;
                if (Fire && Arcane) return ArcaneStorm;
                return FusionSkill;
            }
        }

        public bool Fire => Game.Me.Fire;
        public bool Ice => Game.Me.Ice;
        public bool Arcane => Game.Me.Arcane;

        public bool IsBoostFire => Game.Me.FireBoost;
        public bool IsBoostFrost => Game.Me.IceBoost;
        public bool IsBoostArcane => Game.Me.ArcaneBoost;



        public SorcererLayoutVM()
        {
            SorcererAbnormalityTracker.BoostChanged += OnBoostChanged;

            Game.DB!.SkillsDatabase.TryGetSkill(340200, Class.Sorcerer, out var mb);
            Game.DB.SkillsDatabase.TryGetSkill(360100, Class.Sorcerer, out var fusion);
            Game.DB.SkillsDatabase.TryGetSkill(360600, Class.Sorcerer, out var fusionBoost);
            Game.DB.SkillsDatabase.TryGetSkill(360200, Class.Sorcerer, out var primeFlame);
            Game.DB.SkillsDatabase.TryGetSkill(360400, Class.Sorcerer, out var iceberg);
            Game.DB.SkillsDatabase.TryGetSkill(360300, Class.Sorcerer, out var arcaneStorm);

            PrimeFlame = primeFlame; //fire ice
            Iceberg = iceberg; //ice arcane
            ArcaneStorm = arcaneStorm; //fire arcane
            FusionSkill = fusion;
            FusionSkillBoost = fusionBoost;

            ManaBoost = new SkillWithEffect(Dispatcher, mb);
            Fusion = new Cooldown(fusion, false);

            _sw = new Stopwatch();

        }

        private void OnBoostChanged()
        {
            NotifyElementBoostChanged();
        }
        public override void Dispose()
        {
            ManaBoost.Dispose();
            SorcererAbnormalityTracker.BoostChanged -= OnBoostChanged;
        }


        public override bool StartSpecialSkill(Cooldown sk)
        {
            if (sk.Skill.IconName == ManaBoost.Cooldown.Skill.IconName)
            {
                ManaBoost.StartCooldown(sk.Duration);
                return true;
            }

            if (sk.Skill.IconName == PrimeFlame.IconName)
            {
                Fusion.Skill = PrimeFlame;
                Fusion.Start(sk.Duration, sk.Mode);
                if (sk.Mode != CooldownMode.Normal) return true;
                _latestCooldown = (long)sk.OriginalDuration;
                _sw.Restart();

                return true;
            }

            var fusion = ManaBoost.Effect.IsAvailable ? FusionSkill : FusionSkillBoost;

            if (sk.Skill.IconName == fusion.IconName)
            {
                _latestCooldown = (long)sk.OriginalDuration;
                Fusion.Start(sk.Duration, sk.Mode);
                _sw.Restart();
                return false;
            }

            return false;
        }

        public void EndFireIcePre()
        {
            _sw.Stop();
            Fusion.Start(_latestCooldown > _sw.ElapsedMilliseconds ? (ulong)(_latestCooldown - _sw.ElapsedMilliseconds) : (ulong)_latestCooldown);
        }

        public void NotifyElementChanged()
        {
            N(nameof(Fire));
            N(nameof(Ice));
            N(nameof(Arcane));
            Fusion.Skill = CurrentFusionSkill;
        }

        public void NotifyElementBoostChanged()
        {
            N(nameof(IsBoostFire));
            N(nameof(IsBoostFrost));
            N(nameof(IsBoostArcane));
        }
    }
}
