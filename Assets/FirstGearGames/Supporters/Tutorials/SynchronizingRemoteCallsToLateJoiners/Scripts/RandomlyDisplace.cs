using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.SynchronizingRemoteCallsToLateJoiners
{


    public class RandomlyDisplace : NetworkBehaviour
    {

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            transform.position = Vector3.zero + new Vector3(
                Random.Range(-4f, 4f),
                Random.Range(-4f, 4f),
                0f);
        }
    }


}