Shader "Unlit/No Cull"
{
	Properties
	{
		_Color("Main Color", Color) = (1.0, 0.0, 0.0, 0.0)
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			fixed4 _Color;

			float4 vert (appdata v) : SV_POSITION
			{
				return UnityObjectToClipPos(v.vertex);
			}
			
			fixed4 frag () : SV_Target
			{
				return _Color;
			}
			ENDCG
		}
	}
}
