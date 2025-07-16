using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace LaunchSitesExpire
{
    [StaticConstructorOnStartup]
    public static class LaunchSitesExpireStartup
    {
        static LaunchSitesExpireStartup()
        {
            LaunchSitesExpireMod.harmony = new Harmony("cass.launchsitesexpire");
            LaunchSitesExpireMod.harmony.PatchAll();
        }
    }

    public class LaunchSitesExpireMod : Mod
    {
        public static Harmony harmony;
        public static LaunchSitesExpireSettings settings;

        public LaunchSitesExpireMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<LaunchSitesExpireSettings>();
        }
        public override string SettingsCategory() => "Launch Sites Expire";
        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoWindowContents(inRect);
            base.DoSettingsWindowContents(inRect);
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
            if (__state.wasGravshipLaunch && __instance.Tile.LayerDef == PlanetLayerDefOf.Surface)
            {
                if (__state.wasLandmark && !LaunchSitesExpireMod.settings.landmarkSitesCanExpire)
                {
                    WorldObject worldObject = WorldObjectMaker.MakeWorldObject(ModDefOf.GravshipLaunchLandmark);
                    worldObject.Tile = __instance.Tile;
                    worldObject.SetFaction(__instance.Faction);
                    Find.WorldObjects.Add(worldObject);
                }
                else
                {
                    WorldObject worldObject = WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.GravshipLaunch);
                    worldObject.Tile = __instance.Tile;
                    worldObject.SetFaction(__instance.Faction);
                    worldObject.GetComponent<TimeoutComp>().StartTimeout(60000 * LaunchSitesExpireMod.settings.launchSiteTimeoutDays);
                    Find.WorldObjects.Add(worldObject);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Camp), nameof(Camp.Notify_MyMapRemoved))]
    class Camp_Notify_MyMapRemoved_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            //Toggle for if abandoned landmarks can spawn
            var landmarkMethod = AccessTools.PropertyGetter(typeof(Tile), nameof(Tile.Landmark));
            codeMatcher.MatchStartForward(CodeMatch.Calls(landmarkMethod), CodeMatch.Branches("landmark-branch"))
            .ThrowIfInvalid("Couldn't match place to insert camp landmark toggle.")
            .InsertAfterAndAdvance(CodeInstruction.Call(() => CanCreateAbandonedLandmarks()))
            .InsertAndAdvance(codeMatcher.NamedMatch("landmark-branch"));


            //Override the duration of the timeout
            var durationMatch = CodeMatch.LoadsConstant(1800000);
            var timeoutMethod = AccessTools.Method(typeof(TimeoutComp), nameof(TimeoutComp.StartTimeout));
            codeMatcher.Start()
            .MatchStartForward(durationMatch, CodeMatch.Calls(timeoutMethod))
            .ThrowIfInvalid("Couldn't match timeout duration")
            .RemoveInstruction()
            .Insert(CodeInstruction.Call(() => AbandonedCampTimeoutDuration()));

            return codeMatcher.Instructions();
        }

        static bool CanCreateAbandonedLandmarks()
        {
            return !LaunchSitesExpireMod.settings.landmarkCampsCanExpire;
        }

        static int AbandonedCampTimeoutDuration()
        {
            return 60000 * LaunchSitesExpireMod.settings.abandonedCampTimeoutDays;
        }
    }
}
