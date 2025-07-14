using RimWorld;

namespace LaunchSitesExpire
{
    [DefOf]
    public static class ModDefOf
    {
        static ModDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ModDefOf));
        }

        public static WorldObjectDef GravshipLaunchLandmark;
    }
}
