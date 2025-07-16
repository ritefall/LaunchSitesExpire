using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LaunchSitesExpire
{
    public static class Strings
    {
        public static class Translated
        {
            public static string LSE_LaunchSiteTimeoutDaysLabel (int days) { return "LSE_LaunchSiteTimeoutDaysLabel".Translate(days); }
            public static readonly string LSE_LaunchSiteTimeoutDaysTooltip = "LSE_LaunchSiteTimeoutDaysTooltip".Translate();
            public static readonly string LSE_LandmarkSitesCanExpireLabel = "LSE_LandmarkSitesCanExpireLabel".Translate();
            public static readonly string LSE_LandmarkSitesCanExpireTooltip = "LSE_LandmarkSitesCanExpireTooltip".Translate();

            public static readonly string LSE_LandmarkCampsCanExpireLabel = "LSE_LandmarkCampsCanExpireLabel".Translate();
            public static readonly string LSE_LandmarkCampsCanExpireTooltip = "LSE_LandmarkCampsCanExpireTooltip".Translate();
        }
    }
}
