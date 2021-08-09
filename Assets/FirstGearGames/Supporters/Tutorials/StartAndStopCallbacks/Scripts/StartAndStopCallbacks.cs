using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.StartAndStopCalls
{

    public class StartAndStopCallbacks : NetworkBehaviour
    {

        private void Awake() { Debug.Log("Awake"); }

        #region Start & Stop Callbacks   
        /// <summary>
        /// This is invoked for NetworkBehaviour objects when they become active on the server.
        /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
        /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
        /// </summary>
        public override void OnStartServer() { Debug.Log("OnStartServer"); }

        /// <summary>
        /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
        /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
        /// <para>When <see cref="NetworkIdentity.AssignClientAuthority"/> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnection parameter included, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStartAuthority() { Debug.Log("OnStartAuthority"); }

        /// <summary>
        /// Called on every NetworkBehaviour when it is activated on a client.
        /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
        /// </summary>
        public override void OnStartClient() { Debug.Log("OnStartClient"); }

        /// <summary>
        /// Called when the local player object has been set up.
        /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
        /// </summary>
        public override void OnStartLocalPlayer() { Debug.Log("OnStartLocalPlayer"); }

        /// <summary>
        /// This is invoked on behaviours when authority is removed.
        /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
        /// </summary>
        public override void OnStopAuthority() { Debug.Log("OnStopAuthority"); }
        #endregion

        private void Start() { Debug.Log("Start"); }


        /* Client start cycle.
        * - Awake
        *                  (No network data is known.
        *                  Initialize any components used for all clients and/or the server which don't rely on the network.)
        * - OnStartAuthority
        *                  (Network data is known.
        *                  Initialize components which are only needed for owning that also rely on the network.)
        * - OnStartClient  
        *                  (Network data is known.
        *                  Initialize components which are needed by all clients that also rely on the network.)
        * - OnStartLocalPlayer
        *                  (Called if this object is the 'player' object being spawned via "Player Prefab" in the NetworkManager.
        *                  Can also be called when manually adding, or replacing the 'player' object.)
        * - Start
        *                  (Network data is known.
        *                  Usually called last, but order is not guaranteed.)
        */

        /* Server start cycle.
        * - Awake
        *                  (No network data is known.
        *                  Initialize any components used for all clients and/or the server which don't rely on the network.)
        * - OnStartServer
        *                  (Network data is known.
        *                  Initialize components which are needed by the server that rely on the network.
        * - Start
        *                  (Network data is known.
        *                  Usually called last, but order is not guaranteed.)
        */

        /* ClientHost start cycle.
        * - Awake
        *                  (No network data is known.
        *                  Initialize any components used for all clients and/or the server which don't rely on the network.)
        * - OnStartServer
        *                  (Network data is known.
        *                  Initialize components which are needed by the server that rely on the network.
        * - OnStartAuthority
        *                  (Network data is known.
        *                  Initialize components which are only needed for owning client that also rely on the network.)
        * - OnStartClient  
        *                  (Network data is known.
        *                  Initialize components which are needed by all clients that also rely on the network.)
        * - OnStartLocalPlayer
        *                  (Called if this object is the 'player' object being spawned via "Player Prefab" in the NetworkManager.)
        * - Start
        *                  (Network data is known.
        *                  Usually called last, but order is not guaranteed.)
        */

    }



}