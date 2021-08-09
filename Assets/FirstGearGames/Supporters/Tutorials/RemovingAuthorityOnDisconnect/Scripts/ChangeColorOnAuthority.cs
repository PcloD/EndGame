using Mirror;
using UnityEngine;


namespace FirstGearGames.Mirrors.RemoveAuthorityOnDisconnect
{

    public class ChangeColorOnAuthority : NetworkBehaviour
    {
        /// <summary>
        /// SpriteRenderer on this object.
        /// </summary>
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Called when authority is gained on this object.
        /// </summary>
        public override void OnStartAuthority()
        {
            base.OnStopAuthority();
            _spriteRenderer.color = Color.blue;
        }

        /// <summary>
        /// Called when authority is lost on this object.
        /// </summary>
        public override void OnStopAuthority()
        {
            base.OnStopAuthority();
            _spriteRenderer.color = Color.white;
        }

    }


}