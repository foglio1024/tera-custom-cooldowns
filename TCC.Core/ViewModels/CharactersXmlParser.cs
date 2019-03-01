using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TCC.Data.Abnormalities;
using TCC.Data.Map;
using TCC.Data.Pc;

namespace TCC.Data
{
    public class CharactersXmlParser
    {
        private const string CharactersTag = "Characters";
        private const string CharacterTag = "Character";
        private const string NameTag = "name";
        private const string IdTag = "id";
        private const string PosTag = "pos";
        private const string VanguardCreditsTag = "vanguardCredits";
        private const string GuardianCreditsTag = "guardianCredits";
        private const string VanguardWeeklyTag = "vanguardWeekly";
        private const string VanguardDailyTag = "vanguardDaily";
        private const string GuardianQuestsTag = "guardianQuests";
        private const string ElleonMarksTag = "elleonMarks";
        private const string DragonwingScalesTag = "dragonwing";
        private const string PiecesOfDragonScrollTag = "scrollPieces";
        private const string ClassTag = "class";
        private const string LevelTag = "level";
        private const string ItemLevelTag = "ilvl";
        private const string DungeonTag = "Dungeon";
        private const string DungeonsTag = "Dungeons";
        private const string EntriesTag = "entries";
        private const string TotalTag = "total";
        private const string GearTag = "Gear";
        private const string GearPiecesTag = "GearPieces";
        private const string PieceTag = "piece";
        private const string TierTag = "tier";
        private const string EnchantTag = "enchant";
        private const string ExpTag = "exp";
        private const string EliteTag = "elite";
        private const string LastOnlineTag = "lastOnline";
        private const string LastLocationTag = "lastLocation";
        private const string GuildNameTag = "guildName";
        private const string BuffTag = "Buff";
        private const string BuffsTag = "Buffs";
        private const string StacksTag = "stacks";
        private const string DurationTag = "duration";
        private const string InventoryTag = "Inventory";
        private const string ItemTag = "Item";
        private const string AmountTag = "amount";
        private const string ServerTag = "server";
        private const string SlotTag = "slot";
        private const string HiddenTag = "hidden";


        private readonly string _path = Path.Combine(App.BasePath, "resources/config/characters.xml");
        private XDocument _doc;

        public static XDocument BuildCharacterFile(SynchronizedObservableCollection<Character> list)
        {
            var root = new XElement(CharactersTag, new XAttribute(EliteTag, SessionManager.IsElite));
            list.ToSyncList().ForEach(c =>
            {
                var xChar = BuildGeneralDataXelement(c);
                xChar.Add(BuildDungeonDataXelement(c));
                xChar.Add(BuildGearDataXelement(c));
                xChar.Add(BuildBuffsXelement(c));
                xChar.Add(BuildInventoryDataXelement(c));
                root.Add(xChar);
            });

            return new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
        }

