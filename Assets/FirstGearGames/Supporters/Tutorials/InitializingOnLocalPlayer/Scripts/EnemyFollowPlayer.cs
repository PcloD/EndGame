using Mirror;
using UnityEngine;

#pragma warning disable CS0618, CS0672
namespace FirstGearGames.Mirrors.InitializingLocalPlayers
{

    public class EnemyFollowPlayer : MonoBehaviour
    {
        private HealthPercent _playerHealthPercent;

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
            //Don't check if player health percent is null.
            if (_playerHealthPercent == null)
                return;
            //Only move to player if health percent is below half.
            if (_playerHealthPercent.CurrentPercent < 0.5f)
                transform.position = Vector3.MoveTowards(transform.position, _playerHealthPercent.transform.position, 0.5f * Time.deltaTime);
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