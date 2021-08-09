using Mirror;

namespace FirstGearGames.Mirrors.SyncLists
{
    /* Make sure both the item you are making into a list,
     * as well the synclist itself are set as serializable;
     * see below System.Serializable attribute. */
    /// <summary>
    /// Information about a players gear.
    /// </summary>
    [System.Serializable]
    public class PlayerGear
    {
        public PlayerGear() { }
        public PlayerGear(int playerConnectionId, int gearCount)
        {
            PlayerConnectionId = playerConnectionId;
            GearCount = gearCount;
        }

        public int PlayerConnectionId;
        public int GearCount;
    }

    /// <summary>
    /// SyncList for PlayerGear.
    /// </summary>
    [System.Serializable]
    public class SyncListPlayerGear : SyncList<PlayerGear> { }
}
