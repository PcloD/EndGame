using Mirror;
using UnityEngine;

#pragma warning disable CS0618, CS0672, CS0649
namespace FirstGearGames.Mirrors.SynchronizingBulkSceneObjects
{

    public class Tree : WorldObject
    {
        #region Public.
        /// <summary>
        /// Data for this WorldObject. Needs to be serialized so that the key be written in the scene.
        /// </summary>
        [SerializeField, HideInInspector]
        public TreeObjectData Data = new TreeObjectData();
        #endregion

        #region Serialized.
        /// <summary>
        /// Object to show when WorldObject is available.
        /// </summary>
        [Tooltip("Object to show when WorldObject is available.")]
        [SerializeField]
        private GameObject _visualObject;
        #endregion

        /// <summary>
        /// Always override this in children and call base.Awake()
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Updates this object's data.
        /// </summary>
        public override void UpdateData(WorldObjectData data)
        {
            Data = (TreeObjectData)data;
            ApplyStateVisuals();
        }

        /// <summary>
        /// Returns if the object is not in it's default state.
        /// </summary>
        /// <returns></returns>
        public override bool ObjectNotDefault()
        {
            /* You would override this for each object to determine
            * if it has changed. Generally if an object is spawned
            * and exist in any fashion it is not considered default,
            * as spawned objects must be sent to late joining clients. */
            bool changed = (Data.TreeState != TreeObjectData.TreeStates.Default ||
                               Data.Instantiated);
            return changed;
        }

        /// <summary>
        /// Returns WorldObjectData for this object.
        /// </summary>
        /// <returns></returns>
        public override WorldObjectData ReturnData()
        {
            return Data;
        }


        public void OnMouseDown()
        {
            //Only allow server to perform actions on WorldObjects.
            if (!NetworkServer.active)
                return;

            //Perform actions based on current state.
            if (Data.TreeState == TreeObjectData.TreeStates.Default)
            {
                Data.SetTreeState(TreeObjectData.TreeStates.Chopped);
            }
            //Not active. Second click will destroy.
            else if (Data.TreeState == TreeObjectData.TreeStates.Chopped)
            {
                Data.SetTreeState(TreeObjectData.TreeStates.Removed);
            }
            //Cannot change from any other states.
            else
            {
                return;
            }

            ApplyStateVisuals();

            /* If object should be removed. */
            if (Data.TreeState == TreeObjectData.TreeStates.Removed)
                WorldObjectManager.Instance.RemoveWorldObject(this);
            /* If object still exist then tell WorldObjectManager
             * it has updated. */
            else
                WorldObjectManager.Instance.UpdateWorldObject(this);
        }

        /// <summary>
        /// Applies a state to this WorldObject.
        /// </summary>
        /// <param name="state"></param>
        private void ApplyStateVisuals()
        {
            /* You would typically exit the method here if
             * running as server only. I'm still showing visual
             * changes for demo purposes. */

            //Set whole active if not destroyed.
            gameObject.SetActive(Data.TreeState != TreeObjectData.TreeStates.Removed);
            /* Set WorldObject visual active based on if default. 
             * Default for WorldObjects is visible. */
            _visualObject.SetActive(Data.TreeState == TreeObjectData.TreeStates.Default);
        }



    }



}