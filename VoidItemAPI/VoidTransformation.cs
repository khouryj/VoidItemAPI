using static VoidItemAPI.VoidItemAPI;
using RoR2;
using System.Linq;

namespace VoidItemAPI
{
    public class CustomVoidEntry
    {
        public ItemDef VoidItem { get; set; }
        public ItemDef TransformedItem { get; set; }
        public string TransformedItemName { get; set; }
        public TransformType transformType { get; set; }
        public enum TransformType
        {
            Def,
            Name
        }

        public CustomVoidEntry(ItemDef VoidItem, ItemDef TransformedItem)
        {
            this.VoidItem = VoidItem;
            this.TransformedItem = TransformedItem;
            this.transformType = TransformType.Def;
        }

        public CustomVoidEntry(ItemDef VoidItem, string TransformedItemName)
        {
            this.VoidItem = VoidItem;
            this.TransformedItemName = TransformedItemName;
            this.transformType = TransformType.Name;
        }
    }

    public static class VoidTransformation
    {
        /// <summary>
        /// Creates a void item for a single ItemDef. Should only be used AFTER ItemCatalog is initialized!
        /// </summary>
        /// <param name="VoidItem">Void item to be created</param>
        /// <param name="TransformedItem">Item to be transformed into the void item.</param>
        public static void CreateTransformation(ItemDef? VoidItem, ItemDef? TransformedItem)
        {
            if (!ValidateItem(VoidItem))
            {
                instance.Logger.LogError("Void item is not a valid item. Did you call CreateTransformation too early?");
                return;
            }
            if (!ValidateItem(TransformedItem))
            {
                instance.Logger.LogError("Transformed item is either not valid or you called CreateTransformation too early. Initialization cancelled for void item: " + VoidItem.name + ".");
                return;
            }
            if (instance.tooLate)
            {
                instance.Logger.LogError("CreateTransformation has been called too late for void item: " + VoidItem.name + ".");
                return;
            }
            if (!VoidTiers.Contains(VoidItem!.tier))
            {
                instance.Logger.LogWarning(VoidItem.name + "'s item tier is not set to a void tier. Was this intentional?");
            }
            instance.entries.Add(new CustomVoidEntry(VoidItem!, TransformedItem!));
        }

        /// <summary>
        /// Creates a void item for more than one ItemDef. Should only be used AFTER ItemCatalog is initialized!
        /// </summary>
        /// <param name="VoidItem">Void item to be created</param>
        /// <param name="TransformedItems">Items to be transformed into the void item</param>
        public static void CreateTransformation(ItemDef? VoidItem, ItemDef[]? TransformedItems)
        {
            if (TransformedItems.Length <= 0)
            {
                instance.Logger.LogError("Sent in an empty array to CreateTransformation! Not initializing void item.");
                return;
            }
            foreach (ItemDef? itemDef in TransformedItems)
            {
                CreateTransformation(VoidItem, itemDef);
            }
        }

        /// <summary>
        /// Creates a void item for a single name of an item. Can be run at any point in time assuming you properly initialized your VoidItem.
        /// </summary>
        /// <param name="VoidItem"></param>
        /// <param name="TransformedItemName"></param>
        public static void CreateTransformation(ItemDef? VoidItem, string TransformedItemName)
        {
            if (!ValidateItem(VoidItem))
            {
                instance.Logger.LogError("Void item is not a valid item. Did you call CreateTransformation too early?");
                return;
            }
            if (string.IsNullOrEmpty(TransformedItemName))
            {
                instance.Logger.LogError("TransformedItemName is either null or empty. Initialization cancelled for void item: " + VoidItem.name + ".");
                return;
            }
            if (instance.tooLate)
            {
                instance.Logger.LogError("CreateTransformation has been called too late for void item: " + VoidItem.name + ".");
                return;
            }
            if (!VoidTiers.Contains(VoidItem!.tier))
            {
                instance.Logger.LogWarning(VoidItem.name + "'s item tier is not set to a void tier. Was this intentional?");
            }
            instance.entries.Add(new CustomVoidEntry(VoidItem!, TransformedItemName));
        }

        /// <summary>
        /// Creates a void item for multiple names of an item. Can be run at any point in time assuming you properly initialized your VoidItem.
        /// </summary>
        /// <param name="VoidItem"></param>
        /// <param name="TransformedItemNames"></param>
        public static void CreateTransformation(ItemDef? VoidItem, string[] TransformedItemNames)
        {
            if (TransformedItemNames.Length <= 0)
            {
                instance.Logger.LogError("Sent in an empty array to CreateTransformation! Not initializing void item.");
                return;
            }
            foreach (string itemName in TransformedItemNames)
            {
                CreateTransformation(VoidItem, itemName);
            }
        }
    }
}