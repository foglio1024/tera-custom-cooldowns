using Nostrum.WPF.ThreadSafe;

namespace TCC.Data;

public class Dungeon : ThreadSafeObservableObject
{
    string _shortName = "";
    bool _doublesOnElite;
    bool _show;
    int _index = -1;
    short _maxBaseRuns = 1;
    int _itemLevel;
    //ItemLevelTier _requiredIlvl = ItemLevelTier.Tier0;

    public uint Id { get; }
    public string Name { get; }
    public int Cost { get; set; }
    public ResetMode ResetMode { get; set; } = ResetMode.Weekly;
    public string ShortName
    {
        get => _shortName;
        set
        {
            if (_shortName == value) return;
            _shortName = value;
            N();
        }
    }
    public short MaxBaseRuns
    {
        get => _maxBaseRuns;
        set
        {
            if (_maxBaseRuns == value) return;
            _maxBaseRuns = value;
            N();
        }
    }
    // TODO: get this from DC
    public int ItemLevel
    {
        get => _itemLevel;
        set
        {
            if (_itemLevel == value) return;
            _itemLevel = value;
            N();
        }
    }
    public bool Show
    {
        get => _show;
        set
        {
            if (_show == value) return;
            _show = value;
            N();
        }
    }
    public bool DoublesOnElite
    {
        get => _doublesOnElite;
        set
        {
            if (_doublesOnElite == value) return;
            _doublesOnElite = value;
            N();
        }
    }
    public int Index
    {
        get => _index;
        set
        {
            if (_index == value) return;
            _index = value;
            N();
        }
    }
    public int MaxEntries => MaxBaseRuns * (Game.Account.IsElite && DoublesOnElite ? 2 : 1);
    public string IconName { get; set; } = "";
    public string Region => Game.DB!.GetDungeonGuardName(Id);
    public bool HasDef { get; set; }
    //public ItemLevelTier RequiredIlvl
    //{
    //    get => _requiredIlvl;
    //    set
    //    {
    //        if (_requiredIlvl == value) return;
    //        _requiredIlvl = value;
    //        N();
    //        N(nameof(ItemLevel));
    //    }
    //}

    public Dungeon(uint id, string name)
    {
        Id = id;
        Name = name;
    }
}

public enum ResetMode
{
    Daily,
    Weekly
}