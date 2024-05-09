using UnityEngine;

namespace Manager.DialogueScripts
{
    [System.Serializable]
    public class Utterance
    {
        public string speaker;
        public Sentence[] sentences;
    }
    
    
    [System.Serializable]
    public class Sentence
    {
        public AudioClip voice;

        [TextArea(5, 10)]
        public string text;
    }

    
    [CreateAssetMenu(menuName = "Dialogue Data")]
    public class Dialogue : ScriptableObject
    {
        public string id;
    
        [SerializeField] public Utterance[] utterances;

        // Evene name 이 이름과 동기화됨
        private void OnValidate() 
        {
#if UNITY_EDITOR
            id = this.name;
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
