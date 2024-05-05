using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Data")]
public class Dialogue : ScriptableObject
{
    public string id;
    
    public Utterance[] utterances;

    // Evene name 이 이름과 동기화됨
    private void OnValidate() 
    {
        #if UNITY_EDITOR
            id = this.name;
            UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
