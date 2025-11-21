using BepInEx;
using LinearDamage.Tweaks;
using R2API.Utils;
using RoR2;
using System;
using System.Linq;
using System.Reflection;

namespace LinearDamage
{
    [BepInDependency("com.RiskyLives.SneedHooks")]
    [BepInPlugin("com.RiskyLives.LinearDamage", "LinearDamage", "1.0.3")]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    public class LinearDamagePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            AddToAssembly();
        }

        private void AddToAssembly()
        {
            var fixTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(TweakBase)));

            foreach (var fixType in fixTypes)
            {
                TweakBase fix = (TweakBase)Activator.CreateInstance(fixType);
                fix.Init(Config);
            }
        }
    }
}
