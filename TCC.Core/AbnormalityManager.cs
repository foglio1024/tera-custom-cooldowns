using TCC.Data;
using TCC.Data.Databases;
using TCC.ViewModels;

namespace TCC
{
    public static class AbnormalityManager
    {
        public static AbnormalityDatabase CurrentDb;

        public static void BeginAbnormality(uint id, ulong target, uint duration, int stacks)
        {
            if (CurrentDb.Abnormalities.TryGetValue(id, out Abnormality ab))
            {
                if (!Filter(ab)) return;
                if (target == SessionManager.CurrentPlayer.EntityId)
                {
                    BeginPlayerAbnormality(ab, stacks, duration);
                    if (SettingsManager.DisablePartyAbnormals) return;
                    GroupWindowViewModel.Instance.BeginOrRefreshAbnormality(ab, stacks, duration, SessionManager.CurrentPlayer.PlayerId, SessionManager.CurrentPlayer.ServerId);
                }
                else
                {
                    BeginNpcAbnormality(ab, stacks, duration, target);
                }
            }
        }
        public static void EndAbnormality(ulong target, uint id)
        {
            if (CurrentDb.Abnormalities.TryGetValue(id, out Abnormality ab))
            {
                if (target == SessionManager.CurrentPlayer.EntityId)
                {
                    EndPlayerAbnormality(ab);
                    GroupWindowViewModel.Instance.EndAbnormality(ab, SessionManager.CurrentPlayer.PlayerId, SessionManager.CurrentPlayer.ServerId);

                }
                //else if (EntitiesManager.TryGetBossById(target, out Npc b))
                //{
                //    b.EndBuff(ab);
                //}
                else
                {
                    BossGageWindowViewModel.Instance.EndNpcAbnormality(target, ab);
                }
            }

        }

        private static void BeginPlayerAbnormality(Abnormality ab, int stacks, uint duration)
        {
            if (ab.Type == AbnormalityType.Buff)
            {
                if (ab.Infinity)
                {
                    BuffBarWindowViewModel.Instance.Player.AddOrRefreshInfBuff(ab, duration, stacks);

                }
                else
                {
                    BuffBarWindowViewModel.Instance.Player.AddOrRefreshBuff(ab, duration, stacks);
                    if(ab.IsShield) SessionManager.SetPlayerMaxShield(ab.ShieldSize);
                }
            }
            else
            {
                BuffBarWindowViewModel.Instance.Player.AddOrRefreshDebuff(ab, duration, stacks);
                CharacterWindowViewModel.Instance.Player.AddToDebuffList(ab);
                ClassManager.SetStatus(ab, true);
            }
            CheckPassivity(ab);
            //var sysMsg = new ChatMessage("@661\vAbnormalName\v" + ab.Name, SystemMessages.Messages["SMT_BATTLE_BUFF_DEBUFF"]);
            //ChatWindowManager.Instance.AddChatMessage(sysMsg);

        }

        private static void CheckPassivity(Abnormality ab)
        {
            if (PassivityDatabase.Passivities.Contains(ab.Id))
            {
                SkillManager.AddPassivitySkill(ab.Id, 60);
            }
        }

        private static void EndPlayerAbnormality(Abnormality ab)
        {
            if (ab.Type == AbnormalityType.Buff)
            {
                if (ab.Infinity)
                {
                    BuffBarWindowViewModel.Instance.Player.RemoveInfBuff(ab);
                }
                else
                {

                    BuffBarWindowViewModel.Instance.Player.RemoveBuff(ab);
                    if (ab.IsShield) SessionManager.SetPlayerShield(0);
                    if (ab.IsShield) SessionManager.SetPlayerMaxShield(0);

                }
            }
            else
            {
                BuffBarWindowViewModel.Instance.Player.RemoveDebuff(ab);
                CharacterWindowViewModel.Instance.Player.RemoveFromDebuffList(ab);
                ClassManager.SetStatus(ab, false);
            }
        }

        private static void BeginNpcAbnormality(Abnormality ab, int stacks, uint duration, ulong target)
        {
            //if (EntitiesViewModel.TryGetBossById(target, out Npc b))
            //{
            //    b.AddorRefresh(ab, duration, stacks, BOSS_AB_SIZE, BOSS_AB_LEFT_MARGIN);
            //}
            BossGageWindowViewModel.Instance.AddOrRefreshNpcAbnormality(ab, stacks, duration, target);
        }

        private static bool Filter(Abnormality ab)
        {
            if (ab.Name.Contains("BTS") || ab.ToolTip.Contains("BTS") || !ab.IsShow) return false;
            if (ab.Name.Contains("(Hidden)") || ab.Name.Equals("Unknown") || ab.Name.Equals(string.Empty)) return false;
            return true;
        }

        public static void BeginOrRefreshPartyMemberAbnormality(uint playerId, uint serverId, uint id, uint duration, int stacks)
        {
            if (CurrentDb.Abnormalities.TryGetValue(id, out Abnormality ab))
            {
                if (!Filter(ab)) return;
                GroupWindowViewModel.Instance.BeginOrRefreshAbnormality(ab, stacks, duration, playerId, serverId);
            }
        }

        internal static void EndPartyMemberAbnormality(uint playerId, uint serverId, uint id)
        {
            if (CurrentDb.Abnormalities.TryGetValue(id, out Abnormality ab))
            {
                if (!Filter(ab)) return;
                GroupWindowViewModel.Instance.EndAbnormality(ab, playerId, serverId);
            }
        }
    }
}
