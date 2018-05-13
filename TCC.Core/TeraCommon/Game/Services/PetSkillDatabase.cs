using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TCC.TeraCommon.Game.Services
{
    // Contains information about skills
    // Currently this is limited to the name of the skill
    public class PetSkillDatabase
    {
        private readonly Dictionary<Tuple<ushort,uint>, List<UserSkill>> _petSkilldata = new Dictionary<Tuple<ushort, uint>, List<UserSkill>>();

        public PetSkillDatabase(string folder, string language, NpcDatabase npcDatabase)
        {
            StreamReader reader;
            try
            {
                reader = new StreamReader(File.OpenRead(Path.Combine(folder, $"skills\\pets-skills-{language}.tsv")));
            }
            catch
            {
                return;
            }
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null) continue;
                var values = line.Split('\t');

                var huntingZoneId = ushort.Parse(values[0]);
                var templateId = uint.Parse(values[1]);
                var petName = values[2];
                var skillId = (huntingZoneId << 16) + ushort.Parse(values[3]);
                var skillName = values[4];
                var icon = values[5];
                var npcinfo = npcDatabase.GetOrPlaceholder(huntingZoneId, templateId);
                var skill = new UserSkill(skillId, new RaceGenderClass(Race.Common,Gender.Common,PlayerClass.Common), skillName, null, string.IsNullOrWhiteSpace(petName) ? npcinfo.Name : petName, icon, npcinfo);
                var lookup = Tuple.Create(huntingZoneId, templateId);
                if (!_petSkilldata.ContainsKey(lookup))
                {
                    _petSkilldata[lookup] = new List<UserSkill>();
                }
                _petSkilldata[lookup].Add(skill);
            }
        }

        public UserSkill GetOrNull(NpcInfo pet, int skillId)
        {
            var lookup = Tuple.Create(pet.HuntingZoneId, pet.TemplateId);
            if (!_petSkilldata.ContainsKey(lookup))
            {
                return null;
            }
            return _petSkilldata[lookup].FirstOrDefault(skill => skill.Id == skillId);
        }
    }
}