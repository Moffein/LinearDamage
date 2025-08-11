using MonoMod.Cil;
using R2API;
using RoR2;
using SneedHooks;
using System;
using UnityEngine;

namespace LinearDamage.Tweaks.Items
{
    public class ChronicExpansion : TweakBase<ChronicExpansion>
    {
        public override string ConfigCategoryString => "Items";

        public override string ConfigOptionName => "Chronic Expansion";

        public override string ConfigDescriptionString => "Enable Changes";

        public override void ApplyChanges()
        {
            IL.RoR2.CharacterBody.RecalculateStats += RemoveVanilla;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int buffCount = sender.GetBuffCount(DLC2Content.Buffs.IncreaseDamageBuff);
            if (buffCount > 0)
            {
                int itemCount = 0;
                if (sender.inventory) itemCount = sender.inventory.GetItemCount(DLC2Content.Items.IncreaseDamageOnMultiKill);

                float stackBonus = (itemCount > 1) ? (0.01f * (itemCount - 1)) : 0f;
                args.damageMultAdd += buffCount * (0.035f + stackBonus);
            }
        }

        public override void RemoveChanges()
        {
            IL.RoR2.CharacterBody.RecalculateStats -= RemoveVanilla;
            RecalculateStatsAPI.GetStatCoefficients -= RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RemoveVanilla(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DLC2Content.Buffs), "IncreaseDamageBuff"), x => x.MatchCall<CharacterBody>("GetBuffCount")))
            {
                c.EmitDelegate<Func<int, int>>(i => 0);
            }
            else
            {
                Debug.LogError("LinearDamage: ChronicExpansion RemoveVanilla IL hook failed.");
            }
        }
    }
}
