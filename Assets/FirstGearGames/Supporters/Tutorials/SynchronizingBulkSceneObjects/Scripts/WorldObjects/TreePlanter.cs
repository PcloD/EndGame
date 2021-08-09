using Mirror;
using UnityEngine;

namespace FirstGearGames.Mirrors.SynchronizingBulkSceneObjects
{

    public class TreePlanter : MonoBehaviour
    {
        /// <summary>
        /// Prefab to spawn for new WorldObjects.
        /// </summary>
        [Tooltip("Prefab to spawn for new WorldObjects.")]
        [SerializeField]
        private WorldObject _treePrefab;

        private void Update()
        {
            if (NetworkServer.active)
            {
                CheckPlantTreeObject();
            }
        }

        /// <summary>
        /// Plant all the WorldObjects.
        /// </summary>
        private void CheckPlantTreeObject()
        {
            //Right click plants
            if (!Input.GetKeyDown(KeyCode.Mouse1))
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == "Ground")
                {
                    WorldObject wo = WorldObjectManager.Instance.InstantiateWorldObject(WorldObjectTypes.Tree, hit.point, Quaternion.identity);
                    //Grab data from world object as TreeObjectData, since I know this is a tree.
                    TreeObjectData data = (TreeObjectData)wo.ReturnData();
                    data.SetTreeState(TreeObjectData.TreeStates.Default);
                    //Apply changes to WorldObject.
                    wo.UpdateData(data);

                    //Initialize on the WorldObjectManager.
                    WorldObjectManager.Instance.InitializeWorldObject(wo, WorldObjectTypes.Tree);
                }
            }

        }
    }

}