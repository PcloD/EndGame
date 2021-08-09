using Mirror;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0618, CS0672
namespace FirstGearGames.Mirrors.InitializingLocalPlayers
{
    public class HealthBarUI : MonoBehaviour
    {
        #region Serialized.
        /// <summary>
        /// Image used as fill to show players health percent.
        /// </summary>
        [Tooltip("Image used as fill to show players health percent.")]
        [SerializeField]
        private Image _healthFillbar = null;
        #endregion

        #region Private.
        /// <summary>
        /// HealthPercent script on the local player.
        /// </summary>
        private HealthPercent _playerHealthPercent;
        #endregion

        #region FindType
        public void SetPlayerHealthPercent(HealthPercent hp) { }
        #endregion

        private void Awake()
        {
            /* Since we don't know if the event has fired or not
             * prior to this object spawning, call PlayerUpdated
             * immediately. */
            PlayerUpdated(ClientScene.localPlayer);
            //Listen for additional local player updates.
            LocalPlayerAnnouncer.OnLocalPlayerUpdated += PlayerUpdated;
        }
        private void Update()
        {
            if (_playerHealthPercent == null)
                return;

            _healthFillbar.fillAmount = _playerHealthPercent.CurrentPercent;
        }
        private void OnDestroy()
        {
            //Remove the event listener when this object is destroyed.
            LocalPlayerAnnouncer.OnLocalPlayerUpdated -= PlayerUpdated;
        }

        /// <summary>
        /// Received when the local player is updated.
        /// </summary>
        private void PlayerUpdated(NetworkIdentity localPlayer)
        {
            if (localPlayer != null)
                _playerHealthPercent = localPlayer.GetComponent<HealthPercent>();

            /* If your script relied entirely on local player existing
             * you could consider disabling the script when the
             * local player is null as shown below. */
            this.enabled = (localPlayer != null);
        }
    }
}