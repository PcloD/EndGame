//Allowed system data types.
/*
    bool System.Boolean
    byte System.Byte
    sbyte System.SByte
    char System.Char
    decimal System.Decimal
    double System.Double
    float System.Single
    int System.Int32
    uint System.UInt32
    long System.Int64
    ulong System.UInt64    
    short System.Int16
    ushort System.UInt16
    string System.string

    Note: object is missing.
 */

//Allowed Unity and Mirror types.
/*
    Vector2
    Vector3
    Vector4
    Vector2Int
    Vector3Int
    Color
    Color32
    Quaternion
    Rect
    Plane
    Ray
    Matrix4x4
    Guid
    NetworkIdentity
    Transform
    GameObject
    IMessageBase (long as it uses built in data types.)
 */


using Mirror;
using UnityEngine;


namespace FirstGearGames.Mirrors.BuildInDataTypes
{


    public class BuiltInDataTypes : NetworkBehaviour
    {

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!base.hasAuthority)
                return;

            //Cannot send list!
            //List<int> ints = new List<int>() { 0, 1, 3, 2 };
            //CmdSendList(ints);

            int[] ints = new int[] { 0, 1, 3, 2 };
            CmdSendArray(ints);
        }

        //Cannot send list!
        //[Command]
        //private void CmdSendList(List<int> values)
        //{
        //    Debug.Log("Received ints:");
        //    foreach (int item in values)
        //        Debug.Log(item);
        //}

        [Command]
        private void CmdSendArray(int[] values)
        {
            Debug.Log("Received ints:");
            foreach (int item in values)
                Debug.Log(item);
        }

    }

}