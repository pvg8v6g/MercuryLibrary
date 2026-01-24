sampler2D Input : register(s0);
sampler2D Bkg : register(s1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 src = tex2D(Input, uv);
    float4 bkg = tex2D(Bkg, uv);
    
    float4 res;
    res.r = (src.r < 0.5) ? (2.0 * src.r * bkg.r) : (1.0 - 2.0 * (1.0 - src.r) * (1.0 - bkg.r));
    res.g = (src.g < 0.5) ? (2.0 * src.g * bkg.g) : (1.0 - 2.0 * (1.0 - src.g) * (1.0 - bkg.g));
    res.b = (src.b < 0.5) ? (2.0 * src.b * bkg.b) : (1.0 - 2.0 * (1.0 - src.b) * (1.0 - bkg.b));
    res.a = src.a;
    
    return saturate(res);
}
