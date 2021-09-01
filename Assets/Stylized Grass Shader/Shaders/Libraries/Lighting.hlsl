//Stylized Grass Shader
//Staggart Creations (http://staggart.xyz)
//Copyright protected under Unity Asset Store EULA

void ApplyTranslucency(inout float3 color, float3 viewDirectionWS, Light light, float amount)
{
	float VdotL = saturate(dot(-viewDirectionWS, light.direction));
	VdotL = pow(VdotL, 4);

	//Translucency masked by shadows and grass mesh bottom
	float tMask = VdotL * light.shadowAttenuation * light.distanceAttenuation;

	//Fade the effect out as the sun approaches the horizon (75 to 90 degrees)
	half sunAngle = dot(float3(0, 1, 0), light.direction);
	half angleMask = saturate(sunAngle * 6.666); /* 1.0/0.15 = 6.666 */

	tMask *= angleMask;

	float3 tColor = color + BlendOverlay((light.color), color);
	color = lerp(color, tColor, tMask * (amount * 4.0));
}

float3 UnlitShading(SurfaceData surfaceData, InputData input)
{
	#if VERSION_GREATER_EQUAL(10,0)
	#if defined(_SCREEN_SPACE_OCCLUSION)
	AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(input.normalizedScreenSpaceUV);
	surfaceData.occlusion = min(surfaceData.occlusion, aoFactor.indirectAmbientOcclusion);

	surfaceData.albedo *= min(surfaceData.occlusion, aoFactor.indirectAmbientOcclusion);
	#endif
	#endif

	return surfaceData.albedo;
}

// General function to apply lighting based on the configured mode
half3 ApplyLighting(SurfaceData surfaceData, InputData inputData, Light mainLight, half translucency)
{
	half3 color = 0;
	
#ifdef _UNLIT
	color = UnlitShading(surfaceData, inputData);
#endif

#if _SIMPLE_LIGHTING
	#if VERSION_LOWER(12,0)
	color = UniversalFragmentBlinnPhong(inputData, surfaceData.albedo, 0, surfaceData.smoothness, surfaceData.emission, surfaceData.alpha).rgb;
	#else
	color = UniversalFragmentBlinnPhong(inputData, surfaceData).rgb;
	#endif
#endif

#if _ADVANCED_LIGHTING
	#if VERSION_LOWER(10,0)
	color = UniversalFragmentPBR(inputData, surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.occlusion, surfaceData.emission, surfaceData.alpha).rgb;
	#else
	color = UniversalFragmentPBR(inputData, surfaceData).rgb;
	#endif
#endif

	ApplyTranslucency(color, inputData.viewDirectionWS, mainLight, translucency);
	
	return color;
}