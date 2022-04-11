using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static VoidItemAPI.VoidItemAPI;
using RoR2;
using RoR2.ExpansionManagement;

namespace VoidItemAPI
{
    public static class VoidTransformation
    {
        /// <summary>
        /// Creates a void item for a single item.
        /// </summary>
        /// <param name="VoidItem"></param>
        /// <param name="TransformedItem"></param>
        public static void CreateTransformation(ItemDef? VoidItem, ItemDef? TransformedItem)
        {
            Logger.LogWarning("Check 1");
            if (!ValidateItem(VoidItem) || !ValidateItem(TransformedItem))
            {
                Logger.LogError("Problem initializing transformation for void item. Be sure to create item correctly and/or load transformations at the correct time.");
                return;
            }
            if (!VoidTiers.Contains(VoidItem.tier))
            {
                Logger.LogWarning("Item: " + VoidItem.name + "'s ItemTier is not set to a void tier, was this intentional?");
            }
            Logger.LogWarning("Check 5");
            VoidItem.requiredExpansion = ExpansionCatalog.expansionDefs.FirstOrDefault(def => def.nameToken == "DLC1_NAME");
            Logger.LogWarning("Check 6");
            voidToTransformations.Add(VoidItem!, new ItemDef[1] { TransformedItem! });
            Logger.LogMessage("Transformation for " + VoidItem.name + " being setup...");
        }

        /// <summary>
        /// Creates a void item for more than one item.
        /// </summary>
        /// <param name="VoidItem"></param>
        /// <param name="TransformedItems"></param>
        public static void CreateTransformation(ItemDef? VoidItem, ItemDef[]? TransformedItems)
        {
            if (!ValidateItem(VoidItem))
            {
                Logger.LogError("Problem initializing transformation for void item. Be sure to create item correctly and/or load transformations at the correct time.");
                return;
            }
            if (!VoidTiers.Contains(VoidItem.tier))
            {
                Logger.LogWarning("Item: " + VoidItem.name + "'s ItemTier is not set to a void tier, was this intentional?");
            }
            if (TransformedItems == null || TransformedItems.Length <= 0)
            {
                Logger.LogError("Unable to create transformations, ensure ItemDef array is created correctly.");
            }

            VoidItem.requiredExpansion = ExpansionCatalog.expansionDefs.FirstOrDefault(def => def.nameToken == "DLC1_NAME");
            List<ItemDef> items = new List<ItemDef>();
            foreach (ItemDef? item in TransformedItems)
            {
                if (!ValidateItem(item))
                {
                    Logger.LogError("Problem initializing a tranformation for " + VoidItem.name + ", ensure ItemDefs are valid.");
                    continue;
                }
                items.Add(item!);
            }
            voidToTransformations.Add(VoidItem!, items.ToArray());
        }

        private static bool ValidateItem(ItemDef? item)
        {
            Logger.LogWarning("Check 2");
            if (!item) { return false; }
            Logger.LogWarning("Check 3");
            if (string.IsNullOrEmpty(item.name) || string.IsNullOrEmpty(item.nameToken)) { return false; }
            Logger.LogWarning("Check 4");
            return true;
        }
    }
}
