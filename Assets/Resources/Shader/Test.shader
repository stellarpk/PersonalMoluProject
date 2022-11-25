Shader "Custom/Test"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _AreaColor("Area Color",Color) = (1,1,1)
        _Center("Center",Vector) = (0,0,0,0)
        _Radius("Radius",Range(0,10)) = 4
        _Border("Border",Range(0,2)) = 0.5
    }
        SubShader
        {
            Tags {"RenderType" = "Opaque"}

            CGPROGRAM
    #pragma surface surf Lambert

            sampler2D _MainTex;
        fixed3 _AreaColor;
        float3 _Center;
        float _Radius;
        float _Border;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
        };
        void surf (Input IN, inout SurfaceOutput o)
        {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            float dist = distance(_Center, IN.worldPos);

            if (dist > _Radius && dist < (_Radius + _Border)) {
                o.Albedo = _AreaColor;
            }
            else {
                o.Albedo = c.rgb;
            }
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
