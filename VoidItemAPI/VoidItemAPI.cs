using System;
using HarmonyLib;
using UnityEngine;
using RoR2;
using System.Collections.Generic;
using BepInEx;
using RoR2.ExpansionManagement;
using System.Linq;

#pragma warning disable CS8632
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
        public List<CustomVoidEntry> entries;
        public List<VoidItemModification> modifications;
        public List<ItemDef.Pair> newTransformationTable;
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
            instance.modifications = new List<VoidItemModification>();
            instance.harmony = new Harmony(MODGUID);

            new PatchClassProcessor(harmony, typeof(VoidItemAPI)).Patch();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(RoR2.Items.ContagiousItemManager), nameof(RoR2.Items.ContagiousItemManager.Init))]
        private static void InitializeTransformations()
        {
            instance.newTransformationTable = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].ToList();
            instance.tooLate = true;
            if (instance.entries.Count <= 0 && instance.modifications.Count <= 0)
            {
                instance.Logger.LogWarning("No void items in the entries and modifications list! Cancelling transformation.");
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
                instance.newTransformationTable.Add(new ItemDef.Pair() { itemDef1 = entry.TransformedItem, itemDef2 = entry.VoidItem });
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
                instance.newTransformationTable.Add(new ItemDef.Pair() { itemDef1 = itemDef, itemDef2 = entry.VoidItem });
                instance.Logger.LogMessage("Successfully created a transformation for " + entry.VoidItem.name + "!");
            }

            ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = instance.newTransformationTable.ToArray();
            
            IEnumerable<VoidItemModification> modDefs = instance.modifications.Where(x => x.transformType == CustomVoidEntry.TransformType.Def);
            IEnumerable<VoidItemModification> modNames = instance.modifications.Where(x => x.transformType == CustomVoidEntry.TransformType.Name);

            foreach (VoidItemModification mod in modDefs)
            {
                if (mod.modification == VoidItemModification.ModificationType.Modify)
                {
                    ChangeTransformation(mod);
                }
                else
                {
                    RemoveTransformation(mod);
                }
            }
            foreach (VoidItemModification mod in modNames)
            {
                ItemDef VoidItem = ValidateItemString(mod.VoidItemName);
                ItemDef CurrentTransformedItem = ValidateItemString(mod.CurrentTransformedItemName);
                ItemDef NewTransformation = null;
                if(mod.modification == VoidItemModification.ModificationType.Modify)
                    NewTransformation = ValidateItemString(mod.NewTransformationName);
                
                if (!ValidateItem(VoidItem) || !ValidateItem(CurrentTransformedItem))
                {
                    instance.Logger.LogError("Issue modifying transformation for " + mod.VoidItemName + ". Aborting this modification.");
                    continue;
                }
                if (mod.modification == VoidItemModification.ModificationType.Modify)
                {
                    ChangeTransformation(new VoidItemModification(VoidItem, CurrentTransformedItem, NewTransformation, mod.modification));
                }
                else
                {
                    RemoveTransformation(new VoidItemModification(VoidItem, CurrentTransformedItem, NewTransformation, mod.modification));
                }
            }

            ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = instance.newTransformationTable.ToArray();
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

        private static bool IsInTable(ItemDef def, ItemDef transformation, out ItemDef.Pair pair)
        {
            pair = new ItemDef.Pair { itemDef1 = transformation, itemDef2 = def};
            return instance.newTransformationTable.Contains(pair);
        }

        private static void ChangeTransformation(VoidItemModification mod)
        {
            if (!IsInTable(mod.VoidItem, mod.CurrentTransformedItem, out ItemDef.Pair pair))
            {
                instance.Logger.LogError("Transformation between " + mod.VoidItem.name + " and " + mod.CurrentTransformedItem.name + " does not exist in the transformation table. Aborting modification.");
                return;
            }
            instance.newTransformationTable.Remove(pair);
            instance.newTransformationTable.Add(new ItemDef.Pair { itemDef1 = mod.NewTransformation, itemDef2 = mod.VoidItem });
            instance.Logger.LogMessage("Transformation registered for " + mod.VoidItem.name + " is now " + mod.NewTransformation.name + ".");
        }

        private static void RemoveTransformation(VoidItemModification mod)
        {
            if (!IsInTable(mod.VoidItem, mod.CurrentTransformedItem, out ItemDef.Pair pair))
            {
                instance.Logger.LogError("Transformation between " + mod.VoidItem.name + " and " + mod.CurrentTransformedItem.name + " does not exist in the transformation table. Aborting removal.");
                return;
            }
            instance.newTransformationTable.Remove(pair);
            instance.Logger.LogMessage("Transformation between " + mod.VoidItem.name + " and " + mod.CurrentTransformedItem.name + " has been successfully removed.");
        }
    }
}
