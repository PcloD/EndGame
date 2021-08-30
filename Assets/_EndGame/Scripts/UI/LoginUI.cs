using System;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] Button loginBtn;
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] Button registerBtn;
    
    [SerializeField] TMP_Text infoText;

    // Runtime
    private string _username;
    private string _password;
    
    private void OnEnable()
    {        
        loginBtn.onClick.AddListener(LoginToPlayFab);
        registerBtn.onClick.AddListener(RegisterPlayer);
    }

    private void OnDisable()
    {
        loginBtn.onClick.RemoveListener(LoginToPlayFab);
        registerBtn.onClick.RemoveListener(RegisterPlayer);
    }

    #region Login
    void LoginToPlayFab()
    {
        _username = usernameInput.text;
        _password = passwordInput.text;
        
        Debug.Log($"Submitting Login for {_username}");
        loginBtn.enabled = false;
        registerBtn.enabled = false;

        infoText.text = "";
        infoText.enabled = false;

        AccountManager.Instance.PlayFabLogin.LoginWithEmail(_username, _password, OnLoginSuccess, OnLoginError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        GameObject.Destroy(gameObject);
    }

    void OnLoginError(PlayFabError error)
    {
        loginBtn.enabled = true;
        registerBtn.enabled = true;

        infoText.text = $"<#ffbb00>{error.ErrorMessage}";
        infoText.enabled = true;
    }
    #endregion

    #region Register
    void RegisterPlayer()
    {
        _username = usernameInput.text;
        _password = passwordInput.text;
        
        loginBtn.enabled = false;
        registerBtn.enabled = false;

        infoText.text = "";
        infoText.enabled = false;
        
        AccountManager.Instance.PlayFabLogin.RegisterPlayerWithEmail(_username, _password, OnRegisterSuccess, OnRegisterError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        loginBtn.enabled = true;
        registerBtn.enabled = true;

        infoText.text = $"<#00ffbb>Register successful {result.Username}";
        infoText.enabled = true;
    }

    void OnRegisterError(PlayFabError error)
    {
        loginBtn.enabled = true;
        registerBtn.enabled = true;

        infoText.text = $"<#ffbb00>{error.ErrorMessage}";
        infoText.enabled = true;
    }
    #endregion
}