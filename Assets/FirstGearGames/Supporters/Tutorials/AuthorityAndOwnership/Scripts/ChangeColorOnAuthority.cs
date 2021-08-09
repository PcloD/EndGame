using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.AuthorityAndOwnerships
{ 
public class ChangeColorOnAuthority : NetworkBehaviour
{

    /// <summary>
    /// Called on a client when they receive authority over an object.
    /// </summary>
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        GetComponent<SpriteRenderer>().color = Color.blue;
    }

}

}