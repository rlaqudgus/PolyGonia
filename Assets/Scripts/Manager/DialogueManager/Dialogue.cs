using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Data")]
public class Dialogue : ScriptableObject
{
    public string eventName;
    
    public Utterance[] utterances;
}
