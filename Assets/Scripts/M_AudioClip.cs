using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ClipType
{
    Menu,
    Level
}

[Serializable]
public class M_AudioClip
{
    public string name;
    public AudioClip clip;
    public ClipType type;
}
