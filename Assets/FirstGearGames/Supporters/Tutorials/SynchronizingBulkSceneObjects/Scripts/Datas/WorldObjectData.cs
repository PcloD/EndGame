
using Mirror;
using UnityEngine;

#pragma warning disable CS0618, CS0672
namespace FirstGearGames.Mirrors.SynchronizingBulkSceneObjects
{

    /* I made the data which must be in sync it's own class so that I
     * could write a serializer for it and pass that over the network. */
    [System.Serializable]
    public class WorldObjectData
    {
        /// <summary>
        /// Serialized value because accessors cannot be serialized.
        /// </summary>
        [SerializeField, HideInInspector]
        private uint _key;
        /// <summary>
        /// Key for this data.
        /// </summary>
        public uint Key
        {
            get { return _key; }
            set { _key = value; }
        }
        /// <summary>
        /// Sets the Key value.
        /// </summary>
        /// <param name="value"></param>
        public void SetKey(uint value)
        {
            Key = value;
        }
        /// <summary>
        /// WorldObjectType for this instance.
        /// </summary>
        public WorldObjectTypes ObjectType { get; private set; } = WorldObjectTypes.Unset;
        /// <summary>
        /// Sets Type value.
        /// </summary>
        /// <param name="value"></param>
        public void SetObjectType(WorldObjectTypes value)
        {
            ObjectType = value;
        }
        /// <summary>
        /// True if instantiated rather than placed in the scene.
        /// </summary>
        public bool Instantiated { get; private set; } = false;
        /// <summary>
        /// Sets Instantiated value.
        /// </summary>
        /// <param name="value"></param>
        public void SetInstantiated(bool value) { Instantiated = value; }
    }

    public static class WorldObjectDataSerializer
    {
        public static void WriteWorldObjectData(this NetworkWriter writer, WorldObjectData data)
        {
            writer.WriteInt32((int)data.ObjectType);
            writer.WriteUInt32(data.Key);
            writer.WriteBoolean(data.Instantiated);

            //Tree type.
            if (data is TreeObjectData od)
            {
                writer.WriteByte((byte)od.TreeState);
            }
        }

        public static WorldObjectData ReadWorldObjectData(this NetworkReader reader)
        {
            WorldObjectTypes objectType = (WorldObjectTypes)reader.ReadInt32();
            uint key = reader.ReadUInt32();
            bool spawned = reader.ReadBoolean();

            //Tree type.
            if (objectType == WorldObjectTypes.Tree)
            {
                TreeObjectData data = new TreeObjectData();
                data.SetObjectType(objectType);
                data.SetKey(key);
                data.SetInstantiated(spawned);
                data.SetTreeState((TreeObjectData.TreeStates)reader.ReadByte());

                return data;
            }
            //Not supported by serializer.
            else
            {
                Debug.LogError("Serializer not written for type " + objectType.ToString());
                return null;
            }

        }

    }

}