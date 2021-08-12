using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class UISetPing : MonoBehaviour
{
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        text.text = $"RTT: {Math.Round(NetworkTime.rtt * 1000)}ms";
    }
}
