using MonoMod.Cil;
using RoR2;
using SneedHooks;
using System;
using UnityEngine;

namespace LinearDamage.Tweaks.Items
{
    public class APRounds : TweakBase<APRounds>
    {
        public override string ConfigCategoryString => "Items";

        public override string ConfigOptionName => "AP Rounds";

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
            if (!victimBody.isBoss || !attackerBody.inventory || attackerBody == victimBody) return;
            int itemCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.BossDamageBonus);
            if (itemCount > 0)
            {
                damageModifierArgs.damageMultAdd += 0.2f * itemCount;
                if (damageInfo.damageColorIndex == DamageColorIndex.Default) damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                EffectManager.SimpleImpactEffect(HealthComponent.AssetReferences.bossDamageBonusImpactEffectPrefab, damageInfo.position, damageInfo.force, true);
            }
        }

        private void RemoveVanilla(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(RoR2Content.Items), "BossDamageBonus"), x => x.MatchCallvirt<Inventory>("GetItemCount")))
            {
                c.EmitDelegate<Func<int, int>>(i => 0);
            }
            else
            {
                Debug.LogError("LinearDamage: APRounds RemoveVanilla IL hook failed.");
            }
        }
    }
}
