using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CombatTextTesting : MonoBehaviour
{
    [SerializeField] private Transform combatTextPrefab;
    [SerializeField] private Vector3 spawnPos;

    private Array values = Enum.GetValues((typeof(CombatTextManager.DamageType)));

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.D))
        {
            Debug.Log("Applying damage to test combat text", this);
            CombatTextManager.Instance.CreatePopup(spawnPos, Random.Range(1,20000), (CombatTextManager.DamageType)values.GetValue(Random.Range(0, values.Length)));
        }
    }
}
