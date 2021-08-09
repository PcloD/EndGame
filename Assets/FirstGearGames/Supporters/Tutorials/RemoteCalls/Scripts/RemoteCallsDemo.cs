using Mirror;
using System.Collections;
using UnityEngine;
namespace FirstGearGames.Mirrors.Assets.RemoteCalls
{
    public class RemoteCallsDemo : NetworkBehaviour
    {

        /// <summary>
        /// This method runs only if this client has authority.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            StartCoroutine(__RandomizeColor());
        }

        private void SetColor(Color32 color)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.color = color;
        }

        private IEnumerator __RandomizeColor()
        {
            WaitForSeconds wait = new WaitForSeconds(1f);

            while (true)
            {
                CmdChangeColor();
                yield return wait;
            }
        }

        [Command]
        private void CmdChangeColor()
        {
            Debug.Log("CmdChangeColor called.");
            //Pick a random color.
            Color32 color = Random.ColorHSV(0f, 1f, 1f, 1f, 0f, 1f, 1f, 1f);
            /* Set locally to show color changing on server. You likely
             * wouldn't update visuals on the server if it were also not
             * a client, or if it were headless. */
            SetColor(color);

            /* Send the color update to clients. */
            //Send to all clients.
            //RpcChangeColor(color);
            //Send to a specific client. Generally you will want ot use connectoToClient.
            NetworkIdentity identity = GetComponent<NetworkIdentity>();
            TargetChangeColor(identity.connectionToClient, color);
        }

        /// <summary>
        /// Useful for triggering an event on all clients which isn't neccesarily associated with a variable.
        /// 
        /// For example: if a player uses a spell and you wish to show the effects across all clients.
        /// You may call a command to the server to initiate the spell, the server will ensure the player
        /// can perform the spell, and afterwards will tell all clients to spawn the spell effect with
        /// ClientRpc.
        /// </summary>
        /// <param name="color"></param>
        [ClientRpc]
        private void RpcChangeColor(Color32 color)
        {
            Debug.Log("RpcChangeColor called.");
            SetColor(color);
        }

        /// <summary>
        /// Useful for triggering an event on a single client which isn't necessarily associated with a variable.
        /// 
        /// For example: if as a player you wanted to 'poke' another player by clicking on them. You could get the
        /// NetworkIdentity component on the other player, pass it into a command to the server requesting the poke.
        /// The server would then use the connectionToClient property on the NetworkIdentity through a TargetRpc to
        /// tell the other player they have been poked.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="color"></param>
        [TargetRpc]
        private void TargetChangeColor(NetworkConnection conn, Color32 color)
        {
            Debug.Log("TargetChangeColor called.");
            SetColor(color);
        }

    }


}