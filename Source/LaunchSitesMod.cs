using RimWorld;
using Verse;
using HarmonyLib;
using RimWorld.Planet;

namespace LaunchSitesExpire
{
    [StaticConstructorOnStartup]
    public static class LaunchSitesMod
    {
        static Harmony harmony;
        static LaunchSitesMod()
        {
            harmony = new Harmony("cass.launchsitesexpire");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(MapParent), nameof(MapParent.Abandon))]
    class MapParent_Abandon_Patch
    {
        static void Prefix(out (bool wasGravshipLaunch, bool wasLandmark) __state, MapParent __instance, ref bool wasGravshipLaunch)
        {
            __state = (wasGravshipLaunch, wasLandmark: __instance.Map.TileInfo.Landmark != null);

            wasGravshipLaunch = false;
        }

        static void Postfix((bool wasGravshipLaunch, bool wasLandmark) __state, MapParent __instance)
        {
            if(__state.wasGravshipLaunch && __instance.Tile.LayerDef == PlanetLayerDefOf.Surface)
            {
                WorldObject worldObject = WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.GravshipLaunch);
                worldObject.Tile = __instance.Tile;
                worldObject.SetFaction(__instance.Faction);
                worldObject.GetComponent<TimeoutComp>().StartTimeout(1800000);
                Find.WorldObjects.Add(worldObject);
            }
        }
    }
}
