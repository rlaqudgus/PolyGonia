// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// reference: https://github.com/chavaloart/urp-multi-pass

Shader "2D Dynamic Lights/Masks/Occluded_URP"
{
    Properties
    {
        [PerRendererData] _MainTex ( "Sprite Texture", 2D ) = "white" {}
        _Color ( "Regular Color", Color ) = ( 1, 1, 1, 1 )
       // [MaterialToggle] PixelSnap ( "Pixel snap", Float ) = 0
        _OccludedColor ( "Masked Color", Color ) = ( 0, 0, 0, 0.5 )
		//_Position("Position", Vector) = (.0, .0, .0)
        _fallOff ("FallOff", float) = 0
    }


CGINCLUDE

struct appdata_t  
{
    float4 vertex   : POSITION;
    float4 color    : COLOR;
    float2 texcoord : TEXCOORD0;
};

struct v2f  
{
    float4 vertex   : SV_POSITION;
	float4 pixelWorld : TEXCOORD1;
	float4 lightPosWorld : TEXCOORD3;
    float4 lightPos : TEXCOORD4;
    float4 pixelPos : TEXCOORD5;
    float3 normalDir : TEXCOORD2;
    fixed4 color    : COLOR;
    half2 texcoord  : TEXCOORD0;
	
	
            
};

#include "UnityCG.cginc"
fixed4 _Color;
fixed4 _OccludedColor;  
sampler2D _MainTex;
float _lightX;
float _lightY;
float RadiusOfLight2D;
uniform float3 _Position;
float _fallOff;



v2f vert( appdata_t IN )  
{
    v2f OUT;
	UNITY_INITIALIZE_OUTPUT(v2f, OUT);
    OUT.texcoord = IN.texcoord;
    OUT.color = IN.color;
   
	OUT.pixelPos = UnityObjectToClipPos (IN.vertex);////mul (UNITY_MATRIX_MV, IN.vertex);
    OUT.vertex =  UnityObjectToClipPos (IN.vertex);//
    OUT.pixelWorld = mul (unity_ObjectToWorld, OUT.pixelPos);
    OUT.lightPos = mul(unity_WorldToObject, float4(_lightX,_lightY,0,0));
	OUT.lightPosWorld = float4(_lightX,_lightY,0,0);
   
    
   


    return OUT;
}

ENDCG



    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Name "Comparison1"
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "RenderPipeline" = "UniversalPipeline"
                "LightMode" = "UniversalForward" // This allow me do 2 passes in URP
            
            }

            Stencil
            {
                Ref 1
                Comp NotEqual
               
            }


        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
          
            #include "UnityCG.cginc"


            fixed4 frag( v2f IN ) : SV_Target
            {
                fixed4 c = tex2D( _MainTex, IN.texcoord ) * _Color;
                c.rgb *= c.a;
                return c;
            }
        ENDCG


        }


         Pass
        {
            Name "Comparison2"
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "RenderPipeline" = "UniversalPipeline"            
            }

            Stencil
            {
                Ref 1
                Comp Equal
               
            }


        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
         
            #include "UnityCG.cginc"


            fixed4 frag( v2f IN ) : SV_Target
            {
				
				float3 vertexToLightSource = IN.lightPosWorld.xyz - IN.pixelWorld.xyz;
				float distance = max(length(vertexToLightSource),0);
                fixed4 c = _OccludedColor;
                

                if(_fallOff < 0) _fallOff = 0;
                if(_fallOff > RadiusOfLight2D) _fallOff = RadiusOfLight2D;

                float attenuation = distance / (RadiusOfLight2D - _fallOff);
                attenuation *= dot(attenuation,attenuation);
				c = lerp(_OccludedColor,_Color, saturate(attenuation));
				fixed4 c2 = tex2D( _MainTex, IN.texcoord ) * c;
				 c2.rgb *= c2.a;
                return c2;
            }
        ENDCG


        }

    }
}