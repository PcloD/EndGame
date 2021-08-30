using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    private string Username;
    public void Login()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)){
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "1C275";
        }
        var request = new LoginWithCustomIDRequest { CustomId = "LionTurtle", CreateAccount = true};
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    public void LoginWithEmail(string email, string password, Action<LoginResult> success, Action<PlayFabError> failure)
    {
        LoginWithEmailAddressRequest loginReq = new LoginWithEmailAddressRequest()
            {Email = email, Password = password};
        
        PlayFabClientAPI.LoginWithEmailAddress(loginReq, 
        (result) => {
            OnLoginSuccess(result);
            success(result);
        },
        (error) => {
            OnLoginFailure(error);
            failure(error);
        });
    }

    public void RegisterPlayerWithEmail(string email, string password, Action<RegisterPlayFabUserResult> success, Action<PlayFabError> failure)
    {
        RegisterPlayFabUserRequest registerRequest = new RegisterPlayFabUserRequest(){Email = email, Password = password, RequireBothUsernameAndEmail = false};
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, 
        (result) => {
            OnRegisterSuccess(result);
            success(result);
        },
        (error) => {
            OnRegisterFailure(error);
            failure(error);
        });
    }
    

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Username = result.Username;
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogWarning("Error while registering new player");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
        Debug.Log($"PlayerID: {result.PlayFabId}");

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(){
            PlayFabId = result.PlayFabId
        }, OnAccountInfoSuccess, OnAccountInfoFailure);
        AccountManager.Instance.PlayerID = result.PlayFabId;
    }

    private void OnAccountInfoSuccess(GetAccountInfoResult result)
    {
        AccountManager.Instance.Username = result.AccountInfo.Username;
    }

    private void OnAccountInfoFailure(PlayFabError error)
    {
        Debug.LogWarning("Error while getting account info");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Error while logging in player");
        Debug.LogError(error.GenerateErrorReport());
    }
}