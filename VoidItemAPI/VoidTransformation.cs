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

    public class VoidItemModification
    {
        public ItemDef VoidItem;
        public ItemDef CurrentTransformedItem;
        public ItemDef NewTransformation;
        public string VoidItemName;
        public string CurrentTransformedItemName;
        public string NewTransformationName;
        public ModificationType modification;
        public CustomVoidEntry.TransformType transformType;
        public enum ModificationType
        {
            Modify,
            Remove
        }

        public VoidItemModification(ItemDef VoidItem, ItemDef CurrentTransformedItem, ItemDef? NewTransformation, ModificationType modification)
        {
            this.VoidItem=VoidItem;
            this.CurrentTransformedItem=CurrentTransformedItem;
            if (modification == ModificationType.Modify)
            {
                this.NewTransformation = NewTransformation!;
            }
            this.modification = modification;
            this.transformType = CustomVoidEntry.TransformType.Def;
        }

        public VoidItemModification(string VoidItemName, string CurrentTransformedItemName, string? NewTransformationName, ModificationType modification)
        {
            this.VoidItemName=VoidItemName;
            this.CurrentTransformedItemName=CurrentTransformedItemName;
            if (modification == ModificationType.Modify)
            {
                this.NewTransformationName=NewTransformationName!;
            }
            this.modification = modification;
            this.transformType = CustomVoidEntry.TransformType.Name;
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

        public static void ModifyTransformation(ItemDef? VoidItem, ItemDef? CurrentTransformedItem, ItemDef? ItemToTransformInto, VoidItemModification.ModificationType modification)
        {
            if (!ValidateItem(VoidItem))
            {
                instance.Logger.LogError("Void item ItemDef sent to ModifyTransformation is not valid. You are most likely calling the method too early. Aborting modification.");
                return;
            }
            if (!ValidateItem(CurrentTransformedItem))
            {
                instance.Logger.LogError("The current item transformation sent to ModifyTransformation for " + VoidItem!.name + " is not valid. Aborting modification.");
                return;
            }
            if (instance.tooLate)
            {
                instance.Logger.LogError("ModifyTransformation was called too late for " + VoidItem.name + ", aborting modification.");
                return;
            }
            if (modification == VoidItemModification.ModificationType.Modify)
            {
                if (!ValidateItem(ItemToTransformInto))
                {
                    instance.Logger.LogError("Modification type for void item " + VoidItem!.name + " is set to modify, yet the new ItemDef transformation is not valid. Aborting modification.");
                    return;
                }
                instance.modifications.Add(new VoidItemModification(VoidItem!, CurrentTransformedItem!, ItemToTransformInto!, modification));
                return;
            }
            instance.modifications.Add(new VoidItemModification(VoidItem!, CurrentTransformedItem!, null, modification));
        }

        public static void ModifyTransformation(string VoidItemName, string CurrentTransformedItemName, string ItemToTransformIntoName, VoidItemModification.ModificationType modification)
        {
            if (string.IsNullOrEmpty(VoidItemName))
            {
                instance.Logger.LogError("Void item name sent into ModifyTransformation is null or empty. Aborting modification.");
                return;
            }
            if (string.IsNullOrEmpty(CurrentTransformedItemName))
            {
                instance.Logger.LogError("Current Transformed Item Name sent in for " + VoidItemName + " is null or empty. Aborting modification.");
                return;
            }
            if (instance.tooLate)
            {
                instance.Logger.LogError("ModifyTransformation for " + VoidItemName + " was called too late, aborting modification.");
                return;
            }
            if (modification == VoidItemModification.ModificationType.Modify)
            {
                if (string.IsNullOrEmpty(ItemToTransformIntoName))
                {
                    instance.Logger.LogError("Modification type for " + VoidItemName + " is set to Modify, yet the new transformation name is null. Aborting modification.");
                    return;
                }
                instance.modifications.Add(new VoidItemModification(VoidItemName, CurrentTransformedItemName, ItemToTransformIntoName!, modification));
            }
            instance.modifications.Add(new VoidItemModification(VoidItemName, CurrentTransformedItemName, null, modification));
        }
    }
}