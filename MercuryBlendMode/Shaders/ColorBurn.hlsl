sampler2D Input : register(s0);
sampler2D Bkg : register(s1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 src = tex2D(Input, uv);
    float4 bkg = tex2D(Bkg, uv);
    return saturate(1.0 - (1.0 - bkg) / src);
}
