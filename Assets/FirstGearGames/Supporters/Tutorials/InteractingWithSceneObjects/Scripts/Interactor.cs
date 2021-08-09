using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.InteractingSceneObjects
{

    public class Interactor : NetworkBehaviour
    {

        private void Update()
        {
            ClientUpdate();
        }

        private void ClientUpdate()
        {
            if (base.isServer && !base.isClient)
                return;
            if (!base.hasAuthority)
                return;

            if (Input.GetKeyDown(KeyCode.Mouse1))
                TryInteract();
        }


        /// <summary>
        /// Try to interact with nearby usables.
        /// </summary>
        private void TryInteract()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 3f);
            for (int i = 0; i < hits.Length; i++)
            {
                /* Look for network usable interface on the object.
                 * If found then call CmdUse passing in the objects
                 * NetworkIdentity component. */
                INetworkUsable[] usables = hits[i].GetComponents<INetworkUsable>();
                foreach (INetworkUsable usable in usables)
                {
                    if (usable != null)
                    {
                        /* Only use locally if not a client host. This is to prevent
                         * the isServer check from firing twice on Use. */
                        if (!base.isServer)
                            usable.Use();
                        CmdUse(usable.GetNetworkIdentity(), usable.GetId());
                    }
                }
            }
        }

        /// <summary>
        /// Uses the INetworkUsable on the NetworkIdentity passed in.
        /// </summary>
        /// <param name="netIdent"></param>
        [Command]
        private void CmdUse(NetworkIdentity netIdent, int id)
        {
            INetworkUsable[] usables = netIdent.gameObject.GetComponents<INetworkUsable>();
            for (int i = 0; i < usables.Length; i++)
            {
                if (usables[i].GetId() == id)
                    usables[i].Use();
            }
        }
    }


}