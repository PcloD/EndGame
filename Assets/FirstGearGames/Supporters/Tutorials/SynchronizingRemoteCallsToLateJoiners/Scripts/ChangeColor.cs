using Mirror;
using UnityEngine;


namespace FirstGearGames.Mirrors.SynchronizingRemoteCallsToLateJoiners
{

    public class ChangeColor : NetworkBehaviour
    {
        /// <summary>
        /// SpriteRenderer on this object.
        /// </summary>
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        #region Supporter code.
        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            if (initialState)
            {
                writer.WriteColor(_spriteRenderer.color);
            }
            return base.OnSerialize(writer, initialState);
        }
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                _spriteRenderer.color = reader.ReadColor();
            }
            base.OnDeserialize(reader, initialState);
        }
        #endregion

        private void Update()
        {
            if (base.hasAuthority)
            {
                //Every other frame and if C is held.
                if (Input.GetKeyDown(KeyCode.C))
                {
                    float r = Random.Range(0f, 1f);
                    float b = Random.Range(0f, 1f);
                    float g = Random.Range(0f, 1f);
                    _spriteRenderer.color = new Color(r, b, g, 1f);
                    CmdUpdateColor(_spriteRenderer.color);
                }
            }
        }

        /// <summary>
        /// Updates the sprite color on the server.
        /// </summary>
        [Command]
        private void CmdUpdateColor(Color c)
        {
            _spriteRenderer.color = c;
            RpcUpdateColor(c);
        }

        /// <summary>
        /// Updates the sprite color to clients.
        /// </summary>
        [ClientRpc]
        private void RpcUpdateColor(Color c)
        {
            //Don't need to update on server if client host.
            if (base.isServer)
                return;
            //Don't want to update for owner.
            if (base.hasAuthority)
                return;

            _spriteRenderer.color = c;
        }


    }


}