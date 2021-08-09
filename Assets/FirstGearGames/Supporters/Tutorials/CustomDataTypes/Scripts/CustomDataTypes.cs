using Mirror;
using UnityEngine;


namespace FirstGearGames.Mirrors.CustomDataTypes
{

    public class CustomDataTypes : NetworkBehaviour
    {
        #region Will serialize.
        //Will serialize because it uses built in types.
        public struct A
        {
            public byte ID;
            public Vector3 Position;
        }
        public class B
        {
            public string Name;
            //Won't serialize if ItemStats is null.
            public C Stats;
        }
        /* Will serialize because it uses built-in types.
         * Any type which isn't built in would need to have a
         * serializer made. */
        public class C
        {
            public int Count;
        }
        #endregion

        #region Demo classes.
        public class Item
        {
            public string Name;

            public virtual object DeepCopy()
            {
                Item result = new Item();
                result.Name = Name;
                return result;
            }
        }
        public class Armour : Item
        {
            public int Protection;
            public int Weight;

            public override object DeepCopy()
            {
                Armour result = (Armour)base.DeepCopy();
                result.Protection = Protection;
                result.Weight = Weight;

                return result;
            }
        }
        public class Potion : Item
        {
            public int Health;
        }
        #endregion

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            //Make a new potion and send it over the network.
            Potion pot = new Potion();
            pot.Name = "Health Potion";
            pot.Health = 5;
            //Send potion.
            CmdSendItem(pot);
        }

        [Command]
        private void CmdSendItem(Item item)
        {
            //If an item.
            if (item is Item)
                Debug.Log("My item name is " + item.Name + ".");
            //If is also a potion.
            if (item is Potion potion)
                Debug.Log("My potion gives " + potion.Health + " health.");
        }

    }


}