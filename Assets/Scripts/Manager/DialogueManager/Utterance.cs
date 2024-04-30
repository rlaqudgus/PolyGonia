using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Utterance
{
    public string speaker;

    [TextArea(5, 10)]
    public string[] sentences;
}
