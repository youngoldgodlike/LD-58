Shader "Custom/MinecraftPortal"
{
    Properties
    {
        _MainTex ("Portal Texture", 2D) = "white" {}
        _SecondTex ("Second Layer", 2D) = "white" {}
        _PortalColor ("Portal Color", Color) = (0.5, 0, 1, 1)
        _Speed ("Animation Speed", Float) = 0.2
        _Distortion ("Distortion", Float) = 0.5
        _Brightness ("Brightness", Float) = 2.0
        _Transparency ("Transparency", Float) = 0.8
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            TEXTURE2D(_SecondTex);
            SAMPLER(sampler_SecondTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _SecondTex_ST;
                float4 _PortalColor;
                float _Speed;
                float _Distortion;
                float _Brightness;
                float _Transparency;
            CBUFFER_END
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uv2 = TRANSFORM_TEX(IN.uv, _SecondTex);
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                // Анимация первого слоя с вращением и искажением
                float2 centeredUV = IN.uv - 0.5;
                float angle = _Time.y * _Speed;
                float s = sin(angle);
                float c = cos(angle);
                float2x2 rotationMatrix = float2x2(c, -s, s, c);
                float2 rotatedUV1 = mul(rotationMatrix, centeredUV) + 0.5;
                
                // Добавляем волновое искажение
                float2 distortedUV1 = rotatedUV1;
                distortedUV1.x += sin(rotatedUV1.y * 10.0 + _Time.y * _Speed * 2) * _Distortion * 0.05;
                distortedUV1.y += cos(rotatedUV1.x * 10.0 + _Time.y * _Speed * 2) * _Distortion * 0.05;
                
                // Второй слой с другой скоростью вращения
                float angle2 = _Time.y * _Speed * -0.7;
                float s2 = sin(angle2);
                float c2 = cos(angle2);
                float2x2 rotationMatrix2 = float2x2(c2, -s2, s2, c2);
                float2 rotatedUV2 = mul(rotationMatrix2, centeredUV) + 0.5;
                
                float2 distortedUV2 = rotatedUV2;
                distortedUV2.x += sin(rotatedUV2.y * 8.0 - _Time.y * _Speed * 1.5) * _Distortion * 0.03;
                distortedUV2.y += cos(rotatedUV2.x * 8.0 - _Time.y * _Speed * 1.5) * _Distortion * 0.03;
                
                // Сэмплируем текстуры
                half4 tex1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV1);
                half4 tex2 = SAMPLE_TEXTURE2D(_SecondTex, sampler_SecondTex, distortedUV2);
                
                // Смешиваем слои
                half4 combined = (tex1 + tex2) * 0.5;
                
                // Применяем цвет портала
                half4 finalColor = combined * _PortalColor;
                
                // Добавляем эффект свечения от центра
                float2 centerDist = IN.uv - 0.5;
                float distFromCenter = length(centerDist);
                float glow = 1.0 - saturate(distFromCenter * 2.0);
                glow = pow(glow, 2.0);
                
                finalColor.rgb += glow * _PortalColor.rgb * 0.5;
                finalColor.rgb *= _Brightness;
                
                // Применяем прозрачность
                finalColor.a = combined.a * _Transparency;
                
                return finalColor;
            }
            ENDHLSL
        }
    }
    
    Fallback Off
}
