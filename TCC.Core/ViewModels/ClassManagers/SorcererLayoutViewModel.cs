using System.Diagnostics;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.ViewModels.ClassManagers;

public class SorcererLayoutViewModel : BaseClassLayoutViewModel
{
    private readonly Stopwatch _sw;
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
            return Elements switch
            {
                (FusionElements.Flame | FusionElements.Frost | FusionElements.Arcane) => FusionSkill,
                (FusionElements.Flame | FusionElements.Frost) => PrimeFlame,
                (FusionElements.Frost | FusionElements.Arcane) => Iceberg,
                (FusionElements.Flame | FusionElements.Arcane) => ArcaneStorm,
                _ => FusionSkill
            };
        }
    }

    private FusionElements _elements;
    public FusionElements Elements
    {
        get => _elements;
        private set => RaiseAndSetIfChanged(value, ref _elements);
    }

    private FusionElements _boosts;
    public FusionElements Boosts
    {
        get => _boosts;
        set => RaiseAndSetIfChanged(value, ref _boosts);
    }

    public SorcererLayoutViewModel()
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

        ManaBoost = new SkillWithEffect(_dispatcher, mb);
        Fusion = new Cooldown(fusion, false);

        _sw = new Stopwatch();

    }

    private void OnBoostChanged(FusionElements elements)
    {
        Boosts = elements;
    }
    public override void Dispose()
    {
        ManaBoost.Dispose();
        Fusion.Dispose();
        SorcererAbnormalityTracker.BoostChanged -= OnBoostChanged;
    }

    protected override bool StartSpecialSkillImpl(Cooldown sk)
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

        if (sk.Skill.IconName != fusion.IconName) return false;

        _latestCooldown = (long)sk.OriginalDuration;
        Fusion.Start(sk.Duration, sk.Mode);
        _sw.Restart();
        return false;

    }

    public void EndFireIcePre()
    {
        _sw.Stop();
        Fusion.Start(_latestCooldown > _sw.ElapsedMilliseconds ? (ulong)(_latestCooldown - _sw.ElapsedMilliseconds) : (ulong)_latestCooldown);
    }

    public void SetElements(FusionElements elements)
    {
        Elements = elements;
        Fusion.Skill = CurrentFusionSkill;
    }
}