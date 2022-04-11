using System;
using HarmonyLib;
using UnityEngine;
using RoR2;
using System.Collections.Generic;
using BepInEx;
using RoR2.ExpansionManagement;
using System.Linq;

namespace VoidItemAPI
{
    [BepInPlugin(MODGUID, MODNAME, MODVERSION)]
    [HarmonyPatch]
    public class VoidItemAPI : BaseUnityPlugin
    {
        public const string MODNAME = "VoidItemAPI";
        public const string MODVERSION = "1.0.0";
        public const string MODGUID = "com.RumblingJOSEPH.VoidItemAPI";

        public static VoidItemAPI instance;
        public BepInEx.Logging.ManualLogSource Logger;
        public Dictionary<ItemDef, ItemDef[]> voidToTransformations;
        public Harmony harmony;
        public static ItemTier[] VoidTiers = new ItemTier[]
        {
            ItemTier.VoidTier1,
            ItemTier.VoidTier2,
            ItemTier.VoidTier3,
            ItemTier.VoidBoss
        };

        private void Awake()
        {
            instance = this; 
            instance.Logger = base.Logger;
            instance.voidToTransformations = new Dictionary<ItemDef, ItemDef[]>();
            instance.harmony = new Harmony(MODGUID);

            new PatchClassProcessor(instance.harmony, typeof(VoidItemAPI)).Patch();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(RoR2.Items.ContagiousItemManager), nameof(RoR2.Items.ContagiousItemManager.Init))]
        private static void InitializeTransformations()
        {
            if (instance.voidToTransformations.Count <= 0)
            {
                instance.Logger.LogError("Created void items too late! Cancelling transformation.");
                return;
            }

            List<ItemDef.Pair> pairs = new List<ItemDef.Pair>();
            foreach (KeyValuePair<ItemDef, ItemDef[]> pair in instance.voidToTransformations)
            {
                foreach (ItemDef item in pair.Value)
                {
                    pairs.Add(new ItemDef.Pair { itemDef1 = item, itemDef2 = pair.Key });
                }
            }
            ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].AddRangeToArray(pairs.ToArray());
            instance.Logger.LogMessage("Tranformations created successfully!");
        }
    }
}