        private static XElement BuildBuffsXelement(Character character)
        {
            var xRet = new XElement(BuffsTag);
            character.Buffs.ToList().ForEach(b =>
            {
                xRet.Add(new XElement(BuffTag,
                            new XAttribute(IdTag, b.Id),
                            new XAttribute(DurationTag, b.Duration),
                            new XAttribute(StacksTag, b.Stacks)
                    ));
            });
            return xRet;
        }
        private static XElement BuildGearDataXelement(Character c)
        {
            var xGear = new XElement(GearPiecesTag);
            c.Gear.ToSyncList().ForEach(item =>
            {
                xGear.Add(new XElement(GearTag,
                    new XAttribute(IdTag, item.Id),
                    new XAttribute(PieceTag, item.Piece),
                    new XAttribute(TierTag, item.Tier),
                    new XAttribute(ExpTag, item.Experience),
                    new XAttribute(EnchantTag, item.Enchant)));
            });
            return xGear;
        }
        private static XElement BuildGeneralDataXelement(Character c)
        {
            return new XElement(CharacterTag,
                new XAttribute(NameTag, c.Name),
                new XAttribute(IdTag, c.Id),
                new XAttribute(ClassTag, c.Class),
                new XAttribute(PosTag, c.Position),
                new XAttribute(LastOnlineTag, c.LastOnline),
                new XAttribute(LastLocationTag, c.LastLocation == null ? "0_0_0": $"{c.LastLocation.World}_{c.LastLocation.Guard}_{c.LastLocation.Section}"),
                new XAttribute(VanguardCreditsTag, c.VanguardCredits),
                new XAttribute(GuildNameTag, c.GuildName),
                new XAttribute(ServerTag, c.ServerName),
                new XAttribute(GuardianCreditsTag, c.GuardianCredits),
                new XAttribute(VanguardWeeklyTag, c.VanguardWeekliesDone),
                new XAttribute(VanguardDailyTag, c.VanguardDailiesDone),
                new XAttribute(GuardianQuestsTag, c.ClaimedGuardianQuests),
                new XAttribute(ElleonMarksTag, c.ElleonMarks),
                new XAttribute(DragonwingScalesTag, c.DragonwingScales),
                new XAttribute(LevelTag, c.Level),
                new XAttribute(ItemLevelTag, c.ItemLevel),
                new XAttribute(HiddenTag, c.Hidden),
                new XAttribute(PiecesOfDragonScrollTag, c.PiecesOfDragonScroll)
            );
        }
        private static XElement BuildDungeonDataXelement(Character c)
        {
            var xDungeons = new XElement(DungeonsTag);
            c.Dungeons.ToSyncList().ForEach(dungCd =>
            {
                xDungeons.Add(new XElement(DungeonTag,
                    new XAttribute(IdTag, dungCd.Dungeon.Id),
                    new XAttribute(EntriesTag, dungCd.Entries),
                    new XAttribute(TotalTag, dungCd.Clears)));
            });

            return xDungeons;
        }
        private static XElement BuildInventoryDataXelement(Character c)
        {
            var xRet = new XElement(InventoryTag);
            c.Inventory.ToList().ForEach(item =>
            {
                xRet.Add(new XElement(ItemTag,
                            new XAttribute(SlotTag, item.Slot),
                            new XAttribute(IdTag, item.Item.Id),
                            new XAttribute(AmountTag, item.Amount)));
            });

            return xRet;
        }
        private static void ParseGeneralCharInfo(XElement xChar, Character ch)
        {
            xChar.Attributes().ToList().ForEach(attr =>
            {
                if (attr.Name == NameTag) ch.Name = attr.Value;
                else if (attr.Name == IdTag) ch.Id = Convert.ToUInt32(attr.Value);
                else if (attr.Name == PosTag) ch.Position = Convert.ToInt32(attr.Value);
                else if (attr.Name == VanguardCreditsTag) ch.VanguardCredits = Convert.ToInt32(attr.Value);
                else if (attr.Name == GuardianCreditsTag) ch.GuardianCredits = Convert.ToInt32(attr.Value);
                else if (attr.Name == VanguardWeeklyTag) ch.VanguardWeekliesDone = Convert.ToInt32(attr.Value);
                else if (attr.Name == VanguardDailyTag) ch.VanguardDailiesDone = Convert.ToInt32(attr.Value);
                else if (attr.Name == GuardianQuestsTag) ch.ClaimedGuardianQuests = Convert.ToInt32(attr.Value);
                else if (attr.Name == ElleonMarksTag) ch.ElleonMarks = Convert.ToInt32(attr.Value);
                else if (attr.Name == DragonwingScalesTag) ch.DragonwingScales = Convert.ToInt32(attr.Value);
                else if (attr.Name == PiecesOfDragonScrollTag) ch.PiecesOfDragonScroll = Convert.ToInt32(attr.Value);
                else if (attr.Name == ClassTag) ch.Class = (Class)Enum.Parse(typeof(Class), attr.Value);
                else if (attr.Name == LevelTag) ch.Level = Convert.ToInt32(attr.Value);
                else if (attr.Name == ItemLevelTag) ch.ItemLevel = Convert.ToInt32(attr.Value);
                else if (attr.Name == LastOnlineTag) ch.LastOnline = Convert.ToInt64(attr.Value);
                else if (attr.Name == LastLocationTag) ch.LastLocation = new Location(attr.Value);
                else if (attr.Name == GuildNameTag) ch.GuildName = attr.Value;
                else if (attr.Name == ServerTag) ch.ServerName = attr.Value;
                else if (attr.Name == HiddenTag) ch.Hidden = bool.Parse(attr.Value);
            });
        }
        private static void ParseDungeonCharInfo(XElement xChar, Character ch)
        {
            var dungeons = new Dictionary<uint, short>();
            xChar.Descendants().Where(x => x.Name == DungeonTag).ToList().ForEach(xDung =>
            {
                uint id = 0;
                short entries = 0;
                var total = 0;

                xDung.Attributes().ToList().ForEach(attr =>
                {
                    if (attr.Name == IdTag) id = Convert.ToUInt32(attr.Value);
                    else if (attr.Name == EntriesTag) entries = Convert.ToInt16(attr.Value);
                    else if (attr.Name == TotalTag) total = Convert.ToInt16(attr.Value);
                });
                ch.SetDungeonClears(id, total);
                dungeons.Add(id, entries);
            });

            ch.UpdateDungeons(dungeons);
        }
        private static void ParseGearCharInfo(XElement xChar, Character ch)
        {
            var gear = new List<GearItem>();
            xChar.Descendants().Where(x => x.Name == GearTag).ToList().ForEach(xPiece =>
            {
                uint id = 0;
                var type = GearPiece.Weapon;
                var tier = GearTier.Low;
                var enchant = 0;
                uint exp = 0;

                xPiece.Attributes().ToList().ForEach(attr =>
                {
                    if (attr.Name == IdTag) id = Convert.ToUInt32(attr.Value);
                    if (attr.Name == PieceTag) type = (GearPiece)Enum.Parse(typeof(GearPiece), attr.Value);
                    if (attr.Name == TierTag) tier = (GearTier)Enum.Parse(typeof(GearTier), attr.Value);
                    if (attr.Name == EnchantTag) enchant = Convert.ToInt32(attr.Value);
                    if (attr.Name == ExpTag) exp = Convert.ToUInt32(attr.Value);
                });
                gear.Add(new GearItem(id, tier, type, enchant, exp));
            });
            ch.UpdateGear(gear);
        }
        private static void ParseBuffsInfo(XElement xChar, Character ch)
        {
            xChar.Descendants().Where(x => x.Name == BuffTag).ToList().ForEach(xBuff =>
            {
                var id = 0U;
                ulong duration = 0;
                var stacks = 0;

                xBuff.Attributes().ToList().ForEach(a =>
                {
                    if (a.Name == IdTag) id = uint.Parse(a.Value);
                    if (a.Name == DurationTag) duration = ulong.Parse(a.Value);
                    if (a.Name == StacksTag) stacks = int.Parse(a.Value);
                });
                ch.Buffs.Add(new AbnormalityData { Id = id, Duration = duration, Stacks = stacks });
            });

        }

