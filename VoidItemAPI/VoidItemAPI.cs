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

        private void Awake()
        {
            instance = this; 
            instance.Logger = base.Logger;
            instance.voidToTransformations = new Dictionary<ItemDef, ItemDef[]>();
            instance.harmony = new Harmony(MODGUID);

            new PatchClassProcessor(instance.harmony, typeof(VoidItemAPI)).Patch();
        }

        /// <summary>
        /// Creates a void item for a single item.
        /// </summary>
        /// <param name="VoidItem"></param>
        /// <param name="TransformedItem"></param>
        /// <param name="requiresDLC"></param>
        public static void CreateTransformation(ItemDef VoidItem, ItemDef TransformedItem, bool requiresDLC = true)
        {
            if (!ValidateItem(VoidItem) || !ValidateItem(TransformedItem))
            {
                instance.Logger.LogError("Problem initializing transformation for void item. Be sure to create item correctly and/or load transformations at the correct time.");
                return;
            }

            if (requiresDLC)
            {
                VoidItem.requiredExpansion = ExpansionCatalog.expansionDefs.FirstOrDefault(def => def.nameToken == "DLC1_NAME");
            }

            instance.voidToTransformations.Add(VoidItem, new ItemDef[1] { TransformedItem });
            instance.Logger.LogMessage("Transformation for " + VoidItem.name + " being setup...");
        }

        /// <summary>
        /// Creates a void item for more than one item.
        /// </summary>
        /// <param name="VoidItem"></param>
        /// <param name="TransformedItems"></param>
        /// <param name="requiresDLC"></param>
        public static void CreateTransformation(ItemDef VoidItem, ItemDef[] TransformedItems, bool requiresDLC = true)
        {
            if (!ValidateItem(VoidItem))
            {
                instance.Logger.LogError("Problem initializing transformation for void item. Be sure to create item correctly and/or load transformations at the correct time.");
                return;
            }

            if (requiresDLC)
            {
                VoidItem.requiredExpansion = ExpansionCatalog.expansionDefs.FirstOrDefault(def => def.nameToken == "DLC1_NAME");
            }
            
            List<ItemDef> items = new List<ItemDef>();
            foreach (ItemDef item in TransformedItems)
            {
                if(!ValidateItem(item))
                {
                    instance.Logger.LogError("Problem initializing a tranformation for " + VoidItem.name + ", ensure ItemDefs are valid.");
                    continue;
                }
                items.Add(item);
            }
            instance.voidToTransformations.Add(VoidItem, items.ToArray());
        }

        private static bool ValidateItem(ItemDef item)
        {
            if (!item || item.name == null || item.nameToken == null) { return false; }
            return true;
        }

        /// <summary>
        /// Get an ItemDef based on a string of its internal item name.
        /// </summary>
        /// <param name="ItemName"></param>
        /// <returns></returns>
        public static ItemDef GetItemDef(string ItemName)
        {
            if (!ItemCatalog.itemNames.Contains(ItemName)) { return null; }
            return ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(ItemName));
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
