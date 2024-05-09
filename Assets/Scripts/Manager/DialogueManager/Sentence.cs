using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sentence
{
    public AudioClip voice;

    [TextArea(5, 10)]
    public string text;
}
