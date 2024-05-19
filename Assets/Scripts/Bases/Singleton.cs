using UnityEngine;

namespace Bases
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (!_instance) Debug.Log($"{typeof(T)} is Empty");
                return _instance;
            }
        }

        protected void CreateSingleton(T instance)
        {
            if (_instance == null)
            {
                _instance = instance; 
                DontDestroyOnLoad(this);
                Init();
            }
            else Destroy(this);
        }

        protected virtual void Init()
        {
        
        }
    }
}