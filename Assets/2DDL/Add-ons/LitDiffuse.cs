#if UNITY_EDITOR
namespace DynamicLight2D
{
	using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
    using DynamicLight2D;

	[ExecuteInEditMode]
	public class LitDiffuse : AddOnBase {

       


		// Tags array is used for search results in inspector
		public static string []tags = {"light", "diffuse", "sprite", "lit", "illumination"};
		
		// Brief description of behavior in this Add-on
		public static string description = "Illuminate sprites";


		[TitleAttribute("This script add a illumination by adding a Unity Point Light. \n Remember using materials like 'sprite-lit'",21f)]
		[SerializeField]GameObject pointLightGO = null;

		[Range(1f, 20f)] public float Falloff = 5f;
        [Range(0f, 10f)] public float Intensity = 7f;
        public Color Color;

        [ShowOnly] public List<GameObject> ConvertedElements;
        [DropArea(50f, "Drop objects to diffuse convertion", "DynamicLight2D.LitDiffuse", "Convert")] public bool dropAreaStuff;
       
        static void Convert(object[] o)
        {
            LitDiffuse.instance.SetObjectsToLitMaterial(o[0]);
        }


        Light _pointLight_component;
        Material LitMaterial;

        public static LitDiffuse instance;
        public override void Start () {
			base.Start();

            if (instance == null)
                instance = this;

            ConvertedElements = new List<GameObject>(5);
            Transform _t = gameObject.transform.Find("UnityPointLight");

            string _path = CoreUtils.MainPath();
            LitMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath(_path + "Prefabs/Casters/Materials/SpriteLit.mat", typeof(Material)) as Material;


            if (_t == null)
            {

                // CREATING AND SETTING GO
                pointLightGO = new GameObject("UnityPointLight")
                {
                    hideFlags = HideFlags.HideInHierarchy
                };

				pointLightGO.transform.parent = gameObject.transform;
				Vector3 _p = pointLightGO.transform.position;
				_p.x = 0;
				_p.y = 0;
				_p.z = -1f;
				pointLightGO.transform.localPosition = _p;
				pointLightGO.transform.localEulerAngles = new Vector3(-90,0,0);
				pointLightGO.transform.localScale = Vector3.one;
				_t = pointLightGO.transform;


				// ADDING POINT LIGHT
				_pointLight_component = pointLightGO.transform.GetComponent<Light>();
				if(_pointLight_component == null) _pointLight_component = pointLightGO.AddComponent<Light>();

				_pointLight_component.intensity = dynamicLightInstance.Intensity;

                // Init Color
                _pointLight_component.color = dynamicLightInstance.LightColor;
                Color = dynamicLightInstance.LightColor;

                //Ignore middle vertices false
                dynamicLightInstance.intelliderConvex = false;
            }

            // Convert objects to LIT
            //SetObjectsToLitMaterial());

            pointLightGO = _t.gameObject;

		}
       
       
        public void SetObjectsToLitMaterial(object obj)
        {
            GameObject theObject = (GameObject)obj;
            if (theObject.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer sr = theObject.GetComponent<SpriteRenderer>();
                //set Fog material
                sr.material = LitMaterial;

                //Trace objects
                ConvertedElements.Add(theObject);
            }

        }

       

        public override void Update()
		{	

			if(_pointLight_component == null && pointLightGO != null) _pointLight_component = pointLightGO.GetComponent<Light>();
			if (_pointLight_component == null)
				return;

			// Angle
			if (this.dynamicLightInstance.RangeAngle < 180) {
				_pointLight_component.type = LightType.Spot;
				_pointLight_component.spotAngle = Mathf.Clamp(dynamicLightInstance.RangeAngle, 1f, 179f);
			} else {
				_pointLight_component.type = LightType.Point;
			}


			//Radius
			_pointLight_component.range = dynamicLightInstance.LightRadius * Falloff;
            
            //Radius
            _pointLight_component.intensity = Intensity;

            
            //Color
            if (_pointLight_component.color != Color)
                _pointLight_component.color = Color;


        }
	
		void OnDestroy () {

            for (int i = 0; i < ConvertedElements.Count; i++)
            {
                if (ConvertedElements[i].GetComponent<SpriteRenderer>())
                {
                    ConvertedElements[i].GetComponent<SpriteRenderer>().material = new Material(Shader.Find("Sprites/Default"));
                }

            }

            if (pointLightGO != null)
				DestroyImmediate(pointLightGO);
		}
	}
}
#endif

