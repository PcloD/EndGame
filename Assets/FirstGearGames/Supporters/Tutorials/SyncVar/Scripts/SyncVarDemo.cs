using Mirror;
using System.Collections;
using UnityEngine;

namespace FirstGearGames.Mirrors.Assets.SyncVars
{
    public class SyncVarDemo : NetworkBehaviour
    {
        //[SyncVar]
        [SyncVar(hook = nameof(SetColor))]
        private Color32 _color = Color.red;

        /// <summary>
        /// This method runs on the server. It may also run on a client acting as the host.
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartServer();
            StartCoroutine(__RandomizeColor());
        }

        private void SetColor(Color32 oldColor, Color32 newColor)
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.color = newColor;
        }

        private IEnumerator __RandomizeColor()
        {
            WaitForSeconds wait = new WaitForSeconds(2f);

            while (true)
            {
                yield return wait;

                /* Set the _color field to a random color.
                 * Notice the color is not actually changing
                 * in the servers game view. This is because
                 * the hook does not call on the server, so we are
                 * only updating the field, not the sprite renderer as well. */
                _color = Random.ColorHSV(0f, 1f, 1f, 1f, 0f, 1f, 1f, 1f);
            }

        }


    }


}