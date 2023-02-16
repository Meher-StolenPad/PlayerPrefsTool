Shader "Custom/YPositionColor" {
    Properties {
        _TopColor ("Top Color", Color) = (1, 0, 0, 1)
        _BottomColor ("Bottom Color", Color) = (0, 0, 1, 1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Standard
        #include "UnityCG.cginc"

        struct Input {
            float3 worldPos;
        };

        fixed4 _TopColor;
        fixed4 _BottomColor;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Remap the Y position to a value between 0 and 1
            float yRemapped = saturate((IN.worldPos.y - 1.0) / 10.0);

            // Set the color based on the Y position
            o.Albedo = lerp(_TopColor, _BottomColor, yRemapped);

            // Set the metallic and smoothness values to defaults
            o.Metallic = 0;
            o.Smoothness = 0.5;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
