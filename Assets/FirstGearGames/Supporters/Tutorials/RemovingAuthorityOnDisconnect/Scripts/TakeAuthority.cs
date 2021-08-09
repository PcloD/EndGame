using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.RemoveAuthorityOnDisconnect
{


    public class TakeAuthority : NetworkBehaviour
    {

        private void Update()
        {
            RequestOwnershipOnClick();
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
                    CmdRequestAuthority(id);
            }
        }


        /// <summary>
        /// Gives authority of the passed in NetworkIdentity to the owner of this object.
        /// </summary>
        /// <param name="otherId"></param>
        [Command]
        private void CmdRequestAuthority(NetworkIdentity otherId)
        {
            otherId.AssignClientAuthority(base.connectionToClient);
        }


    }


}