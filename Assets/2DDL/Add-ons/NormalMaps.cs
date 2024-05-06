#if UNITY_EDITOR
namespace DynamicLight2D
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections;
    using System.Collections.Generic;
    using DynamicLight2D;

    [ExecuteInEditMode]
    public class NormalMaps : AddOnBase
    {




        // Tags array is used for search results in inspector
        public static string[] tags = { "light", "diffuse", "normal", "map", "bump", "lit", "illumination" };

        // Brief description of behavior in this Add-on
        public static string description = "Normal maps on sprites";


        [TitleAttribute("This script add a illumination with Bump material to get working Normal Maps in sprites", 21f)]


        [Range(1f, 20f)] public float Falloff = 5f;
        [Range(0f, 10f)] public float Intensity = 7f;
        public Color Color;


        [DropArea(50f, "Drop objects to Bump Diffuse convertion", "DynamicLight2D.NormalMaps", "Convert")] public bool dropAreaStuff;



        // PRIVATES

        private GameObject pointLightGO = null;
        private List<GameObject> ConvertedElements;

        static void Convert(object[] o)
        {
            NormalMaps.instance.SetObjectsToLitMaterial(o[0]);

        }

        //store all temp materials
        List<Material> _tmpMaterials;

        Light _pointLight_component;
        Material LitMaterial;

        public static NormalMaps instance;
        public override void Start()
        {
            base.Start();

            if (instance == null)
                instance = this;

            ConvertedElements = new List<GameObject>(5);

            _tmpMaterials = new List<Material>(5);

            Transform _t = gameObject.transform.Find("UnityPointLight");

            string _path = CoreUtils.MainPath();
            LitMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath(_path + "Prefabs/Casters/Materials/SpriteBumping.mat", typeof(Material)) as Material;


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
                pointLightGO.transform.localEulerAngles = new Vector3(-90, 0, 0);
                pointLightGO.transform.localScale = Vector3.one;
                _t = pointLightGO.transform;


                // ADDING POINT LIGHT
                _pointLight_component = pointLightGO.transform.GetComponent<Light>();
                if (_pointLight_component == null) _pointLight_component = pointLightGO.AddComponent<Light>();

                _pointLight_component.intensity = dynamicLightInstance.Intensity;

                // Init Color
                _pointLight_component.color = dynamicLightInstance.LightColor;
                Color = dynamicLightInstance.LightColor;

                //Ignore middle vertices false
                dynamicLightInstance.intelliderConvex = false;
            }



            pointLightGO = _t.gameObject;

        }


        public void SetObjectsToLitMaterial(object obj)
        {
            GameObject theObject = (GameObject)obj;
            if (theObject.GetComponent<SpriteRenderer>())
            {
                SpriteRenderer sr = theObject.GetComponent<SpriteRenderer>();
                //set Fog material
                Material NewMaterial = new Material(LitMaterial);

                sr.material = NewMaterial;
                //sr.material.CopyPropertiesFromMaterial(LitMaterial);


                //Create a Normal
                Texture2D _normal = NormalMap(sr.sprite.texture, 4f);

                //Make sure to enable the Keywords
                NewMaterial.EnableKeyword("_NORMALMAP");

                // Set the Normal map using the Texture (invoke twice avoid no update issue)

                NewMaterial.SetTexture("_MainTex", sr.sprite.texture);
                NewMaterial.SetTexture("_BumpMap", _normal);

                //UnityEditor.EditorUtility.SetDirty(LitMaterial);

                //Trace objects
                ConvertedElements.Add(theObject);

                //Trace materials
                _tmpMaterials.Add(NewMaterial);
            }

        }



        public override void Update()
        {

            if (_pointLight_component == null && pointLightGO != null) _pointLight_component = pointLightGO.GetComponent<Light>();
            if (_pointLight_component == null)
                return;

            // Angle
            if (this.dynamicLightInstance.RangeAngle < 180)
            {
                _pointLight_component.type = LightType.Spot;
                _pointLight_component.spotAngle = Mathf.Clamp(dynamicLightInstance.RangeAngle, 1f, 179f);
            }
            else
            {
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

        void OnDestroy()
        {
            if (!BuildPipeline.isBuildingPlayer)
            {
                Material diffuse = new Material(Shader.Find("Sprites/Default"));

                if (ConvertedElements != null)
                {

                    for (int i = 0; i < ConvertedElements.Count; i++)
                    {
                        if (ConvertedElements[i] == null)
                            continue;

                        if (ConvertedElements[i].GetComponent<SpriteRenderer>())
                        {
                            ConvertedElements[i].GetComponent<SpriteRenderer>().sharedMaterial = diffuse;
                        }

                    }
                }

            }




            if (pointLightGO != null)
                DestroyImmediate(pointLightGO);
        }

        private Texture2D NormalMap(Texture2D source, float strength)
        {
            strength = Mathf.Clamp(strength, 0.0F, 5.0F);

            Texture2D normalTexture;
            float xLeft;
            float xRight;
            float yUp;
            float yDown;
            float yDelta;
            float xDelta;

            //SET READEABLE ORIGINAL TEX
            string path = AssetDatabase.GetAssetPath(source);


            TextureImporter A = (TextureImporter)AssetImporter.GetAtPath(path);
            A.isReadable = true;

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            normalTexture = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);

            for (int y = 0; y < normalTexture.height; y++)
            {
                for (int x = 0; x < normalTexture.width; x++)
                {
                    float green = 1f;
                    float blue = 1f;

                    if (source.GetPixel(x, y).a <= .01f)
                    {
                        normalTexture.SetPixel(x, y, new Color(0f, 0f, 0f, 1f));
                        continue;
                    }


                    if (source.GetPixel(x, y).grayscale > 0f && source.GetPixel(x, y).grayscale < .1f)
                    {
                        green = 1.3f;
                        blue = .2f;
                    }

                    xLeft = source.GetPixel(x - 1, y).grayscale * strength;
                    xRight = source.GetPixel(x + 1, y).grayscale * strength;
                    yUp = source.GetPixel(x, y - 1).grayscale * strength;
                    yDown = source.GetPixel(x, y + 1).grayscale * strength;
                    xDelta = ((xLeft - xRight) + 1) * 0.5f;
                    yDelta = ((yUp - yDown) + 1) * 0.5f;
                    normalTexture.SetPixel(x, y, new Color(xDelta, yDelta * green, 1.0f * blue, 1.0f));
                }
            }

            normalTexture.Apply();

            // System.IO.Directory.CreateDirectory("Assets/_NormalTextures/");
            // System.IO.File.WriteAllBytes("Assets/_NormalTextures/" + source.name + ".png", normalTexture.EncodeToPNG());





            return normalTexture;
        }
    }
}
#endif

