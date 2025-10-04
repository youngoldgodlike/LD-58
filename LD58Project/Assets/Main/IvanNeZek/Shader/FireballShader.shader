Shader "Custom/FireballUnlit"
{
    Properties
    {
        _FireColor ("Fire Inner Color", Color) = (1, 0.8, 0, 1)
        _FireEdgeColor ("Fire Edge Color", Color) = (1, 0.3, 0, 1)
        _FireDarkColor ("Fire Dark Color", Color) = (0.3, 0, 0, 1)
        _NoiseScale ("Noise Scale", Float) = 3.0
        _NoiseSpeed ("Noise Speed", Float) = 0.5
        _DistortionStrength ("Distortion Strength", Float) = 0.3
        _FirePower ("Fire Power", Float) = 2.0
        _Brightness ("Brightness", Float) = 2.0
        _FresnelPower ("Fresnel Power", Float) = 3.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
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
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
            };

            float4 _FireColor;
            float4 _FireEdgeColor;
            float4 _FireDarkColor;
            float _NoiseScale;
            float _NoiseSpeed;
            float _DistortionStrength;
            float _FirePower;
            float _Brightness;
            float _FresnelPower;

            // Simple 3D Noise функция
            float hash(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            float noise(float3 x)
            {
                float3 i = floor(x);
                float3 f = frac(x);
                f = f * f * (3.0 - 2.0 * f);

                return lerp(lerp(lerp(hash(i + float3(0,0,0)), 
                                      hash(i + float3(1,0,0)), f.x),
                                 lerp(hash(i + float3(0,1,0)), 
                                      hash(i + float3(1,1,0)), f.x), f.y),
                            lerp(lerp(hash(i + float3(0,0,1)), 
                                      hash(i + float3(1,0,1)), f.x),
                                 lerp(hash(i + float3(0,1,1)), 
                                      hash(i + float3(1,1,1)), f.x), f.y), f.z);
            }

            // Fractal Brownian Motion для более сложного шума
            float fbm(float3 x)
            {
                float v = 0.0;
                float a = 0.5;
                float3 shift = float3(100, 100, 100);
                
                for (int i = 0; i < 4; ++i)
                {
                    v += a * noise(x);
                    x = x * 2.0 + shift;
                    a *= 0.5;
                }
                return v;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float time = _Time.y * _NoiseSpeed;
                
                // Создаём 3 октавы шума с разными скоростями
                float3 noiseCoord1 = i.worldPos * _NoiseScale + float3(0, time, 0);
                float3 noiseCoord2 = i.worldPos * (_NoiseScale * 2.0) + float3(0, time * 1.5, 0);
                float3 noiseCoord3 = i.worldPos * (_NoiseScale * 4.0) + float3(0, time * 2.0, 0);
                
                float noise1 = fbm(noiseCoord1);
                float noise2 = fbm(noiseCoord2) * 0.5;
                float noise3 = fbm(noiseCoord3) * 0.25;
                
                // Комбинируем шумы
                float combinedNoise = noise1 + noise2 + noise3;
                
                // Искажаем UV на основе шума
                float2 distortion = float2(
                    fbm(i.worldPos * _NoiseScale + time * 0.3),
                    fbm(i.worldPos * _NoiseScale + time * 0.5 + 50.0)
                ) * 2.0 - 1.0;
                
                float distortedNoise = fbm(i.worldPos * _NoiseScale + float3(distortion * _DistortionStrength, time));
                
                // Создаём маску на основе расстояния от центра (для сферы)
                float centerDist = length(i.uv - 0.5) * 2.0;
                float sphereMask = 1.0 - saturate(centerDist);
                sphereMask = pow(sphereMask, _FirePower);
                
                // Применяем шум к маске
                float fireMask = saturate(distortedNoise * sphereMask);
                
                // Создаём градиент огня
                float fireGradient = fireMask;
                float4 fireColor;
                
                if (fireGradient > 0.6)
                {
                    fireColor = lerp(_FireEdgeColor, _FireColor, (fireGradient - 0.6) / 0.4);
                }
                else if (fireGradient > 0.2)
                {
                    fireColor = lerp(_FireDarkColor, _FireEdgeColor, (fireGradient - 0.2) / 0.4);
                }
                else
                {
                    fireColor = _FireDarkColor * (fireGradient / 0.2);
                }
                
                // Добавляем Fresnel эффект для краёв
                float fresnel = pow(1.0 - saturate(dot(i.worldNormal, i.viewDir)), _FresnelPower);
                fireColor.rgb += _FireEdgeColor.rgb * fresnel * 0.5;
                
                // Устанавливаем альфа-канал
                float alpha = saturate(fireMask * 1.5);
                
                // Применяем яркость
                fireColor.rgb *= _Brightness;
                
                return float4(fireColor.rgb, alpha);
            }
            ENDCG
        }
    }
}
