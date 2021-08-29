using System;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] Button LoginBtn;
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_InputField passwordInput;
    
    [SerializeField] private PlayFabLogin playFabLogin;

    // Runtime
    private string _username;
    private string _password;
    
    private void OnEnable()
    {
        if (playFabLogin == null)
            playFabLogin = GetComponent<PlayFabLogin>();
        
        LoginBtn.onClick.AddListener(LoginToPlayFab);
    }

    private void OnDisable()
    {
        LoginBtn.onClick.AddListener(LoginToPlayFab);
    }

    void LoginToPlayFab()
    {
        _username = usernameInput.text;
        _password = passwordInput.text;
        
        Debug.Log($"Submitting Login for {_username}");
        playFabLogin.LoginWithEmail(_username, _password);
    }
}