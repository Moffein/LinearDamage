using MonoMod.Cil;
using RoR2;
using SneedHooks;
using System;
using UnityEngine;

namespace LinearDamage.Tweaks.Items
{
    public class FocusCrystal : TweakBase<FocusCrystal>
    {
        public override string ConfigCategoryString => "Items";

        public override string ConfigOptionName => "Focus Crystal";

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
            if (!attackerBody.inventory || attackerBody == victimBody) return;
            int itemCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.NearbyDamageBonus);

            Vector3 attackVector = attackerBody.corePosition - damageInfo.position;

            if (itemCount > 0 && attackVector.sqrMagnitude < 169f)
            {
                damageModifierArgs.damageMultAdd += 0.2f * itemCount;
                damageInfo.damageColorIndex = DamageColorIndex.Nearby;
                EffectManager.SimpleImpactEffect(HealthComponent.AssetReferences.diamondDamageBonusImpactEffectPrefab, damageInfo.position, attackVector, true);
            }
        }

        private void RemoveVanilla(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(RoR2Content.Items), "NearbyDamageBonus"), x => x.MatchCallvirt<Inventory>("GetItemCount")))
            {
                c.EmitDelegate<Func<int, int>>(i => 0);
            }
            else
            {
                Debug.LogError("LinearDamage: FocusCrystal RemoveVanilla IL hook failed.");
            }
        }
    }
}
