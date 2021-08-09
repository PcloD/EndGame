using Mirror;

namespace FirstGearGames.Mirrors.InitializingLocalPlayers
{

    public class NetworkSelfDestruct : NetworkBehaviour
    {

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            base.Invoke("CmdDestroyMe", 5f);
        }


        [Command]
        private void CmdDestroyMe()
        {
            NetworkServer.Destroy(gameObject);
        }
    }

}
