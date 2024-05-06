using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicLight2D;


[ExecuteInEditMode]
public class OccludedUR_passToShader : MonoBehaviour
{

    public DynamicLight Light2D;
    public float r;

    Material m;
    void Start()
    {
        m = GetComponent<SpriteRenderer>().sharedMaterial;
    }


    //Vector2 screenPos;

    void LateUpdate()
    {

        if (m == null) return;
        if (Light2D == null) return;

       

        ///screenPos = transform.position;

        //print(screenPos);

        m.SetFloat("_lightX", Light2D.transform.position.x);
        m.SetFloat("_lightY", Light2D.transform.position.y);
        m.SetFloat("RadiusOfLight2D", Light2D.LightRadius);
        r = Light2D.LightRadius;

    }
}
