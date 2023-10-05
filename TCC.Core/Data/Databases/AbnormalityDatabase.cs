using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TCC.Data.Abnormalities;
using TCC.Data.Skills;
using TeraDataLite;

namespace TCC.Data.Databases;

public class AbnormalityDatabase : DatabaseBase
{
    public Dictionary<uint, Abnormality> Abnormalities { get; }
    public static readonly List<uint> NoctIds = new() { 902, 910, 911, 912, 913, 916, 917, 999010000 };
    public static List<uint> BlueNoctIds = new() { 920, 921, 922 };

    protected override string FolderName => "hotdot";
    protected override string Extension => "tsv";

    public AbnormalityDatabase(string lang) : base(lang)
    {
        Abnormalities = new Dictionary<uint, Abnormality>();
    }

    public override void Load()
    {
        Abnormalities.Clear();
        var lines = File.ReadAllLines(FullPath);
        foreach (var l in lines.Where(x => x != ""))
        {
            var s = l.Split('\t');
            var id = uint.Parse(s[0]);
            var isBuff = bool.Parse(s[3]);
            var isShow = bool.Parse(s[14]);
            var abType = (AbnormalityType)Enum.Parse(typeof(AbnormalityType), s[2]);
            var infinite = s[5] == "0";
            var ab = new Abnormality(id, isShow, isBuff, infinite, abType, s[13], s[8], s[11]);
            if (s[1].IndexOf("Absorb", StringComparison.Ordinal) > -1)
            {
                var amount = double.Parse(s[7], CultureInfo.InvariantCulture);
                ab.SetShield(amount); //TODO: may just parse everything instead of doing this
            }

            if (Abnormalities.TryGetValue(id, out var ex))
            {
                if (!ex.IsShield && ab.IsShield) Abnormalities[id] = ab;
                if (ab.Infinity && !ex.Infinity) ex.Infinity = false;
                if (ex.Type != AbnormalityType.Debuff && ab.Type == AbnormalityType.Debuff)
                    ex.Type = AbnormalityType.Debuff;
                if (!isBuff) ex.IsBuff = false;
                continue;
            }

            if (App.Settings.BuffWindowSettings.Specials.Contains(id) && ab.Type == AbnormalityType.Buff)
                ab.Type = AbnormalityType.Special;

            Abnormalities[id] = ab;
        }

        var foglioAura = new Abnormality(10241024, true, true, true, AbnormalityType.Special, "icon_items.bloodchipa_tex", "Foglio's aura", "Reduces your ping by $H_W_GOOD80$COLOR_END ms when one of $H_W_GOODFoglio$COLOR_END 's characters is nearby.$BRDoes not stack with Skill prediction.");
        Abnormalities[foglioAura.Id] = foglioAura;

        var fearInoculum = new Abnormality(30082019, true, true, true, AbnormalityType.Special, "icon_status.third_eye_ab", "Fear Inoculum", "New $H_W_GOODTool$COLOR_END album release provides the following effects:$BR - increases attack speed by $H_W_GOOD25%$COLOR_END $BR - increases skill damage by $H_W_GOOD100%$COLOR_END $BR - decreases skill cooldowns by $H_W_GOOD80%$COLOR_END $BR - increases drop rate in dungeons by $H_W_GOOD800%$COLOR_END $BR$BREffect only applies while Tool music is playing.");
        Abnormalities[fearInoculum.Id] = fearInoculum;

        var zonaGialla = new Abnormality(10240001, true, true, true, AbnormalityType.Buff, "icon_status.yellowaura_tex", "Zona Terradrax", "Sei in <font color='#fcee49'>zona gialla</font>. $BRPuoi andare in dungeon che consentono il mantenimento della distanza minima di $H_W_GOODun metro$COLOR_END tra le persone. Permane l'obbligo di indossare Annihilation Mask o Dark Light Mask. $BR$BR<font color='#777777'>Questo è un friendly reminder per invitare le persone ad essere più responsabili. Se lo stai leggendo probabilmente stai già rimanendo a casa, bravo bimbo.</font>");
        Abnormalities[zonaGialla.Id] = zonaGialla;
        var zonaArancione = new Abnormality(10240002, true, true, true, AbnormalityType.Buff, "icon_status.orangeaura_tex", "Zona Ignidrax", "Sei in <font color='#ffa047'>zona arancione</font>. $BRSono consentiti gli spostamenti solo all'interno del proprio villaggio. Divieto di andare in $H_W_BADraid da 10$COLOR_END o più per evitare assembramenti. I merchant e gli speciality store sono aperti fino alle 21. $BR$BR<font color='#777777'>Questo è un friendly reminder per invitare le persone ad essere più responsabili. Se lo stai leggendo probabilmente stai già rimanendo a casa, bravo bimbo.</font>");
        Abnormalities[zonaArancione.Id] = zonaArancione;
        var zonaRossa = new Abnormality(10240003, true, true, true, AbnormalityType.Buff, "icon_status.redaura_tex", "Zona Umbradrax", "Sei in <font color='#f24141'>zona rossa</font>. $BRSono consentiti gli spostamenti solo per motivi di necessità. Si può andare in dungeon al massimo in $H_W_GOOD3 persone$COLOR_END. I merchant e gli speciality store restano chiusi, ma possono effettuare consegne a domicilio. $BR$BR<font color='#777777'>Questo è un friendly reminder per invitare le persone ad essere più responsabili. Se lo stai leggendo probabilmente stai già rimanendo a casa, bravo bimbo.</font>");
        Abnormalities[zonaRossa.Id] = zonaRossa;
            
        #region Extreme overrides
        // Twisted Fate
        Abnormalities[781066].ToolTip = Abnormalities[781066].ToolTip.Replace("60", "3");

        // Broken Ball
        Abnormalities[44300011].ToolTip = "Increases Physical and Magic Amplification by $H_W_GOOD250$COLOR_END per stack";

        // Larva Venom
        Abnormalities[44300080].ToolTip = Abnormalities[44300080].ToolTip.Replace("3s", "2s");

        // God's Wrath
        Abnormalities[91100400].ToolTip = "Reduces Endurance, Power and Movement Speed by $H_W_BAD4%$COLOR_END$BRReduces Attack Speed by $H_W_BAD3%$COLOR_END";

        // Demokron's Curse
        Abnormalities[777034].ToolTip = "Boss HP 100%~30%$BR - Increases Physical and Magic Amplification by $H_W_GOOD7k$COLOR_END$BR - Reduces HP by $H_W_BAD3%$COLOR_END per 1s$BR$BRBoss HP 30%~0%$BR - Increases Physical and Magic Amplification by $H_W_GOOD7k$COLOR_END$BR - Reduces HP by $H_W_BAD2%$COLOR_END per 1s$BR - $H_W_BADCannot be cleansed$COLOR_END";
        #endregion

        #region Corrupted Skynest overrides

        // Doomfire
        Abnormalities[30260001].IsBuff = true;
        // Doomchill
        Abnormalities[30260002].IsBuff = true;

        #endregion

    }

    public bool TryGetPassiveSkill(uint id, out Skill sk)
    {
        var ret = false;
        if (PassivityDatabase.TryGetPassivitySkill(id, out sk)) ret = true;
        else if (Abnormalities.TryGetValue(id, out var ab))
        {
            ret = true;
            sk = new Skill(ab.Id, Class.None, ab.Name, ab.ToolTip) { IconName = ab.IconName };
        }
        return ret;
    }

    public bool GetAbnormality(uint id, out Abnormality ab)
    {
        ab = new Abnormality(0, false, false, false, AbnormalityType.Buff, "", "", "");
        if (!Abnormalities.TryGetValue(id, out var found)) return false;

        ab = found;
        return true;

    }
}