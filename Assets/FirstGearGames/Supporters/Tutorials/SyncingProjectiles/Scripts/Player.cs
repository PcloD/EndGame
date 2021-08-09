using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGearGames.Mirrors.SyncingProjectiles
{


    public class Player : NetworkBehaviour
    {

        #region Serialized.
        /// <summary>
        /// Projectile prefab.
        /// </summary>
        [Tooltip("Projectile prefab.")]
        [SerializeField]
        private Projectile _projectilePrefab = null;
        #endregion

        private void Update()
        {
            ClientUpdate();
        }

        private void ClientUpdate()
        {
            if (!base.isClient)
                return;

            Fire();
        }

        /// <summary>
        /// Checks to fire a projectile.
        /// </summary>
        private void Fire()
        {
            if (!base.hasAuthority)
                return;

            //Only fire when mouse 0 is pressed.
            if (!Input.GetKeyDown(KeyCode.Mouse0))
                return;

            /* Only show locally if not server.
             * This is to prevent duplicate projectiles
             * as client host. */
            if (!base.isServer)
            {
                //Spawn the projectile locally.
                Projectile p = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
                //Initialize with a 0f catch up duration.
                p.Initialize(0f);
            }

            /* Ask server to fire a projectile using
             * this transforms position, and current
             * network time. Since network time is synchronized
             * it can be used to determine how long it took 
             * the command to reach the server.  */
            CmdFire(transform.position, NetworkTime.time);
        }

        /// <summary>
        /// Fires over the network.
        /// </summary>
        [Command]
        private void CmdFire(Vector3 position, double networkTime)
        {
            /* Determine how much time has passed between when the client
             * called the command and when it was received. */
            double timePassed = NetworkTime.time - networkTime;

            /* Instantiate the projectile on the server so that it may
             * register collision, and continue to act as server authoritive.
             * It's important to instantiate at the position passed in so that 
             * the projectile is spawned in the same position on all clients, regardless
             * of their appeared position. Visually the result would be about the same as
             * if spectators were the owner themself. */
            //Initialize the projectile with a catchup value of time passed.
            Projectile p = Instantiate(_projectilePrefab, position, Quaternion.identity);
            p.Initialize((float)timePassed);

            //Fire on other clients using the same data.
            RpcFire(position, networkTime);
        }


        [ClientRpc]
        private void RpcFire(Vector3 position, double networkTime)
        {
            //If has authority ignore this, already fired locally.
            if (base.hasAuthority)
                return;

            /* Like in the command you will get the time passed and initialize
             * the projectile with that value. */
            double timePassed = NetworkTime.time - networkTime;

            Projectile p = Instantiate(_projectilePrefab, position, Quaternion.identity);
            p.Initialize((float)timePassed);
        }

        #region With delay test code. You're dealing with some messy stuff here!
        //        public float FireDelay = 0.05f;

        //        #region Serialized.
        //        /// <summary>
        //        /// Projectile prefab.
        //        /// </summary>
        //        [Tooltip("Projectile prefab.")]
        //        [SerializeField]
        //        private Projectile _projectilePrefab;
        //        /// <summary>
        //        /// How quickly the player should move.
        //        /// </summary>
        //        [Tooltip("How quickly the player should move.")]
        //        [SerializeField]
        //        private float _moveRate = 1f;
        //        #endregion

        //        private void Update()
        //        {
        //            ClientUpdate();
        //        }       

        //        private void ClientUpdate()
        //        {
        //            if (!base.isClient)
        //                return;

        //            Move();
        //            Fire();
        //        }

        //        /// <summary>
        //        /// Moves the player.
        //        /// </summary>
        //        private void Move()
        //        {
        //            if (!base.hasAuthority)
        //                return;

        //            float horizontal = Input.GetAxisRaw("Horizontal");
        //            float vertical = Input.GetAxisRaw("Vertical");

        //            transform.position += new Vector3(horizontal, vertical, 0f) * _moveRate * Time.deltaTime;
        //        }

        //        /// <summary>
        //        /// Fires projectile.
        //        /// </summary>
        //        private void Fire()
        //        {
        //            if (!base.hasAuthority)
        //                return;

        //            //Only fire when mouse 0 is pressed.
        //            if (!Input.GetKeyDown(KeyCode.Mouse0))
        //                return;

        //            Projectile p = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        //            p.gameObject.name = "C " + p.gameObject.name;
        //            p.Initialize(0f);
        //            StartCoroutine(__DelayCmdFire(FireDelay / 2f));
        //        }

        //        /// <summary>
        //        /// Issues the fire command after a delay. Used to simulate lag.
        //        /// </summary>
        //        /// <param name="delay"></param>
        //        /// <returns></returns>
        //        private IEnumerator __DelayCmdFire(float delay)
        //        {
        //            double networkTime = NetworkTime.time;
        //            Vector3 pos = transform.position;

        //            yield return new WaitForSeconds(delay);

        //            CmdFire(pos, networkTime, delay);
        //        }

        //        /// <summary>
        //        /// Fires over the network.
        //        /// </summary>
        //        [Command]
        //        private void CmdFire(Vector3 position, double networkTime, float delay)
        //        {
        //            double timePassed = NetworkTime.time - networkTime;
        //            //Instantiate on server so that it may register collision.
        //            Projectile p = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        //            p.gameObject.name = "S " + p.gameObject.name;
        //            p.Initialize((float)timePassed);

        //            //RpcFire(position, networkTime);
        //            StartCoroutine(__RpcDelayCmdFire(delay, position, networkTime));
        //        }

        //        private IEnumerator __RpcDelayCmdFire(float delay, Vector3 position, double networkTime)
        //        {
        //            yield return new WaitForSeconds(delay);

        //            RpcFire(position, networkTime);
        //        }


        //        [ClientRpc]
        //        private void RpcFire(Vector3 position, double networkTime)
        //        {
        //#if UNITY_EDITOR
        //            /* Dont fire again if from the editor. This is a lazy hack for testing to not
        //             * duplicate projectiles. */
        //            return;
        //#endif
        //            double timePassed = NetworkTime.time - networkTime;

        //            Projectile p = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
        //            p.gameObject.name = "X " + p.gameObject.name;
        //            p.Initialize((float)timePassed);

        //        }
        #endregion

    }


}