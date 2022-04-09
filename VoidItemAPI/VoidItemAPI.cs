using System;
using HarmonyLib;
using UnityEngine;
using RoR2;
using System.Collections.Generic;
using BepInEx;

namespace VoidItemAPI
{
    [BepInPlugin(MODGUID, MODNAME, MODVERSION)]
    public class VoidItemAPI : BaseUnityPlugin
    {
        public const string MODNAME = "VoidItemAPI";
        public const string MODVERSION = "1.0.0";
        public const string MODGUID = "com.RumblingJOSEPH.VoidItemAPI";

        public static BepInEx.Logging.ManualLogSource Logger;
        public Dictionary<ItemDef, ItemDef[]> voidToTransformations = new Dictionary<ItemDef, ItemDef[]>();

        private void Awake()
        {
            Logger = base.Logger;


        }

        public static void CreateTransformation(ItemDef VoidItem, bool requiresDLC = true, params ItemDef[] transformedItems)
        {
            if (!VoidItem) { Logger.LogError(VoidItem.name + " is not a valid item. Ensure it has been created properly"); }
        }
    }
}
