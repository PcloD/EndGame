using UnityEngine;

namespace FirstGearGames.Mirrors.SyncingProjectiles
{

    public class Projectile : MonoBehaviour
    {
        #region Serialized.
        /// <summary>
        /// How quickly to move.
        /// </summary>
        [Tooltip("How quickly to move.")]
        [SerializeField]
        private float _moveRate = 7f;
        #endregion

        #region Private.
        /// <summary>
        /// Time to destroy this projectile.
        /// </summary>
        private float _destroyTime = -1f;
        /// <summary>
        /// Distance remaining to catch up.
        /// </summary>
        private float _catchupDistance = 0f;
        #endregion

        private void Update()
        {
            //Not initialized.
            if (_destroyTime < 0)
                return;

            float moveValue = _moveRate * Time.deltaTime;
            float catchupValue = 0f;

            //Apply catchup time.
            if (_catchupDistance > 0f)
            {
                float step = (_catchupDistance * Time.deltaTime);
                //Subtract step from catchup distance to eliminate traveled distance.
                _catchupDistance -= step;

                catchupValue = step;

                if (_catchupDistance < (moveValue * 0.1f))
                {
                    catchupValue += _catchupDistance;
                    _catchupDistance = 0f;
                }
            }

            //Move straight up.
            transform.position += Vector3.up * (moveValue + catchupValue);

            if (Time.time > _destroyTime)
                Destroy(gameObject);
        }

        public void Initialize(float duration)
        {
            _catchupDistance = (duration * _moveRate);
            //Destroy projectile after 5 seconds.
            _destroyTime = Time.time + 5f;
        }

    }

}