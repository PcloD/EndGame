using System;
using System.Collections.Generic;
using DG.Tweening;

using PlayFab;
using PlayFab.ClientModels;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [Header ("Common")]
    [SerializeField] TMP_Text headerText;


    #region Login Group

    [Header ("Login")]
    [SerializeField] List<Selectable> loginTabGroup;
    [SerializeField] CanvasGroup loginCG;
    [SerializeField] Button loginBtn;
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] Button registerBtn;

    [SerializeField] TMP_Text loginInfo;
    #endregion

    #region Register Group

    [Header ("Register")]
    [SerializeField] List<Selectable> registerTabGroup;
    [SerializeField] CanvasGroup registerCG;
    [SerializeField] Button registerSubmitBtn;
    [SerializeField] TMP_InputField newUsernameInput;
    [SerializeField] TMP_InputField newEmailInput;
    [SerializeField] TMP_InputField newPasswordInput;
    [SerializeField] TMP_InputField passwordConfirmInput;

    [SerializeField] TMP_Text registerInfo;
    #endregion

    // Runtime
    private string _username;
    private string _email;
    private string _password;

    private void OnEnable ()
    {

        loginBtn.onClick.AddListener (LoginToPlayFab);
        registerBtn.onClick.AddListener (OnNewRegisterClick);
        registerSubmitBtn.onClick.AddListener (RegisterPlayer);

        EnableLoginCG ();
    }

    private void OnDisable ()
    {
        loginBtn.onClick.RemoveListener (LoginToPlayFab);
        registerBtn.onClick.RemoveListener (OnNewRegisterClick);
        registerSubmitBtn.onClick.RemoveListener (RegisterPlayer);

        loginCG.interactable = false;
        loginCG.alpha = 0f;
        loginCG.gameObject.SetActive (false);
    }

    #region Login
    void LoginToPlayFab ()
    {
        _username = emailInput.text;
        _password = passwordInput.text;

        Debug.Log ($"Submitting Login for {_username}");
        loginBtn.enabled = false;
        registerBtn.enabled = false;

        loginInfo.text = "";

        AccountManager.Instance.PlayFabLogin.LoginWithEmail (_username, _password, OnLoginSuccess, OnLoginError);
    }

    void OnLoginSuccess (LoginResult result)
    {
        SceneUtilityManager.Instance.LoadSceneAsync(2);
        
        GameObject.Destroy (gameObject);
    }

    void OnLoginError (PlayFabError error)
    {
        loginBtn.enabled = true;
        registerBtn.enabled = true;

        loginInfo.text = $"<#ffbb00>{error.ErrorMessage}";
    }

    void EnableLoginCG ()
    {
        UIManager.Instance.CurrentTabOrder = loginTabGroup.ToArray();

        headerText.text = "Login";
        loginCG.alpha = 0f;
        loginCG.gameObject.SetActive (true);
        loginInfo.text = "";

        DOTween.To (() => loginCG.alpha, x => loginCG.alpha = x, 1f, 1f).OnComplete (() =>
        {
            loginCG.interactable = true;
            emailInput.Select();
        });
    }

    void OnNewRegisterClick ()
    {
        DOTween.To (() => loginCG.alpha, x => loginCG.alpha = x, 0f, 0.5f).OnComplete (EnableRegisterCG);
    }
    #endregion

    #region Register
    void EnableRegisterCG ()
    {
        UIManager.Instance.CurrentTabOrder = registerTabGroup.ToArray();

        headerText.text = "Register";
        loginCG.interactable = false;
        loginCG.gameObject.SetActive (false);

        registerCG.alpha = 0f;
        registerCG.gameObject.SetActive (true);
        registerInfo.text = "";

        DOTween.To (() => registerCG.alpha, x => registerCG.alpha = x, 1f, 0.5f).OnComplete (() =>
        {
            registerCG.interactable = true;
        });
    }

    void RegisterPlayer ()
    {
        _username = newUsernameInput.text;
        _email = newEmailInput.text;
        _password = newPasswordInput.text;

        if(!_password.Equals(passwordConfirmInput.text))
        {
            registerInfo.text = "<#ffbb00>You trippin? Passwords don't match...";
            return;
        }

        registerSubmitBtn.enabled = false;

        registerInfo.text = "";

        AccountManager.Instance.PlayFabLogin.RegisterPlayerWithEmail (_email, _username, _password, OnRegisterSuccess, OnRegisterError);
    }

    void OnRegisterSuccess (RegisterPlayFabUserResult result)
    {
        loginBtn.enabled = true;
        registerBtn.enabled = true;

        registerInfo.text = $"<#00ffbb>Register successful {result.Username}";

        DOTween.To (() => registerCG.alpha, x => registerCG.alpha = x, 0f, 0.5f).OnComplete (() =>
        {
            registerCG.interactable = false;
            registerCG.gameObject.SetActive (false);
            EnableLoginCG();
        });
    }

    void OnRegisterError (PlayFabError error)
    {
        registerSubmitBtn.enabled = true;

        registerInfo.text = $"<#ffbb00>{error.ErrorMessage}";
    }
    #endregion
}
