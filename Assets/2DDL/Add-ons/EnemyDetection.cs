
namespace DynamicLight2D
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
    [ExecuteInEditMode]
#endif


    public class EnemyDetection : AddOnBase
    {
        private enum TAction {
            Beep,
            BeepAndDestroy
        }

        [FieldDescriptionAttribute("Action to execute", "green")][SerializeField] TAction Action;
        //[Space(20f)]

        // Tags array is used for search results in search field
        public static string[] tags = { "visibility", "fog", "sight", "enemy", "character", "fov" };

        // Brief description of behavior in this Add-on
        public static string description = "Get notified when the light lit an object.";


        //[FieldDescriptionAttribute("DROP OBJECT TO BE DETECTABLE HERE", "green")] [SerializeField] GameObject[] Elements;
        [ShowOnly]public List<GameObject> Elements;
        [DropArea(50f, "Drop objects to be detectable", "DynamicLight2D.EnemyDetection", "Convert")] public bool dropAreaStuff;



        [FieldDescriptionAttribute("Invisible mesh")]
        public bool HideLightMesh = false;

        

        static void Convert(object[] Obj)
        {
            EnemyDetection.instance.SetDetectable(Obj[0]);
        }

        public static EnemyDetection instance;


        // Privates
        // Type of light
        DynamicLight.Light2DType _lastLightType;
        bool _eventsEnabled;

        // Use this for initialization
        public override void Start()
        {

            base.Start();

            if (instance == null)
                instance = this;

            _lastLightType = dynamicLightInstance.light2DType;
            _eventsEnabled = dynamicLightInstance.useEvents;

            dynamicLightInstance.OnEnterFieldOfView += EnemyDetection_Action;
            dynamicLightInstance.useEvents = true;

#if UNITY_EDITOR 
            if(Elements == null)
                Elements = new List<GameObject>(5);
#endif
        }


        // Update is called once per frame
        public override void Update()
        {

            base.Update();

            if (HideLightMesh && dynamicLightInstance.light2DType != DynamicLight.Light2DType.OnlySight)
            {
                dynamicLightInstance.light2DType = DynamicLight.Light2DType.OnlySight;
            }
            if (!HideLightMesh && dynamicLightInstance.light2DType == DynamicLight.Light2DType.OnlySight)
            {
                dynamicLightInstance.light2DType = DynamicLight.Light2DType.PreBaked;
            }


        }

        public void SetDetectable(object obj)
        {
            GameObject theObject = (GameObject)obj;
            Elements.Add(theObject);
        }

        // Called when delete add on
        void OnDestroy()
        {
            dynamicLightInstance.OnEnterFieldOfView -= EnemyDetection_Action;
            dynamicLightInstance.light2DType = _lastLightType;
            dynamicLightInstance.useEvents = _eventsEnabled;
        }



        // ---------------------------- CUSTOMIZE IT BY DEV NEEDS  ---------------------------- //
        //Action performed when enemy is detected by the light
        void EnemyDetection_Action(GameObject obj, DynamicLight dlight)
        {
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i] == null)
                    continue;

                // If you're dealing with the same object in the list
                if (Elements[i].GetHashCode() == obj.GetHashCode())
                {
                    #if UNITY_EDITOR
                    EditorApplication.Beep();
#endif
                    print("Hit");
                    if (Action == TAction.BeepAndDestroy)
                        DestroyImmediate(obj);
                }

            }
        }
        // ---------------------------- CUSTOMIZE IT BY DEV NEEDS  ---------------------------- //

    }
}
