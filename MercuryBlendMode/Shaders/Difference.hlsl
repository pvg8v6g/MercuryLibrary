sampler2D Input : register(s0);
sampler2D Bkg : register(s1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 src = tex2D(Input, uv);
    float4 bkg = tex2D(Bkg, uv);
    
    float3 result = abs(src.rgb - bkg.rgb);
    return float4(result * src.a, src.a);
}
