Shader "Custom/OptimizedColor" {
    Properties{
        _Color("Color", Color) = (1.0,1.0,1.0,1.0)
        _MainTex("Albedo", 2D) = "white" 
    }
    SubShader{
        Tags
        {
            "RenderType" = "Opaque"
        }
            LOD 150

        Pass
        {
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
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = (v.uv * _MainTex_ST.xy) + _MainTex_ST.zw;
                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse" 
}