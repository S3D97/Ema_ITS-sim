Shader "Unlit/DepthTest"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _DepthTex ("Depth Tex", 2D) = "white" {}
        _DepthInfluence ("Depth Influence", Range(0, 1)) = 0.5
        _ParallaxScale ("Parallax Scale", Range(0, 0.1)) = 0.01
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
            sampler2D _DepthTex;
            float4 _MainTex_ST;
            float4 _MousePos;

            float _DepthInfluence;
            float _ParallaxScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float d = tex2D(_DepthTex, i.uv).r; 
                float2 parallax = _MousePos.xy * d * _ParallaxScale;
                float2 uvWithParallax = i.uv + parallax;

                fixed4 mainColor = tex2D(_MainTex, uvWithParallax);
                mainColor.rgb *= lerp(1.0, d, _DepthInfluence);

                return mainColor;
               
                
            }
            ENDCG
        }
    }
}
