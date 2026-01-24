sampler2D Input : register(s0);
sampler2D Bkg : register(s1);
float Mode : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 src = tex2D(Input, uv);
    float4 bkg = tex2D(Bkg, uv);
    
    float4 res = src;
    
    // In WPF, colors are usually premultiplied. 
    // To blend like Photoshop (additive, etc), we treat src.rgb as the light to add.
    
    if (Mode == 2) // ADD
    {
        // Simple Additive: Black disappears, brightness adds up.
        res.rgb = bkg.rgb + src.rgb;
        res.a = max(src.a, bkg.a);
    }
    else if (Mode == 3) // MULTIPLY
    {
        // For Multiply, we want src.rgb * bkg.rgb. 
        // If src is black (0,0,0), result is black. 
        // We use (1 - src.a) to handle the transparency outside the sprite.
        res.rgb = (src.rgb * bkg.rgb) + bkg.rgb * (1.0 - src.a);
        res.a = bkg.a;
    }
    else if (Mode == 4) // SCREEN
    {
        res.rgb = 1.0 - (1.0 - src.rgb) * (1.0 - bkg.rgb);
        res.a = max(src.a, bkg.a);
    }
    else // Default to SRC_OVER if mode not handled yet
    {
        res = src + bkg * (1.0 - src.a);
    }
    
    // Final Clamp to keep colors in valid range [0, 1]
    res.rgb = saturate(res.rgb);
    
    return res;
}
