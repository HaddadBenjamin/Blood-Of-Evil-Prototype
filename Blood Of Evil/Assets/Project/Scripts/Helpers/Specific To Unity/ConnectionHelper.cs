using UnityEngine;

namespace BloodOfEvil.Helpers
{
    public static class ConnectionHelper
    {
        /// <summary>
        /// Renvoie si l'utilisateur est connecté à internet.
        /// </summary>
        public static bool IsConnectedToInternet()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        /// <summary>
        /// Renvoie si l'utilisateur a une connection rapide, c'est à dire si il est connecté par WIFI ou par cable.
        /// </summary>
        public static bool IsConnectedToInternetByWifiOrCable()
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }

        /// <summary>
        /// Renvoie si l'utilisateur a une connection lente.
        /// </summary>
        public static bool IsConnectedByASlowNetworkConnectionDevice()
        {
            return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
        }
    }
}