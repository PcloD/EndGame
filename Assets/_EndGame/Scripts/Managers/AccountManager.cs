using UnityEngine;

public class AccountManager : MonoBehaviour
{
    public static AccountManager Instance;
    public PlayFabLogin PlayFabLogin;

    public string Username;
    public string PlayerID;

    void Awake ()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        
        if (PlayFabLogin == null)
            PlayFabLogin = GetComponent<PlayFabLogin> ();
    }

}
