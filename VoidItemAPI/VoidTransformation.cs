using static VoidItemAPI.VoidItemAPI;
using RoR2;

namespace VoidItemAPI
{
    public class CustomVoidItem
    {
        public ItemDef? itemDef { get; set; }
        public VoidTransformation.TransformationType transformationType { get; set; }

        public CustomVoidItem(ItemDef? itemDef, VoidTransformation.TransformationType type)
        {
            this.itemDef = itemDef;
            transformationType = type;
        }
    }

    public static class VoidTransformation
    {
        public enum TransformationType
        {
            Modify,
            Remove
        };

        /// <summary>
        /// Creates a void item for a single item.
        /// </summary>
        /// <param name="VoidItem">Void item to be created</param>
        /// <param name="TransformedItem">Item to be transformed into the void item.</param>
        public static void CreateTransformation(ItemDef? VoidItem, ItemDef? TransformedItem) => instance.voidToTransformations.Add(VoidItem, new ItemDef[] { TransformedItem });

        /// <summary>
        /// Creates a void item for more than one item.
        /// </summary>
        /// <param name="VoidItem">Void item to be created</param>
        /// <param name="TransformedItems">Items to be transformed into the void item</param>
        public static void CreateTransformation(ItemDef? VoidItem, ItemDef[]? TransformedItems) => instance.voidToTransformations.Add(VoidItem, TransformedItems);

        /// <summary>
        /// Modify or remove existing transformations for VANILLA items.
        /// </summary>
        /// <param name="VoidItem">Void item to be modified or removed</param>
        /// <param name="TransformedItem">Item to be transformed. If removing an entry, use the item that it is paired with.</param>
        /// <param name="type">Transformation Types - Modify or Remove</param>
        public static void ModifyTransformation(ItemDef? VoidItem, ItemDef? TransformedItem, TransformationType type)
        {
            instance.modifyTransformations.Add(new CustomVoidItem(VoidItem, type), new ItemDef[] { TransformedItem });
        }
    }
}
