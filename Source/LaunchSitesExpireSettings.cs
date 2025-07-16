using UnityEngine;
using Verse;

namespace LaunchSitesExpire
{
    public class LaunchSitesExpireSettings : ModSettings
    {
        public int launchSiteTimeoutDays = 30;
        public bool landmarkSitesCanExpire = false;

        public bool landmarkCampsCanExpire = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref launchSiteTimeoutDays, nameof(launchSiteTimeoutDays), 30);
            Scribe_Values.Look(ref landmarkSitesCanExpire, nameof(landmarkSitesCanExpire), false);

            Scribe_Values.Look(ref landmarkCampsCanExpire, nameof(landmarkCampsCanExpire), true);
            base.ExposeData();
        }

        public void DoWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            launchSiteTimeoutDays = (int)listing.SliderLabeled(Strings.Translated.LSE_LaunchSiteTimeoutDaysLabel(launchSiteTimeoutDays), launchSiteTimeoutDays, 1, 300, tooltip: Strings.Translated.LSE_LaunchSiteTimeoutDaysTooltip);
            listing.CheckboxLabeled(Strings.Translated.LSE_LandmarkSitesCanExpireLabel, ref landmarkSitesCanExpire, tooltip: Strings.Translated.LSE_LandmarkSitesCanExpireTooltip);

            listing.Gap();

            listing.CheckboxLabeled(Strings.Translated.LSE_LandmarkCampsCanExpireLabel, ref landmarkCampsCanExpire, tooltip: Strings.Translated.LSE_LandmarkCampsCanExpireTooltip);

            listing.End();
        }
    }
}
