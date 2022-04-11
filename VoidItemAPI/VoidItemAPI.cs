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
        public Dictionary<ItemDef?, ItemDef[]?> voidToTransformations;
        public Dictionary<CustomVoidItem, ItemDef[]?> modifyTransformations;
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
            Logger = base.Logger;
            voidToTransformations = new Dictionary<ItemDef, ItemDef[]>();
            modifyTransformations = new Dictionary<CustomVoidItem, ItemDef[]>();
            harmony = new Harmony(MODGUID);

            new PatchClassProcessor(harmony, typeof(VoidItemAPI)).Patch();
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
            foreach (KeyValuePair<ItemDef?, ItemDef[]?> pair in instance.voidToTransformations)
            {
                if (!ValidateItem(pair.Key))
                {
                    instance.Logger.LogError("Initialization failed, ensure the VoidItem ItemDef has been properly created before intializing or modifying, and that the method is not being called too late.");
                    continue;
                }
                if (!VoidTiers.Contains(pair.Key!.tier))
                {
                    instance.Logger.LogWarning(pair.Key!.name + "'s item tier is not set to a void tier. Is this intended?");
                }
                pair.Key.requiredExpansion = ExpansionCatalog.expansionDefs.FirstOrDefault(def => def.nameToken == "DLC1_NAME");
                foreach (ItemDef item in pair.Value)
                {
                    if (!ValidateItem(item))
                    {
                        instance.Logger.LogError("Error initializing transformation for " + pair.Key.name + ", check CreateTransformation declaration.");
                        continue;
                    }
                    pairs.Add(new ItemDef.Pair { itemDef1 = item!, itemDef2 = pair!.Key });
                }
            }
            AddToRelationshipsTable(ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].AddRangeToArray(pairs.ToArray()));
        }

        private static bool ValidateItem(ItemDef? item)
        {
            if (!item) { return false; }
            if (string.IsNullOrEmpty(item!.name) || string.IsNullOrEmpty(item!.nameToken)) { return false; }
            return true;
        }

        private static void AddToRelationshipsTable(ItemDef.Pair[] pairs)
        {
            if (pairs.Length <= 0)
            {
                instance.Logger.LogError("Problem initializing transformation table, aborting.");
                return;
            }
            ModifyRelationtshipsTable(ref pairs);
            ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = pairs;
            instance.Logger.LogMessage("Transformations updated successfuly!");
        }

        private static void ModifyRelationtshipsTable(ref ItemDef.Pair[] pairs)
        {
            foreach (KeyValuePair<CustomVoidItem, ItemDef[]?> pair in instance.modifyTransformations)
            {
                if (!ValidateItem(pair.Key.itemDef))
                {
                    instance.Logger.LogError("Initialization failed, ensure the VoidItem ItemDef has been properly created before intializing or modifying, and that the method is not being called too late.");
                    continue;
                }
                if (!VoidTiers.Contains(pair.Key.itemDef!.tier))
                {
                    instance.Logger.LogWarning(pair.Key!.itemDef.name + "'s item tier is not set to a void tier. Is this intended?");
                }
                if (!pairs.Contains(pair.Key.itemDef))
                foreach (ItemDef item in pair.Value)
                {

                }
            }
        }
    }
}
