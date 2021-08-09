using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.InteractingSceneObjects
{

    [RequireComponent(typeof(UsableIdAssigner))]
    public class SlidingDoor : NetworkBehaviour, INetworkUsable
    {

        #region Serialized.
        [SerializeField]
        private Transform _door = null;
        /// <summary>
        /// Y position of door when closed.
        /// </summary>
        [Tooltip("Y position of door when closed.")]
        [SerializeField]
        private float _closedHeight = 1f;
        /// <summary>
        /// Y position of door when open.
        /// </summary>
        [Tooltip("Y position of door when open.")]
        [SerializeField]
        private float _openHeight = 1f;
        #endregion

        #region Private.
        /// <summary>
        /// True if the door is open.
        /// </summary>
        private bool _open = false;
        /// <summary>
        /// Id of this usable. Assigned at runtime.
        /// </summary>
        [SyncVar]
        private int _id;
        #endregion

        private void Update()
        {
            MoveDoor();
        }

        /// <summary>
        /// Move door to proper height.
        /// </summary>
        private void MoveDoor()
        {
            if (!base.isServer)
                return;

            float y = (_open) ? _openHeight : _closedHeight;
            Vector3 goal = new Vector3(_door.position.x, y, _door.position.z);

            _door.position = Vector3.MoveTowards(_door.position, goal, 3f * Time.deltaTime);
        }

        #region INetworkUsable.
        /// <summary>
        /// Use this object. Switches open state.
        /// </summary>
        public void Use()
        {
            if (base.isServer)
            { 
                _open = !_open;
            }            
            
            if (base.isClient)
            {
                //Run special client stuff here.
            }
        }
        /// <summary>
        /// Return NetworkIdentity on this object.
        /// </summary>
        /// <returns></returns>
        public NetworkIdentity GetNetworkIdentity()
        {
            return base.netIdentity;
        }
        /// <summary>
        /// Sets the Id for this usable.
        /// </summary>
        /// <param name="value"></param>
        public void SetId(int value)
        {
            _id = value;
        }
        /// <summary>
        /// gets the Id for this usable.
        /// </summary>
        /// <returns></returns>
        public int GetId()
        {
            return _id;
        }
        #endregion

    }


}