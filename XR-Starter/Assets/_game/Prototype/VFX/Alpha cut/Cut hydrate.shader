Shader "Steve/Cutout"
{
	Properties
	{
		_Color ("Albedo", Color) = (1,1,1,1)
		
		_SweepTex ("Sweep Map", 2D) = "white" {}
		_SweepCutoff("Sweep Cutoff", Range(0,1)) = 0.5
		_SweepColor("Sweep Color", Color) = (0,1,0,1)
	}

	SubShader
	{
		Pass
		{
			Tags { "Queue" = "Transparent" "RenderType" = "TransparentCutout" }
            LOD 100
			
			Blend SrcAlpha OneMinusSrcAlpha
			
			LOD 200

			CGPROGRAM

			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			fixed4 _Color;
			fixed4 _Ambient;
			fixed4 _LightColor0;
			
			sampler2D _SweepTex;
			fixed4 _SweepTex_ST;
			fixed _SweepCutoff;
			fixed4 _SweepColor;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed3 normal : NORMAL;
			    UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f 
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 worldPosition : TEXCOORD2;
				fixed3 worldNormal : TEXCOORD3;
                
			    UNITY_VERTEX_OUTPUT_STEREO
			};

			
			v2f vert(appdata_t v)
			{
				v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				
				o.uv = TRANSFORM_TEX(v.uv, _SweepTex);
						
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
				o.position = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{			
                fixed4 sweepValue = tex2D (_SweepTex, i.uv);
                fixed distance = 1 - saturate((sweepValue - _SweepCutoff) * 20);
                clip(distance - 0.1);
				
				fixed3 worldNormal = normalize(i.worldNormal);
				
                fixed4 albedo = _Color;
				
                fixed diffuse = max(0.0, dot(worldNormal, _WorldSpaceLightPos0));
                fixed4 output = albedo;
                
                output.rgb *= unity_AmbientSky.rgb * 1.5 + (albedo.rgb *_LightColor0.rgb * diffuse);                
                output.rgb = lerp(_SweepColor, output.rgb, distance);
				
				return output;
			}
			
			ENDCG
		}
	}
	
	FallBack "VertexLit"
}
