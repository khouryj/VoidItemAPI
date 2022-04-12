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
        Dictionary<ItemDef, ItemDef[]> transformations;
        public List<CustomVoidEntry> entries;
        public Harmony harmony;
        public bool tooLate = false;
        public bool defsUsed = true;
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
            instance.entries = new List<CustomVoidEntry>();
            instance.transformations = new Dictionary<ItemDef, ItemDef[]>();
            instance.harmony = new Harmony(MODGUID);

            new PatchClassProcessor(harmony, typeof(VoidItemAPI)).Patch();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(RoR2.Items.ContagiousItemManager), nameof(RoR2.Items.ContagiousItemManager.Init))]
        private static void InitializeTransformations()
        {
            instance.tooLate = true;
            if (instance.entries.Count <= 0)
            {
                instance.Logger.LogWarning("No void items in the entries list! Cancelling transformation.");
                return;
            }
            foreach (CustomVoidEntry entry in instance.entries)
            {
                if (entry.VoidItem.requiredExpansion == null)
                {
                    entry.VoidItem.requiredExpansion = ExpansionCatalog.expansionDefs.FirstOrDefault(x => x.nameToken == "DLC1_NAME");
                }
            }
            IEnumerable<CustomVoidEntry> defs = instance.entries.Where(x => x.transformType == CustomVoidEntry.TransformType.Def);
            IEnumerable<CustomVoidEntry> names = instance.entries.Where(x => x.transformType == CustomVoidEntry.TransformType.Name);
            
            foreach (CustomVoidEntry entry in defs)
            {
                ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].AddToArray(new ItemDef.Pair() { itemDef1 = entry.TransformedItem, itemDef2 = entry.VoidItem });
                instance.Logger.LogMessage("Successfully created a transformation for " + entry.VoidItem.name + "!");
            }
            foreach (CustomVoidEntry entry in names)
            {
                ItemDef itemDef = ValidateItemString(entry.TransformedItemName);
                if (!ValidateItem(itemDef))
                {
                    instance.Logger.LogError("Failed to create a transformation for " + entry.VoidItem.name + " on transformation name " + entry.TransformedItemName + ".");
                    continue;
                }
                ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].AddToArray(new ItemDef.Pair() { itemDef1 = itemDef, itemDef2 = entry.VoidItem });
                instance.Logger.LogMessage("Successfully created a transformation for " + entry.VoidItem.name + "!");
            }
        }

        public static bool ValidateItem(ItemDef? item)
        {
            if (!item) { return false; }
            if (string.IsNullOrEmpty(item!.name) || string.IsNullOrEmpty(item!.nameToken)) { return false; }
            return true;
        }

        private static ItemDef ValidateItemString(string itemName)
        {
            if (!ItemCatalog.itemNames.Contains(itemName))
            {
                instance.Logger.LogError("The item catalog does not contain an item called: " + itemName + ". Aborting this transformation.");
                return null;
            }
            return ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(itemName));
        }
    }
}
