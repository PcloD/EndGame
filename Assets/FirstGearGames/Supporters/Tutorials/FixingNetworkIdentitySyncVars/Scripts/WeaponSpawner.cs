using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.FixingNetworkIdentityReferences
{
    public class WeaponSpawner : NetworkBehaviour
    {
        /// <summary>
        /// Object to snap weapon to.
        /// </summary>
        [SerializeField]
        private Transform _weaponHolder = null;
        /// <summary>
        /// Prefab to spawn.
        /// </summary>
        [SerializeField]
        private GameObject _swordPrefab = null;
        /// <summary>
        /// Spawned weapon.
        /// </summary>
        [SyncVar]
        private NetworkIdentityReference _spawnedWeapon = new NetworkIdentityReference();

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            Vector2 offset = Random.insideUnitCircle * 3f;
            transform.position += new Vector3(offset.x, offset.y, 0f);
        }

        private void Update()
        {
            //Move weapon to holder.
            if (_spawnedWeapon.Value != null)
                _spawnedWeapon.Value.transform.position = _weaponHolder.position;

            if (base.hasAuthority)
            {
                //Check for move input
                float hor = Input.GetAxis("Horizontal");
                transform.position += new Vector3(hor * Time.deltaTime * 3f, 0f, 0f);

                //Check to spawn weapon.
                if (Input.GetKeyDown(KeyCode.S))
                    CmdSpawnWeapon();

            }
        }

        [Command]
        private void CmdSpawnWeapon()
        {
            if (_spawnedWeapon.Value != null)
                return;

            GameObject go = Instantiate(_swordPrefab);
            NetworkServer.Spawn(go);

            _spawnedWeapon = new NetworkIdentityReference(go.GetComponent<NetworkIdentity>());
        }
    }


}