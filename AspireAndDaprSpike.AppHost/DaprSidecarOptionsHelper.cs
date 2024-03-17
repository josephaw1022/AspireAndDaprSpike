using Aspire.Hosting.Dapr;

namespace AspireAndDaprSpike.AppHost
{
    public static class DaprSidecarOptionsHelper
    {
        public static DaprSidecarOptions CreateDaprSidecarOptions(string appId)
        {
            return new DaprSidecarOptions
            {
                AppHealthThreshold = 20,
                AppId = appId,
            };
        }
    }
}
