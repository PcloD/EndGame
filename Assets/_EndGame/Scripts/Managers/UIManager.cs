using System;
using System.Collections;
using System.Collections.Generic;

using PlayFab;

using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private GameObject LoginCanvasPrefab;

    public Selectable[] CurrentTabOrder
    {
        set
        {
            tabIndex = 0;
            _currentTabOrder = value;
        }
        get
        {
            return _currentTabOrder;
        }
    }
    private Selectable[] _currentTabOrder;

    private int tabIndex = 0;
    private void Awake ()
    {
        Instance = this;
    }

    private void Start ()
    {
        if (!PlayFabAuthenticationAPI.IsEntityLoggedIn ())
        {
            Debug.Log ("Not Logged In, opening login screen");
            GameObject loginCanvas = Instantiate (LoginCanvasPrefab, Vector3.zero, Quaternion.identity);
            loginCanvas.SetActive (true);
        }
    }

    void Update ()
    {
        // Handle UI Tab
        if (Input.GetKeyUp (KeyCode.Tab))
        {
            if(Input.GetKeyDown(KeyCode.LeftShift))
                tabIndex = tabIndex == 0 ? _currentTabOrder.Length - 1 : tabIndex - 1;
            else
                tabIndex = tabIndex == _currentTabOrder.Length - 1 ? 0 : tabIndex + 1;

            if (_currentTabOrder[tabIndex].gameObject.activeInHierarchy && _currentTabOrder[tabIndex].enabled)
                _currentTabOrder[tabIndex].Select ();
        }

    }
}
