sampler2D Input : register(s0);
sampler2D Bkg : register(s1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 src = tex2D(Input, uv);
    float4 bkg = tex2D(Bkg, uv);
    
    // Result = Src * Bkg
    // We add (1-src.a)*bkg to keep background visible where src is transparent
    float4 res;
    res.rgb = (src.rgb * bkg.rgb) + bkg.rgb * (1.0 - src.a);
    res.a = bkg.a;
    
    return saturate(res);
}
