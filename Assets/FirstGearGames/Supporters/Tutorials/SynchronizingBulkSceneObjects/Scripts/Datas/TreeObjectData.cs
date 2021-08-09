
using UnityEngine;

namespace FirstGearGames.Mirrors.SynchronizingBulkSceneObjects
{

    #region Types.
    /* I made the data which must be in sync it's own class so that I
     * could write a serializer for it and pass that over the network. */
    [System.Serializable]
    public class TreeObjectData : WorldObjectData
    {
        #region Types.
        /// <summary>
        /// States a tree may have.
        /// </summary>
        public enum TreeStates
        {
            Default = 0,
            Chopped = 1,
            Removed = 2
        }
        #endregion
        /// <summary>
        /// Current state of the object.
        /// </summary>
        public TreeStates TreeState { get; private set; } = TreeStates.Default;
        /// <summary>
        /// Sets CurrentState for WorldObject.
        /// </summary>
        /// <param name="value"></param>
        public void SetTreeState(TreeStates value) { TreeState = value; }
    }

    #endregion
}