namespace TCC.Data.Databases
{
    public static class ItemSkillsDatabase
    {
        internal static bool TryGetItemSkill(uint itemId, out Skill sk)
        {
            var result = false;
            sk = new Skill(0, Class.None, string.Empty, string.Empty);

            if (ItemsDatabase.Instance.Items.ContainsKey(itemId))
            {
                var item = ItemsDatabase.Instance.Items[itemId];
                result = true;
                sk = new Skill(itemId,Class.Common ,item.Name, "");
                sk.SetSkillIcon(item.Icon);
            }
            return result;

        }
    }
}