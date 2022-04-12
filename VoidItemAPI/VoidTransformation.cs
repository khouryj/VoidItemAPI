using static VoidItemAPI.VoidItemAPI;
using RoR2;

namespace VoidItemAPI
{
    public static class VoidTransformation
    {
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
    }
}
