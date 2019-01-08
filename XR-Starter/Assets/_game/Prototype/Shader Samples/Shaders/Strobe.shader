Shader "Steve/Strobe"
{
    Properties
    {
        _Color ("Albedo", Color) = (1,1,1,1)
        _StrobeColor ("Strobe Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        
        _Smoothness("Smoothness", Range(0,1)) = 0.5
        _FresnelPower("Fresnel Power", Range(0,4)) = 1
    }
    SubShader
    {
            Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" "PerformanceChecks" = "False" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma target 5.0
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
                float3 worldPosition : TEXCOORD2;
            };

            float4 _Color;
            float4 _StrobeColor;
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            fixed _Smoothness;
            fixed _FresnelPower;
            
            static const fixed _Shininess = 800.0;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {            
                fixed3 worldNormal = UnityObjectToWorldNormal(i.normal).xyz;
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPosition.xyz));
                
                fixed strobeAmount = sin(_Time.y)/2 + 0.5;
                fixed4 albedo = tex2D(_MainTex, i.uv) * lerp(_Color, _StrobeColor, strobeAmount);
                fixed ambient = unity_AmbientSky;
                
                fixed diffuse = saturate(dot(_WorldSpaceLightPos0, worldNormal));
                       
                #if defined(_BLINN_PHONG) 
                // Optimisation which doesn't make total physical sense
                fixed halfVector = max(0.0, dot(worldNormal, normalize(_WorldSpaceLightPos0 + worldViewDir)));
                fixed specular = saturate(pow(halfVector, 4 * _Shininess * pow(_Smoothness, 4.0)) * _Smoothness);
                #else
                fixed3 reflectionVector = 2 * dot(_WorldSpaceLightPos0, worldNormal)*worldNormal - _WorldSpaceLightPos0;
                fixed reflectionImpact = saturate(dot(reflectionVector, worldViewDir));
                fixed specular = saturate(pow(reflectionImpact, _Shininess * pow(_Smoothness, 4.0)) * _Smoothness);
                #endif
                
                // Rim / Fresnel
                fixed fresnel = 1 - saturate(dot(worldNormal, worldViewDir));
                fresnel = pow(fresnel, _FresnelPower);
                
                return ambient * albedo 
                     + diffuse * albedo * _LightColor0
                     + specular * _LightColor0
                     + fresnel * unity_AmbientSky * (1-_Smoothness);
            }
            ENDCG
        }
    }
}
