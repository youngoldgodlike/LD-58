Shader "Custom/LightningPulse"
{
    Properties
    {
        _MainTex ("Lightning Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Color ("Lightning Color", Color) = (0.5, 0.7, 1, 1)
        _HDRColor ("HDR Glow Color", Color) = (1, 3, 5, 1)
        _ScrollSpeed ("Scroll Speed", Range(0, 10)) = 3
        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 5
        _FlickerSpeed ("Flicker Speed", Range(0, 20)) = 10
        _NoiseScale ("Noise Scale", Range(0.1, 5)) = 1.5
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.6
        _Distortion ("Distortion", Range(0, 0.5)) = 0.15
        _CoreWidth ("Core Width", Range(0.01, 0.3)) = 0.1
        _BoltCount ("Bolt Count", Range(1, 5)) = 2
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Blend One One
        ZWrite Off
        Cull Off

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
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _HDRColor;
            float _ScrollSpeed;
            float _PulseSpeed;
            float _FlickerSpeed;
            float _NoiseScale;
            float _NoiseStrength;
            float _Distortion;
            float _CoreWidth;
            float _BoltCount;

            // Процедурная функция шума
            float noise(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                // Эффект мерцания (быстрое случайное включение/выключение)
                float flicker = noise(floor(_Time.y * _FlickerSpeed) * 0.1);
                flicker = step(0.3, flicker);
                
                // Прокрутка текстуры шума
                float2 noiseUV = uv * _NoiseScale;
                noiseUV.x += _Time.y * _ScrollSpeed;
                noiseUV.y += sin(_Time.y * 2.0 + uv.x * 10.0) * 0.1;
                
                // Генерация шума для искажения
                float noiseValue = tex2D(_NoiseTex, noiseUV).r;
                float noiseValue2 = tex2D(_NoiseTex, noiseUV * 1.5 + float2(0.5, 0.5)).r;
                
                // Комбинирование нескольких слоев шума
                float combinedNoise = noiseValue * 0.6 + noiseValue2 * 0.4;
                
                // Искажение UV для создания зигзагообразной молнии
                float distortAmount = (combinedNoise - 0.5) * _Distortion;
                uv.y += distortAmount * sin(uv.x * 20.0 + _Time.y * 3.0);
                
                // Создание нескольких ветвей молнии
                float bolt = 0.0;
                for(float b = 0; b < _BoltCount; b++)
                {
                    float offset = (b / _BoltCount - 0.5) * 0.3;
                    float dist = abs(uv.y - 0.5 - offset);
                    
                    // Добавление вариаций в ширину молнии
                    float widthMod = noise(float2(uv.x * 10.0 + b, _Time.y * 2.0)) * 0.5 + 0.5;
                    float boltWidth = _CoreWidth * widthMod;
                    
                    // Создание яркого ядра молнии
                    float core = 1.0 - smoothstep(0.0, boltWidth, dist);
                    float glow = 1.0 - smoothstep(0.0, boltWidth * 3.0, dist);
                    
                    bolt += core * 3.0 + glow;
                }
                
                // Эффект пульсации
                float pulse = sin(_Time.y * _PulseSpeed + uv.x * 5.0) * 0.3 + 0.7;
                pulse *= sin(_Time.y * _PulseSpeed * 1.7) * 0.2 + 0.8;
                
                // Затухание на концах
                float edgeFade = smoothstep(0.0, 0.1, uv.x) * smoothstep(1.0, 0.9, uv.x);
                
                // Добавление шума к общей яркости
                float noiseMask = pow(combinedNoise, 2.0) * _NoiseStrength;
                bolt *= (1.0 - _NoiseStrength + noiseMask);
                
                // Финальный цвет
                fixed4 finalColor = _Color * _HDRColor;
                finalColor.rgb *= bolt * pulse * edgeFade * flicker;
                finalColor.a = saturate(bolt * edgeFade);
                
                return finalColor;
            }
            ENDCG
        }
    }
}
