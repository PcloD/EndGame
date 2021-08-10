using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// class responsible for syncing Buffs/Debuffs across the network
/// </summary>
public class NetworkBuffHandler : NetworkBehaviour
{
    public struct DebuffData
    {
        public DebuffType DebuffType;
        public float Duration;
    }
    
}
