Shader "Custom/ColorToTransparency"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // The input texture
        _TargetColor ("Color to Remove", Color) = (0,1,0,1) // Default: Green
        _Tolerance ("Tolerance", Range(0, 1)) = 0.1 // Allow slight variations
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Enables transparency
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float4 _TargetColor;
            float _Tolerance;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Calculate color difference
                float diff = distance(col.rgb, _TargetColor.rgb);
                if (diff < _Tolerance)
                {
                    col.a = 0; // Make transparent
                }
                return col;
            }
            ENDCG
        }
    }
}
