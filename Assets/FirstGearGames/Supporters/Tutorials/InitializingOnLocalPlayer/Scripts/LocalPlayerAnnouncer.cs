using Mirror;
using System;
using UnityEngine;
namespace FirstGearGames.Mirrors.InitializingLocalPlayers
{
    public class LocalPlayerAnnouncer : NetworkBehaviour
    {

        #region Public.
        /// <summary>
        /// Dispatched when the local player changes, providing the new localPlayer.
        /// </summary>
        public static event Action<NetworkIdentity> OnLocalPlayerUpdated;
        #endregion

        #region Start/Destroy
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            OnLocalPlayerUpdated?.Invoke(base.netIdentity);
        }

        //private void OnDestroy()
        //{
        //    if (base.isLocalPlayer)
        //        OnLocalPlayerUpdated?.Invoke(null);
        //}
        #endregion

        #region OnEnable/Disable.
        private void OnEnable()
        {
            if (base.isLocalPlayer)
                OnLocalPlayerUpdated?.Invoke(base.netIdentity);
        }
        private void OnDisable()
        {
            if (base.isLocalPlayer)
                OnLocalPlayerUpdated?.Invoke(null);
        }
        #endregion

    }

}