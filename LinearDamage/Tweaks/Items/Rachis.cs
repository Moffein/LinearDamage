using MonoMod.Cil;
using R2API;
using RoR2;
using SneedHooks;
using System;
using UnityEngine;

namespace LinearDamage.Tweaks.Items
{
    public class Rachis : TweakBase<Rachis>
    {
        public override string ConfigCategoryString => "Items";

        public override string ConfigOptionName => "Mercurial Rachis";

        public override string ConfigDescriptionString => "Enable Changes";

        public override void ApplyChanges()
        {
            IL.RoR2.CharacterBody.RecalculateStats += RemoveVanilla;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(RoR2Content.Buffs.PowerBuff))
            {
                args.damageMultAdd += 0.5f;
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
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "PowerBuff"), x => x.MatchCall<CharacterBody>("HasBuff")))
            {
                c.EmitDelegate<Func<int, int>>(i => 0);
            }
            else
            {
                Debug.LogError("LinearDamage: Rachis RemoveVanilla IL hook failed.");
            }
        }
    }
}
