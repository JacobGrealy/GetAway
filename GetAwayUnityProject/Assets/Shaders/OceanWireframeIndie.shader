Shader "Ocean/Indie/OceanWireframeShader" 
{
	Properties 
	{
		_WireColor("WireColor", Color) = (1,1,1,0.1)
	}
	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma target 3.0
		#pragma glsl

		uniform sampler2D _Map0;
		uniform float4 _GridSizes;
		uniform float _MaxLod, _LodFadeDist;

		float4 _WireColor;

		struct Input 
		{
			float2 uv_MainTex;
		};
		
		void vert(inout appdata_full v) 
		{
		
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

		void surf (Input IN, inout SurfaceOutput o) 
		{
			o.Albedo = _WireColor.rgb;
			o.Alpha = _WireColor.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
