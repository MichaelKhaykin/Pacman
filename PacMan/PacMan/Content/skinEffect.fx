sampler me : register(s0);

texture Skin;
sampler skinSampler = sampler_state { Texture = <Skin>; };

//This function returns a float4 which also happens to be a Color0 which 
//represents R,G,B,A
//It takes in a float4 position which also happens to be a SV_Position, which 
//represents a vertex position (need clarification)
//It takes a float4 tintColor which also happens to be a Color0, which represents,
//the R,G,B,A values
//It also takes a float2 pixel which also happens to be a TEXCOORD0 which represents,
//the first coordinate of a UV mapping, (UV mapping is used for projecting a 2d image into 3d)
float4 PixelShaderTextureSkin(float4 position : SV_POSITION, float4 tintColor : COLOR0, float2 pixel : TEXCOORD0) : COLOR0
{
	// Test... float4 = vector<float, 4> (typeCount = vector<type, componentCount>)
	float4 originalColor = tex2D(me, pixel);
	float4 skinColor = tex2D(skinSampler, pixel);

	if (originalColor.a)
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