using Mirror;
using UnityEngine;


namespace FirstGearGames.Mirrors.InteractingSceneObjects
{

    [RequireComponent(typeof(UsableIdAssigner))]
    public class TeleportingCube : NetworkBehaviour, INetworkUsable
    {
        #region Private.
        /// <summary>
        /// Id of this usable. Assigned at runtime.
        /// </summary>
        [SyncVar]
        private int _id;
        #endregion

        #region INetworkUsable.
        public void SetId(int value) { _id = value; }
        public int GetId() { return _id; }
        public NetworkIdentity GetNetworkIdentity() { return base.netIdentity; }
        public void Use()
        {
            if (base.isServer)
                    transform.position = new Vector3(Random.Range(-3f, 3f), transform.position.y, Random.Range(-3f, 3f));
        }
        #endregion
    }


}