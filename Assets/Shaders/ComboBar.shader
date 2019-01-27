// most of this is straight up copy-pasted from unity's built in sprite shader :)
Shader "Custom/ComboBar"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (1,1,1,1)
        _Color3 ("Color 3", Color) = (1,1,1,1)
        _FillPercent ("Fill Percent", Range(0, 1)) = 0.0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment ComboBarFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            
            #include "UnitySprites.cginc"

            float4 _MainTex_TexelSize;
            float _FillPercent;
            half4 _Color1;
            half4 _Color2;
            half4 _Color3;

            // Returns 1 if min <= t <= max, 0 otherwise
            float between (float t, float min, float max) {
                return step(min, t) * step(t, max);
            }

            // Blends colors a and b based on t's position between min and max
            // Returns black if t is not inside min & max
            half3 BlendInRange (float t, float min, float max, half3 a, half3 b) {
                float blend = (t - min) / (max - min);
                blend = clamp(blend, 0.0, 1.0);
                return between(t, min, max) * lerp(a, b, blend);
            }

            // Evaluates gradient with 3 colors
            half3 Gradient(float t, float min, float max) {
                half3 color = half3(0,0,0);
                color += BlendInRange(t, min, 0.5, _Color1.rgb, _Color2.rgb);
                color += BlendInRange(t, 0.5, max, _Color2.rgb, _Color3.rgb);
                return color;
            }

            fixed4 ComboBarFrag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture (IN.texcoord);
                //c.rgb *= c.a;

                // tint this pixel if it's past the fill line (on x axis)
                half4 color = half4(Gradient(IN.texcoord.x, 0, 1), 1.0);
                int applyFill = IN.texcoord.x <= _FillPercent;
                c = applyFill*(color) + (1-applyFill)*c;

                //return half4(IN.texcoord.x, 0, 0, 1);
                return c;
            }

        ENDCG
        }
    }
}
