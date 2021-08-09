
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGearGames.Mirrors.SimpleCharacterSelection
{


    public class OfflineGameplayDependencies : MonoBehaviour
    {
        /* Could destroy this on server since it's only meant for client,
         * but that won't be covered in this video. */

        #region Serialized.
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("Canvas for spawning.")]
        [SerializeField]
        private SpawningCanvas _spawningCanvas = null;
        /// <summary>
        /// Canvas for spawning.
        /// </summary>
        public static SpawningCanvas SpawningCanvas { get { return _instance._spawningCanvas; } }
        #endregion


        #region Private.
        /// <summary>
        /// Singleton instance of this script.
        /// </summary>
        private static OfflineGameplayDependencies _instance;
        #endregion

        private void Awake()
        {
            _instance = this;
        }


    }


}