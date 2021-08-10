using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log($"Collision" + other.transform.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger " + other.name);
    }
}
