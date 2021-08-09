using Mirror;
using System;
using static FirstGearGames.Mirrors.CustomDataTypes.CustomDataTypes;

#pragma warning disable CS0618, CS0672
namespace FirstGearGames.Mirrors.CustomDataTypes
{

    public static class ItemSerializer
    {
        private const byte ITEM_ID = 0;
        private const byte POTION_ID = 1;
        private const byte ARMOUR_ID = 2;

        public static void WriteItem(this NetworkWriter writer, Item item)
        {
            //If potion class.
            if (item is Potion potion)
            {
                writer.WriteByte(POTION_ID);
                writer.WriteString(potion.Name);
                writer.WriteInt32(potion.Health);
            }
            //If armour class.
            else if (item is Armour armour)
            {
                writer.WriteByte(ARMOUR_ID);
                writer.WriteString(armour.Name);
                writer.WriteInt32(armour.Protection);
                writer.WriteInt32(armour.Weight);
            }
            //Unhandled, assume Item class.
            else
            {
                writer.WriteByte(ITEM_ID);
                writer.WriteString(item.Name);
            }
        }

        public static Item ReadItem(this NetworkReader reader)
        {
            byte id = reader.ReadByte();

            switch (id)
            {
                //Potion.
                case POTION_ID:
                    return new Potion
                    {
                        Name = reader.ReadString(),
                        Health = reader.ReadInt32()
                    };
                //Armour.
                case ARMOUR_ID:
                    return new Armour
                    {
                        Name = reader.ReadString(),
                        Protection = reader.ReadInt32(),
                        Weight = reader.ReadInt32()
                    };
                //Item.
                case ITEM_ID:
                    return new Item
                    {
                        Name = reader.ReadString()
                    };
                //Unhandled.
                default:
                    throw new Exception($"Unhandled item type for {id}.");
            }
        }

    }


}