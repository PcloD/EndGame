using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[Serializable]
public class AnimationTrigger
{
    public int Hash;
    public string Name;
    public float TimeRegistered;

    public AnimationTrigger(string name)
    {
        Name = name;
        Hash = Animator.StringToHash(name);
        TimeRegistered = Time.time;
    }
}
