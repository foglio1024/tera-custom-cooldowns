using System;

namespace TCC.TeraCommon.Game
{
    public class Skill : IEquatable<object>
    {
        private static readonly string[] Lvls =
        {
            " I", " II", " III", " IV", " V", " VI", " VII", " VIII", " IX", " X",
            " XI", " XII", " XIII", " XIV", " XV", " XVI", " XVII", " XVIII", " XIX", " XX"
        };

        public readonly NpcInfo NpcInfo;

        internal Skill(int id, string name, bool? isChained = null, string detail = "", string iconName = "",
            NpcInfo npcInfo = null, bool isHotDot = false)
        {
            Id = id;
            Name = name;
            ShortName = RemoveLvl(name);
            IsChained = isChained;
            Detail = detail;
            IconName = iconName;
            NpcInfo = npcInfo;
            IsHotDot = isHotDot;
            Boom = detail.Contains("Boom");
        }

        public bool IsHotDot { get; }


        public int Id { get; }
        public string Name { get; private set; }
        public string ShortName { get; private set; }
        public bool? IsChained { get; private set; }
        public bool Boom { get; private set; }
        public string Detail { get; private set; }
        public string IconName { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as Skill;
            if (other == null)
                return false;
            return Id == other.Id && IsHotDot == other.IsHotDot && NpcInfo == other.NpcInfo;
        }

        public static string RemoveLvl(string name)
        {
            foreach (var lvl in Lvls)
            {
                if (name.EndsWith(lvl) || name.Contains(lvl + " "))
                {
                    return name.Replace(lvl, string.Empty);
                }
            }
            return name;
        }

        public override int GetHashCode()
        {
            var npcinfoHash = NpcInfo?.GetHashCode() ?? 0;
            return Id ^ IsHotDot.GetHashCode() ^ npcinfoHash;
        }
    }

    public class UserSkill : Skill
    {
        public UserSkill(int id, PlayerClass playerClass, string name, string hit, bool? ischained, string iconName)
            : base(id, name, ischained, hit, iconName)
        {
            PlayerClass = playerClass;
            RaceGenderClass = new RaceGenderClass(Race.Common, Gender.Common, playerClass);
            Hit = hit;
        }

        public UserSkill(int id, RaceGenderClass raceGenderClass, string name, bool? isChained = null,
            string detail = "", string iconName = "", NpcInfo npcInfo = null)
            : base(id, name, isChained, detail, iconName, npcInfo)
        {
            RaceGenderClass = raceGenderClass;
            PlayerClass = raceGenderClass.Class;
            Hit = detail;
        }

        public string Hit { get; }
        public RaceGenderClass RaceGenderClass { get; }
        public PlayerClass PlayerClass { get; }
    }
}