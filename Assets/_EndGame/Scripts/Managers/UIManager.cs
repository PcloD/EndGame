using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private GameObject LoginCanvasPrefab;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!PlayFabAuthenticationAPI.IsEntityLoggedIn())
        {
            Debug.Log("Not Logged In, opening login screen");
            GameObject loginCanvas = Instantiate(LoginCanvasPrefab, Vector3.zero, Quaternion.identity);
            loginCanvas.SetActive(true);
        }
    }
}