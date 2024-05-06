#if UNITY_EDITOR
namespace DynamicLight2D
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
    [ExecuteInEditMode]
#endif


    public class Fog : AddOnBase
    {
       
        // Tags array is used for search results in search field
        public static string[] tags = { "visibility", "fog", "sight", "enemy", "character", "fov", "mask", "field", "view" };

        // Brief description of behavior in this Add-on
        public static string description = "Use the light as Fog of war technique";

        [ShowOnly] public List<GameObject> ConvertedElements;

        [DropArea(50f, "Drop objects to Fog material conversion", "DynamicLight2D.Fog", "Convert")] public bool dropAreaStuff;



        public static Fog instance;

        Material objMaterial;
        Material lightMaterial;
        Material LastLightMaterial;
        int LastSegments;
        bool LastSolid;
        bool LastMiddleVertex;
        Color LastColor;
        string Path;
        public override void Start()
        {
            base.Start();

            if (instance == null)
                instance = this;

            base.Start();

            ConvertedElements = new List<GameObject>(5);

            Path = CoreUtils.MainPath();
            objMaterial = AssetDatabase.LoadAssetAtPath(Path + "Prefabs/Casters/Materials/SpriteOccluded.mat", typeof(Material)) as Material;
            lightMaterial = AssetDatabase.LoadAssetAtPath(Path + "Prefabs/Lights/Materials/StandardOccluderMaterial.mat", typeof(Material)) as Material;
            LastLightMaterial = dynamicLightInstance.LightMaterial;
            LastColor = dynamicLightInstance.LightColor;
            LastSolid = dynamicLightInstance.SolidColor;
            LastSegments = dynamicLightInstance.Segments;
            LastMiddleVertex = dynamicLightInstance.intelliderConvex;
            dynamicLightInstance.LightMaterial = lightMaterial;
            dynamicLightInstance.Segments = 20;
            dynamicLightInstance.SolidColor = true;
            dynamicLightInstance.intelliderConvex = false;

        }


        // Update is called once per frame
        public override void Update()
        {

            base.Update();

          
        }

        static void Convert(object[] objs)
        {
            Fog.instance.SetFogMaterial(objs[0]);
        }

        public void SetFogMaterial(object obj) {
            //Convert to GameObject
            GameObject theObject = (GameObject)obj;

            

            if (theObject.GetComponent<SpriteRenderer>()) {
                SpriteRenderer sr = theObject.GetComponent<SpriteRenderer>();
                //set Fog material
                sr.material = objMaterial;

                //Trace objects
                ConvertedElements.Add(theObject);
            }

        }

        // Called when delete add on
        void OnDestroy()
        {
            dynamicLightInstance.LightMaterial = LastLightMaterial;
            dynamicLightInstance.SolidColor = LastSolid;
            dynamicLightInstance.Segments = LastSegments;
            dynamicLightInstance.intelliderConvex = LastMiddleVertex;
            dynamicLightInstance.LightColor = LastColor;

            for (int i = 0; i < ConvertedElements.Count; i++)
            {
                if (ConvertedElements[i].GetComponent<SpriteRenderer>()) {
                    ConvertedElements[i].GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
                }
               
            }

        }



        

    }
}
#endif