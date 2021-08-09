using UnityEngine;

#pragma warning disable CS0618, CS0672, CS0649
namespace FirstGearGames.Mirrors.InitializingLocalPlayers
{
    public class HealthPercent : MonoBehaviour
    {

        #region Serialized.
        /// <summary>
        /// 
        /// </summary>
        [Tooltip("Current health percent of this unit.")]
        [Range(0f, 1f)]
        [SerializeField]
        private float _currentPercent;
        /// <summary>
        /// Current health percent of this unit.
        /// </summary>
        public float CurrentPercent { get { return _currentPercent; } }
        #endregion

    }
}