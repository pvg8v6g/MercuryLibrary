sampler2D Input : register(s0);
sampler2D Bkg : register(s1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 src = tex2D(Input, uv);
    float4 bkg = tex2D(Bkg, uv);
    
    // Standard Additive: Result = Bkg + Src
    float4 res = bkg + src;
    
    // Clamp result
    return saturate(res);
}
