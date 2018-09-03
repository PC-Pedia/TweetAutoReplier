using System.Net.NetworkInformation;

namespace TwitterClient.Helpers
{
    public static class IPHelper
    {
        public static bool IsInternetAvailable { get { return isInternetAvailable(); } }
        private static bool isInternetAvailable()
        {
            try
            {
                byte[] buffer = new byte[32];
                return (new Ping().Send("google.com", 1000, buffer, new PingOptions()).Status == IPStatus.Success);
            }
            catch
            {
                return false;
            }
        }
    }
}
