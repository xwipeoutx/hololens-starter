Shader "Steve/Diffuse"
{
    Properties
    {
        _Color ("Albedo", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
            };

            float4 _Color;
            
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 worldNormal = UnityObjectToWorldNormal(i.normal).xyz;
                fixed4 albedo = tex2D(_MainTex, i.uv) * _Color;
                                
                fixed ambient = unity_AmbientSky;
                fixed diffuse = saturate(dot(_WorldSpaceLightPos0, worldNormal));
                
                return ambient * albedo
                     + diffuse * albedo * _LightColor0;
            }
            ENDCG
        }
    }
}
