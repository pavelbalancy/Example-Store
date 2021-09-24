// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "UnnyWorld/Diamond" {
   Properties {
      _Color ("Color", Color) = (1,1,1,1)
      _CubeReflec("Reflection Map", Cube) = "" {}
      _CubeRefrac("Refraction Map", Cube) = "" {}
      _Add("Addictive Light", Range (0,1)) = 0.16
      _ColorScale("Color Scale", Range(0,5)) = 2
   }
   SubShader {
      Pass {
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
 
         // User-specified uniforms
         uniform samplerCUBE _CubeRefrac;
         uniform samplerCUBE _CubeReflec;
         uniform float4 _Color;
         uniform float _Add;
         uniform float _ColorScale;
         
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float3 normalDir : TEXCOORD0;
            float3 viewDir : TEXCOORD1;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject; 
 
            output.viewDir = mul(modelMatrix, input.vertex).xyz - _WorldSpaceCameraPos;
            output.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float3 reflectedDir = reflect(input.viewDir, normalize(input.normalDir));
            float4 refr = texCUBE(_CubeRefrac, reflectedDir);
            float4 refl = texCUBE(_CubeReflec, reflectedDir);
            float4 col = (2*refr*_Color + refl)*_ColorScale + _Add;
            return col;
         }
 
         ENDCG
      }
   }
   Fallback "Diffuse"
}