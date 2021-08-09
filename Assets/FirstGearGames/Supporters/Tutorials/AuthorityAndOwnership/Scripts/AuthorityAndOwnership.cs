using Mirror;
using UnityEngine;

#pragma warning disable CS0618, CS0672, CS0649
namespace FirstGearGames.Mirrors.AuthorityAndOwnerships
{
    public class AuthorityAndOwnership : NetworkBehaviour
    {
        /* Authority is of extreme importance with networking and it's vital
         * to understand how authority works.
         * 
         * Authority and ownership are ultimately the same thing. When a player has
         * authority over a networked object they are considered the owner of that object.
         * The server will always have authority over any networked object, though the
         * player may not.
         * 
         * Authority allows a player to send Commands to the server, which is a common way
         * for a player to talk to the server. Commands are often used to request action by
         * the server. My Remote Calls video will cover commands in more detail.
         * 
         * Generally authority represents which player has the power to modify an object, such as
         * your character, or an item being held. But the uses are limitless.
         * 
         * When the player prefab is automatically spawned in using the NetworkManager,
         * authority is also granted to the player in which the prefab is being spawned for.
         * 
         * There are also several other ways to receive authority; below are some common options. */

        /// <summary>
        /// Prefab to respawn with.
        /// </summary>
        private GameObject _somePrefab;

        private void Update()
        {
            bool playerDead = false;
            /* I'm using base.hasAuthority to check for authority before calling
             * RequestRespawn(). */
            if (playerDead && base.hasAuthority && base.isClient)
                CmdRequestRespawn();

            RequestOwnershipOnClick();
        }


        [Command]
        private void CmdRequestRespawn()
        {
            GameObject result = Instantiate(_somePrefab);
            //Instantiate over the server giving authority to the client which owns this script.
            NetworkServer.Spawn(result, base.connectionToClient);
        }


        /// <summary>
        /// Traces for an object in the scene then tries to take ownership of it.
        /// </summary>
        private void RequestOwnershipOnClick()
        {
            /* This logic ultimately sends a command. Since commands cannot be
             * sent without authority there is no reason to continue if the
             * client does not have authority. */
            if (!base.hasAuthority)
                return;
            //Mouse not pressed, exit method.
            if (!Input.GetKeyDown(KeyCode.Mouse0))
                return;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                NetworkIdentity id = hit.collider.GetComponent<NetworkIdentity>();
                /* If other object has a NetworkIdentity and client doesn't already
                 * own that object then request authority for it. */
                if (id != null && !id.hasAuthority)
                {
                    Debug.Log("Sending request authority for " + hit.collider.gameObject.name);
                    CmdRequestAuthority(id);
                }
            }
        }


        [Command]
        private void CmdRequestAuthority(NetworkIdentity otherId)
        {
            Debug.Log("Received request authority for " + otherId.gameObject.name);
            /* Let's assume this method is being run on PlayerA while
             * otherId belongs to PlayerB. We are telling the
             * NetworkIdentity on PlayerB to assign authority to
             * the NetworkIdentity that owns the object this
             * script runs on, which is PlayerA. */
            otherId.AssignClientAuthority(base.connectionToClient);
        }


    }


}