using Mirror;
using UnityEngine;
#pragma warning disable CS0618, CS0672
namespace FirstGearGames.Mirrors.InitializingLocalPlayers
{
    public class CameraController : MonoBehaviour
    {
        #region Private.
        /// <summary>
        /// Transform for the local player.
        /// </summary>
        private Transform _playerTransform;
        #endregion

        private void Awake()
        {

            LocalPlayerAnnouncer_OnLocalPlayerUpdated(ClientScene.localPlayer);
            LocalPlayerAnnouncer.OnLocalPlayerUpdated += LocalPlayerAnnouncer_OnLocalPlayerUpdated;
        }

        private void OnDestroy()
        {
            LocalPlayerAnnouncer.OnLocalPlayerUpdated -= LocalPlayerAnnouncer_OnLocalPlayerUpdated;
        }

        private void LocalPlayerAnnouncer_OnLocalPlayerUpdated(NetworkIdentity obj)
        {
            if (obj != null)
                _playerTransform = obj.transform;
        }


        private void Update()
        {
            if (_playerTransform == null)
                return;

            Vector3 target = new Vector3(_playerTransform.position.x, _playerTransform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, target, 2f * Time.deltaTime);
        }
    }


}