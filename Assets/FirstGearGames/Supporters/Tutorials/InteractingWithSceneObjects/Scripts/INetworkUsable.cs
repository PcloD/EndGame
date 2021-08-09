using Mirror;

namespace FirstGearGames.Mirrors.InteractingSceneObjects
{

    public interface INetworkUsable
    {
        void SetId(int value);
        int GetId();
        void Use();
        NetworkIdentity GetNetworkIdentity();
    }

}