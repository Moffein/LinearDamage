using MonoMod.Cil;
using RoR2;
using SneedHooks;
using System;
using UnityEngine;

namespace LinearDamage.Tweaks.Items
{
    public class DelicateWatch : TweakBase<DelicateWatch>
    {
        public override string ConfigCategoryString => "Items";

        public override string ConfigOptionName => "Delicate Watch";

        public override string ConfigDescriptionString => "Enable Changes";

        public override void ApplyChanges()
        {
            IL.RoR2.HealthComponent.TakeDamageProcess += RemoveVanilla;
            SneedHooks.ModifyFinalDamage.ModifyFinalDamageAttackerActions += ModifyFinalDamage;
        }

        public override void RemoveChanges()
        {
            IL.RoR2.HealthComponent.TakeDamageProcess -= RemoveVanilla;
            SneedHooks.ModifyFinalDamage.ModifyFinalDamageAttackerActions -= ModifyFinalDamage;
        }

        private void ModifyFinalDamage(ModifyFinalDamage.DamageModifierArgs damageModifierArgs, DamageInfo damageInfo, HealthComponent victim, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (!attackerBody.inventory) return;
            int itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.FragileDamageBonus);
            if (itemCount > 0) damageModifierArgs.damageMultAdd += 0.2f * itemCount;
        }

        private void RemoveVanilla(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DLC1Content.Items), "FragileDamageBonus"), x => x.MatchCallvirt<Inventory>("GetItemCountEffective")))
            {
                c.EmitDelegate<Func<int, int>>(i => 0);
            }
            else
            {
                Debug.LogError("LinearDamage: DelicateWatch RemoveVanilla IL hook failed.");
            }
        }
    }
}