        private static void ParseInventoryInfo(XElement xChar, Character ch)
        {
            xChar.Descendants().Where(x => x.Name == ItemTag).ToList().ForEach(xItem =>
            {
                var slot = 0U;
                var id = 0U;
                var amount = 0;

                xItem.Attributes().ToList().ForEach(a =>
                {
                    if (a.Name == SlotTag) slot = uint.Parse(a.Value);
                    if (a.Name == IdTag) id = uint.Parse(a.Value);
                    if (a.Name == AmountTag) amount = int.Parse(a.Value);
                });
                ch.Inventory.Add(new InventoryItem(slot,id,amount));
            });
        }
        public void Read(SynchronizedObservableCollection<Character> dest)
        {
            if (File.Exists(_path)) _doc = XDocument.Load(_path);
            if (_doc == null) return;

            ParseEliteStatus();

            _doc.Descendants().Where(x => x.Name == CharacterTag).ToList().ForEach(xChar =>
            {
                var ch = new Character();
                ParseGeneralCharInfo(xChar, ch);
                ParseDungeonCharInfo(xChar, ch);
                ParseGearCharInfo(xChar, ch);
                ParseBuffsInfo(xChar, ch);
                ParseInventoryInfo(xChar, ch);
                dest.Add(ch);
            });
        }

        private void ParseEliteStatus()
        {
            SessionManager.IsElite = bool.Parse(_doc.Descendants().FirstOrDefault(x => x.Name == CharactersTag)?.Attribute(EliteTag)?.Value);
        }
    }
}

