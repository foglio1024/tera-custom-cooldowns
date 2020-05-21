namespace TCC.Data.Abnormalities
{
    public class Abnormality
    {
        public string IconName { get; set; }
        public uint Id { get; set; }
        public string Name { get; set; }
        public string ToolTip { get; set; }
        public bool IsBuff { get; set; }
        public bool IsDebuff => Type == AbnormalityType.DOT ||
                               Type == AbnormalityType.Stun ||
                               Type == AbnormalityType.Debuff;
        public bool IsShow { get; }
        public bool Infinity { get; set; }
        public AbnormalityType Type { get; set; }
        public uint ShieldSize { get; private set; }
        public bool IsShield { get; private set; }

        public bool CanShow => IsShow
                               && !ToolTip.Contains("BTS")
                               && !Name.Contains("BTS")
                               && !Name.Contains("(Hidden)")
                               && !Name.Equals("Unknown")
                               && !Name.Equals(string.Empty);


        public Abnormality(uint id, bool isShow, bool isBuff, bool infinity, AbnormalityType prop, string iconName, string name, string tooltip)
        {
            Id = id;
            IsBuff = isBuff;
            IsShow = isShow;
            Infinity = infinity;
            Type = prop;
            IconName = iconName;
            Name = name;
            ToolTip = tooltip;
            ShieldSize = 0;
            IsShield = false;
        }


        public void SetShield(double size)
        {
            IsShield = true;
            ShieldSize = (uint)size;
        }

        public override string ToString()
        {
            return $"{Id} | {Name}";
        }
    }
}
