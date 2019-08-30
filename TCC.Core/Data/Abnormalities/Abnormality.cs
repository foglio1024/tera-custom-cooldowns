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
        public bool IsShow { get; set; }
        public bool Infinity { get; set; }
        public AbnormalityType Type { get; set; }
        public uint ShieldSize { get; private set; }
        public bool IsShield { get; private set; }
        public Abnormality(uint id, bool isShow, bool isBuff, bool infinity, AbnormalityType prop)
        {
            Id = id;
            IsBuff = isBuff;
            IsShow = isShow;
            Infinity = infinity;
            Type = prop;
        }

        public void SetIcon(string iconName)
        {
            IconName = iconName;
        }

        public void SetInfo(string name, string toolTip)
        {
            Name = name;
            ToolTip = toolTip;
        }

        public void SetShield(uint size)
        {
            IsShield = true;
            ShieldSize = size;
        }
    }
}
