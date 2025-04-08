Shader "Unlit/Stencil Mask"
{
	Properties
	{
		[IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
		_Color ("Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Zwrite Off

			Stencil
			{
				Ref [_StencilID]
				Comp Always
				Pass Replace
				Fail Keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"


			float4 _Color;

			// automatically filled out by Unity
			struct MeshData
			{
				// per-vertex mesh data
				float4 vertex : POSITION; // local space vertex position
				float3 normals : NORMAL; // local space normal direction
				// float4 tangent : TANGENT; // tangent direction (xyz) tangent sign (w)
				// float4 color : COLOR; // vertex colors
				float4 uv : TEXCOORD0;
			};

			// data passed from the vertex shader to the fragment shader
			// this will interpolate/blend across the triangle!
			struct Interpolators
			{
				float4 vertex : SV_POSITION; // clip space position
				float3 normal : TEXCOORD0;
				float2 uv : TEXCOORD1;
			};

			Interpolators vert(MeshData v)
			{
				Interpolators o;
				o.vertex = UnityObjectToClipPos(v.vertex); // local space to clip space
				o.normal = UnityObjectToWorldNormal(v.normals);
				o.uv = v.uv;
				return o;
			}

			float4 frag(Interpolators i) : SV_Target
			{
				// blend between two colors based on the X UV coordinate
				// float t = saturate( InverseLerp( _ColorStart, _ColorEnd, i.uv.x ) );
				// float t = abs(frac(i.uv.x * 5) * 2 - 1)
				// return float4( i.normal, 0 );

				return _Color;
			}
			ENDCG
		}
	}
}
