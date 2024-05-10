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

        protected void CreateSingleton(T instance, bool dontDestroyOnLoad = false)
        {
            _instance = instance;
            if (dontDestroyOnLoad) DontDestroyOnLoad(this);
            Init();
        }

        protected virtual void Init()
        {
        
        }
    }
}