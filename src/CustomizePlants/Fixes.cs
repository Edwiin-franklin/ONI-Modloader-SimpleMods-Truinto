﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomizePlants
{
    public class Fixes
    {
        [HarmonyPatch(typeof(FallingWater), nameof(FallingWater.AddParticle),
            typeof(Vector2), typeof(byte), typeof(float), typeof(float), typeof(byte), typeof(int), typeof(bool), typeof(bool), typeof(bool), typeof(bool))]
        public static class Patch_ElementConsumerCrashWithGases
        {
            public static bool Prepare()
            {
                return CustomizePlantsState.StateManager.State.ApplyBugFixes;
            }

            public static bool Prefix(float base_mass)
            {
                if (base_mass <= 0f)
                    return false;
                return true;
            }
        }

        [HarmonyPatch(typeof(SaltPlantConfig), nameof(SaltPlantConfig.OnSpawn))]
        public static class Patch_SaltPlantConfigOnSpawn
        {
            public static bool Prepare()
            {
                return CustomizePlantsState.StateManager.State.ApplyBugFixes;
            }

            public static bool Prefix(GameObject inst)
            {
                inst.GetComponent<ElementConsumer>()?.EnableConsumption(true);
                return false;
            }
        }

        [HarmonyPatch(typeof(ColdBreather), "OnReplanted")]
        public static class Patch_ColdBreatherOnReplanted
        {
            public static float? _cache = CustomizePlantsState.StateManager.State.PlantSettings.FirstOrDefault(f => f.id == ColdBreatherConfig.ID)?.radiation;

            public static bool Prepare()
            {
                return CustomizePlantsState.StateManager.State.ApplyBugFixes;
            }

            public static void Postfix(RadiationEmitter ___radiationEmitter)
            {
                if (_cache != null)
                    ___radiationEmitter.emitRads = _cache.Value;
            }
        }
    }
}
