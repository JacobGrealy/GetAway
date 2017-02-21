Shader "Ocean/Indie/OceanShader" 
{
	Properties 
	{
		_SunPow("SunPow", float) = 256
		_SeaColor("SeaColor", Color) = (1,1,1,1)
		_SkyBox("SkyBox", CUBE) = "" {}
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma target 3.0
		#pragma glsl

		uniform sampler2D _FresnelLookUp, _Map0, _Map1, _Map2;
		uniform float4 _GridSizes;
		uniform float3 _SunColor, _SunDir;
		uniform float _MaxLod, _LodFadeDist;
		
		float _SunPow;
		float3 _SeaColor;
		samplerCUBE _SkyBox;

		struct Input 
		{
			float3 worldPos;
			//float3 viewDir;
			float3 worldRefl;
			INTERNAL_DATA
		};
		
		void vert(inout appdata_full v) 
		{
			v.tangent = float4(1,0,0,1);
			
			float3 worldPos = mul(_Object2World, v.vertex).xyz;
			
			float dist = clamp(distance(_WorldSpaceCameraPos.xyz, worldPos) / _LodFadeDist, 0.0, 1.0);
			float lod = _MaxLod * dist;
			
			float ht = 0.0;
			ht += tex2Dlod(_Map0, float4(worldPos.xz/_GridSizes.x, 0, lod)).x*2.0-1.0;
			ht += tex2Dlod(_Map0, float4(worldPos.xz/_GridSizes.y, 0, lod)).y*2.0-1.0;
			//ht += tex2Dlod(_Map0, float4(worldPos.xz/_GridSizes.z, 0, lod)).z*2.0-1.0;
			//ht += tex2Dlod(_Map0, float4(worldPos.xz/_GridSizes.w, 0, lod)).w*2.0-1.0;

			v.vertex.y += ht;
		}
		
		float Fresnel(float3 V, float3 N)
		{
			float costhetai = abs(dot(V, N));
			return tex2D(_FresnelLookUp, float2(costhetai, 0.0)).a * 0.7; //looks better scaled down a little?
		}
		
		float3 Sun(float3 V, float3 N)
		{
			float3 H = normalize(V+_SunDir);
			return _SunColor * pow(abs(dot(H,N)), _SunPow);
		}

		void surf(Input IN, inout SurfaceOutput o) 
		{
			float2 uv = IN.worldPos.xz;
			
			float2 slope = float2(0,0);
			slope += tex2D(_Map1, uv/_GridSizes.x).xy*2.0-1.0;
			slope += tex2D(_Map1, uv/_GridSizes.y).zw*2.0-1.0;
			slope += tex2D(_Map2, uv/_GridSizes.z).xy*2.0-1.0;
			slope += tex2D(_Map2, uv/_GridSizes.w).zw*2.0-1.0;
			
			float3 N = normalize(float3(-slope.x, 2.0, -slope.y)); //shallow normal
			float3 N2 = normalize(float3(-slope.x, 0.5, -slope.y)); //sharp normal
			
			float3 V = normalize(_WorldSpaceCameraPos-IN.worldPos);

			float fresnel = Fresnel(V, N);
			
			o.Normal = N.xzy;//this is tangent space?
			float3 skyColor = texCUBE(_SkyBox, WorldReflectionVector(IN, o.Normal)*float3(-1,1,1)).rgb;//flip x?

			o.Albedo = lerp(_SeaColor, skyColor, fresnel) + Sun(V,N2);
			o.Alpha = 1.0;
			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
