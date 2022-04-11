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
        /// <param name="requiresDLC"></param>
        public static void CreateTransformation(ItemDef? VoidItem, ItemDef? TransformedItem, bool requiresDLC = true)
        {
            if (!ValidateItem(VoidItem) || !ValidateItem(TransformedItem))
            {
                instance.Logger.LogError("Problem initializing transformation for void item. Be sure to create item correctly and/or load transformations at the correct time.");
                return;
            }
            if (!VoidTiers.Contains(VoidItem.tier))
            {
                instance.Logger.LogWarning("Item: " + VoidItem.name + "'s ItemTier is not set to a void tier, was this intentional?");
            }

            if (requiresDLC)
            {
                VoidItem.requiredExpansion = ExpansionCatalog.expansionDefs.FirstOrDefault(def => def.nameToken == "DLC1_NAME");
            }

            instance.voidToTransformations.Add(VoidItem!, new ItemDef[1] { TransformedItem! });
            instance.Logger.LogMessage("Transformation for " + VoidItem.name + " being setup...");
        }

        /// <summary>
        /// Creates a void item for more than one item.
        /// </summary>
        /// <param name="VoidItem"></param>
        /// <param name="TransformedItems"></param>
        /// <param name="requiresDLC"></param>
        public static void CreateTransformation(ItemDef? VoidItem, ItemDef[]? TransformedItems, bool requiresDLC = true)
        {
            if (!ValidateItem(VoidItem))
            {
                instance.Logger.LogError("Problem initializing transformation for void item. Be sure to create item correctly and/or load transformations at the correct time.");
                return;
            }
            if (!VoidTiers.Contains(VoidItem.tier))
            {
                instance.Logger.LogWarning("Item: " + VoidItem.name + "'s ItemTier is not set to a void tier, was this intentional?");
            }
            if (TransformedItems == null || TransformedItems.Length <= 0)
            {
                instance.Logger.LogError("Unable to create transformations, ensure ItemDef array is created correctly.");
            }

            if (requiresDLC)
            {
                VoidItem.requiredExpansion = ExpansionCatalog.expansionDefs.FirstOrDefault(def => def.nameToken == "DLC1_NAME");
            }

            List<ItemDef> items = new List<ItemDef>();
            foreach (ItemDef? item in TransformedItems)
            {
                if (!ValidateItem(item))
                {
                    instance.Logger.LogError("Problem initializing a tranformation for " + VoidItem.name + ", ensure ItemDefs are valid.");
                    continue;
                }
                items.Add(item!);
            }
            instance.voidToTransformations.Add(VoidItem!, items.ToArray());
        }

        private static bool ValidateItem(ItemDef? item)
        {
            if (!item) { return false; }
            if (string.IsNullOrEmpty(item.name) || string.IsNullOrEmpty(item.nameToken)) { return false; }
            return true;
        }
    }
}
