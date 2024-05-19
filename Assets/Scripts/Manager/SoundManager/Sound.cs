using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound Data")]
public class Sound : ScriptableObject
{
    public string id;
    
    [TextArea(5, 10)]
    public string description;

    public AudioClip clip;

    private void OnValidate() 
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
