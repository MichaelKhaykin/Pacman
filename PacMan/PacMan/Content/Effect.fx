sampler me : register(s0);

texture Skin;
sampler skinSampler = sampler_state { Texture = <Skin>; };

float4 PixelShaderTextureSkin(float4 position : SV_POSITION, float4 tintColor : COLOR0, float2 pixel : TEXCOORD0) : COLOR0
{
	// Test... float4 = vector<float, 4> (typeCount = vector<type, componentCount>)
	float4 originalColor = tex2D(me, pixel); 
	float4 skinColor = tex2D(skinSampler, pixel);
	
	if(originalColor.a)
	{
		return skinColor * tintColor;
	}

	return originalColor;
}

vector<float, 4> PixelShaderGrayScale(float4 position : SV_POSITION, float4 tintColor : COLOR0, float2 pixel : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(me, pixel);
	color.rgb = (color.r + color.g + color.b) / 3.0f;

	return color;
}

technique TextureSkin
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 PixelShaderTextureSkin();
	}
}

technique GrayScale
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 PixelShaderGrayScale();
	}
}

//This doesn't work :(
technique TextureSkinAndGrayScale
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 PixelShaderTextureSkin();
	}

	pass Pass2
	{
		//Uses original color; no good; needs to use last pass's output
		PixelShader = compile ps_4_0 PixelShaderGrayScale();
	}
}