using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.IsLocalPlayers
{
    public class SpawnPet : NetworkBehaviour
    {
        [SerializeField]
        private bool _enabled = true;
        [SerializeField]
        private GameObject _petPrefab = null;

        private void Awake()
        {
            if (!_enabled)
                DestroyImmediate(this);
        }

        public override void OnStartClient()
        {

            base.OnStartClient();
            //If has authority then spawn pet.
            if (base.hasAuthority)
                CmdSpawnPet();
        }

        [Command]
        private void CmdSpawnPet()
        {
            GameObject petInstance = Instantiate(_petPrefab);
            NetworkServer.Spawn(petInstance, base.connectionToClient);
        }
    }

}