Shader "Unlit/Waves"
{
	/* 
	This vertex shader simulates wave motions from open sea, apply any water texture.
	Created by Frans Huntink
	
	Relevant sources:
	https://docs.unity3d.com/Manual/SL-GrabPass.html - Basic grab pass reference
	https://www.youtube.com/watch?v=zKWQn2Ppk74 - Basic water shader setup
	*/

	Properties
	{

		_MainTex ("Texture", 2D) = "white" {} //main texture	
		

		/* Declaration and definition of wave variables */
		
		_WaveSpeed("_WaveSpeed", float) = 0.3 //the speed of waves along the z-axis
		_WaveSpreadSpeed("_WaveSpreadSpeed", float) = 0.3 //the speed of the waves along the x-axis
		_WaveHeight("_WaveHeight", float) = 20 //the height of the waves along the y-axis
		_WaveSpread("_WaveSpread", float) = 6 //the width of the waves along the x-axis
		_Blend("_Blend", Range(0,1) ) = 0.5
		

	}

		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

			//end new
			LOD 100

			GrabPass 
			{
            "_BackgroundTexture"
			}
			
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				/* declare the custom water variables */
				
				float _WaveSpeed;
				float _WaveHeight;
				float _WaveSpread;
				float _WaveSpreadSpeed;
				float _Blend;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
		
				UNITY_FOG_COORDS(1)
		
				/* determine variables for each vertex */
				
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 grabPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v) //handles vertice transformations
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.vertex);
				
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				
				/* determine the position of the vertices */
				
				float heightVertex; //the new vertice position (height)
				float heightTranslation; //the vertice translation (height)
				float widthVertex;  //the new vertice position (width)
				float widthTranslation; //the vertice translation (width)

				/* calculate the translations */
				heightTranslation = sin(worldPos.z + _Time.w * _WaveSpeed);
				widthTranslation = sin(worldPos.x + _Time.w) * _WaveSpread;

				/* adjust the vertex on y-axis according to the sinus wave */
				widthVertex = o.vertex.x + widthTranslation * _WaveSpreadSpeed;
				heightVertex = o.vertex.y + heightTranslation * _WaveHeight;
			
				//apply our calculations to the vertices
				o.vertex.y = heightVertex;
				o.vertex.x = widthVertex;
				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				return o;
			}
			
			sampler2D _BackgroundTexture;

			fixed4 frag(v2f i) : SV_Target
			{
				
				half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos); 
				fixed4 col = tex2D(_MainTex, i.uv);
				float4 Lerp0 = lerp(bgcolor,col,_Blend);
				
				return Lerp0;
		    }


		ENDCG
	}


	}
}
