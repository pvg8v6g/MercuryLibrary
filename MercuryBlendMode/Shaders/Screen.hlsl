sampler2D Input : register(s0);
sampler2D Bkg : register(s1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 src = tex2D(Input, uv);
    float4 bkg = tex2D(Bkg, uv);
    
    // Standard Screen: Result = 1 - (1-Src) * (1-Bkg)
    float4 res = 1.0 - (1.0 - src) * (1.0 - bkg);
    
    return saturate(res);
}
