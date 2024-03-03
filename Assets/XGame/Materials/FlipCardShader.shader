Shader "Custom/FlipCardShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_FlipProcess("Process",Range(0.6,1)) = 0
		_IsLeftShort("IsLeftShort",int) = 0
		_YoffSet("Y_offSet",float)=0
    }
    SubShader
    {
        // No culling or depth
		Tags{"Queue" = "Transparent" "RenderType" = "Transparent"}
        Cull Off ZWrite Off ZTest Always
	
        Pass
        {
		Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			float _FlipProcess;
			float _IsLeftShort;
			float _YoffSet;
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
				half2 uv = v.uv;
				v.vertex.y -= _YoffSet;
				if (_IsLeftShort >0)
				{
					v.vertex.y = v.vertex.y*((1- _FlipProcess)*uv.x+ _FlipProcess);
				}
				else
				{
					v.vertex.y = v.vertex.y*((_FlipProcess-1)* uv.x + 1);
				}
				v.vertex.y += _YoffSet;
                o.vertex = UnityObjectToClipPos(v.vertex);
				
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                //col.rgb = 1 - col.rgb;
				return col;
            }
            ENDCG
        }
    }
}
