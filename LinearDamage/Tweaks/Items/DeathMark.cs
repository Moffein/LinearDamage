using MonoMod.Cil;
using RoR2;
using SneedHooks;
using System;
using UnityEngine;

namespace LinearDamage.Tweaks.Items
{
    public class DeathMark : TweakBase<DeathMark>
    {
        public override string ConfigCategoryString => "Items";

        public override string ConfigOptionName => "Death Mark";

        public override string ConfigDescriptionString => "Enable Changes";

        public override void ApplyChanges()
        {
            IL.RoR2.HealthComponent.TakeDamageProcess += RemoveVanilla;
            SneedHooks.ModifyFinalDamage.ModifyFinalDamageActions += ModifyFinalDamage;
        }

        private void ModifyFinalDamage(ModifyFinalDamage.DamageModifierArgs damageModifierArgs, DamageInfo damageInfo, HealthComponent victim, CharacterBody victimBody)
        {
            if (victimBody.HasBuff(RoR2Content.Buffs.DeathMark))
            {
                damageModifierArgs.damageMultAdd += 0.5f;
                damageInfo.damageColorIndex = DamageColorIndex.DeathMark;
            }
        }

        public override void RemoveChanges()
        {
            IL.RoR2.HealthComponent.TakeDamageProcess -= RemoveVanilla;
            SneedHooks.ModifyFinalDamage.ModifyFinalDamageActions -= ModifyFinalDamage;
        }

        private void RemoveVanilla(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "DeathMark"), x => x.MatchCallvirt<CharacterBody>("HasBuff")))
            {
                c.EmitDelegate<Func<bool, bool>>(i => false);
            }
            else
            {
                Debug.LogError("LinearDamage: DeathMark  RemoveVanilla IL hook failed.");
            }
        }
    }
}
