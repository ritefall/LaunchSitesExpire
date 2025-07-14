﻿using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections;
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

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions);

            codeMatcher.MatchStartForward(CodeMatch.IsLdarg(1))
                .ThrowIfInvalid("Could not find load argument 1 (wasGravshipLaunch)")
                .RemoveInstruction()
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldc_I4_0));

            return codeMatcher.Instructions();
        }
    }
}
