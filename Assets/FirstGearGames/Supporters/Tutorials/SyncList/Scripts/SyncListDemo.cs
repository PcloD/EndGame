using Mirror;
using System;
using System.Collections;
using UnityEngine;

#pragma warning disable CS0618
namespace FirstGearGames.Mirrors.SyncLists
{


    public class SyncListDemo : NetworkBehaviour
    {
        /* Build in sync list types.
         * SyncListString
         * SyncListFloat
         * SyncListInt
         * SyncListUInt
         * SyncListBool
         * */
        /* This must be initialized in the constructor as shown. To help
        * ensure this is done, make your sync list readonly as shown. */
        private readonly SyncListInt _randomNumbers = new SyncListInt();

        /// <summary>
        /// Gear for each player
        /// </summary>
        private readonly SyncListPlayerGear _playerGear = new SyncListPlayerGear();

        public override void OnStartClient()
        {
            base.OnStartClient();

            /* When you first receive OnStartClient you will
             * not get a callback for initial values, even if they
             * are different from the defaults. Be sure to check / handle
             * values in your list if required when this script/object
             * starts on client. */

            /* Unlike SyncVar where you can use an attribute, you must
            * instead subscribe to the synclist callback. */
            _randomNumbers.Callback += RandomNumbers_Callback;


            //_playerGear.Callback += PlayerGear_Callback;
        }

        /// <summary>
        /// Received on client when RandomNumbers change.
        /// </summary>
        /// <param name="op">Type of change to the list.</param>
        /// <param name="itemIndex">Index of list which was changed.</param>
        /// <param name="oldItem">Old item, if removed.</param>
        /// <param name="newItem">New item, if added.</param>
        private void RandomNumbers_Callback(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
        {
            Debug.Log("Client SyncList callback: " + op.ToString() + " old " + oldItem + ", new " + newItem);
            /* It's important to know that whenever you receive
            * a callback for a synclist item, the change has already
            * occurred within the list. You would use the callback
            * parameters to determine what information has been changed. */

            switch (op)
            {
                /* Value was added at itemIndex.
                * Value is newItem. */
                case SyncList<int>.Operation.OP_ADD:
                    break;
                /* List was cleared, no items exist. */
                case SyncList<int>.Operation.OP_CLEAR:
                    break;
                /* Value was inserted at itemIndex.
                 * Value is newItem. */
                case SyncList<int>.Operation.OP_INSERT:
                    break;
                /* Item was removed from itemIndex.
                 * Value removed is oldItem. */
                case SyncList<int>.Operation.OP_REMOVEAT:
                    break;
                /* ItemValuewas replaced/set at itemIndex.
                * Replaced value is oldItem, new value is newItem. */
                case SyncList<int>.Operation.OP_SET:
                    break;
            }

            /* Callbacks can occur multiple times in one frame. If you are performing any
             * intensive list operations after a list change, such as sorting, it may be
             * best to set a flag, such as a bool, indicating to perform these operations
             * on the next frame. */
        }


        /// <summary>
        /// This method runs on the server. It may also run on a client acting as the host.
        /// </summary>
        public override void OnStartServer()
        {
            base.OnStartServer();

            //Add a new player to my list.
            _playerGear.Add(new PlayerGear(0, 50));

            int newCount = UnityEngine.Random.Range(1, 5000);

            //This will fail.
            _playerGear[0].GearCount = newCount;

            //This will work.            
            //PlayerGear pg = new PlayerGear(_playerGear[0].PlayerConnectionId, newCount);
            //_playerGear[0] = pg;

            PlayerGear pg = _playerGear[0];
            _playerGear.RemoveAt(0);
            
            pg.GearCount = newCount;
            _playerGear.Add(pg);

            //Start randomly modifying random numbers.
            StartCoroutine(__ModifyNumbersSyncList());
        }


        /// <summary>
        /// Regularly modify the sync list.
        /// </summary>
        /// <returns></returns>
        private IEnumerator __ModifyNumbersSyncList()
        {
            WaitForSeconds wait = new WaitForSeconds(3f);

            while (true)
            {
                bool add = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
                //No numbers currently, or randomly add.
                if (_randomNumbers.Count <= 0 || add)
                {
                    int number = UnityEngine.Random.Range(0, 100);
                    Debug.Log("Adding new number " + number);
                    _randomNumbers.Add(number);
                }
                //Not adding, modifying or removing.
                else
                {
                    bool modify = (UnityEngine.Random.Range(0f, 1f) > 0.5f);
                    Debug.Log("Modifying existing number, removing? " + (modify == false));
                    //Index to remove or modify.
                    int index = UnityEngine.Random.Range(0, _randomNumbers.Count - 1);
                    //If to modify instead of remove.
                    if (modify)
                        _randomNumbers[index] = UnityEngine.Random.Range(0, 100);
                    //Remove.
                    else
                        _randomNumbers.RemoveAt(index);
                }

                yield return wait;
            }
        }

    }

}