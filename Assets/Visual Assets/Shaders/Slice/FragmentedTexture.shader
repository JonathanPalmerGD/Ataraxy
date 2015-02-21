Shader "Slice/Fragmented Texture" {
  Properties {
	  _PerComplete ("Completeness", Range(0,1)) = .5
      //_Color ("Color", Color) = (0.26,0.19,0.16,0.0)
      _MainTex ("Texture", 2D) = "white" {}
      _BumpMap ("Bumpmap", 2D) = "bump" {}
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      Cull Off
      CGPROGRAM
      #pragma surface surf Lambert vertex:vert
      struct Input {
          float2 uv_MainTex;
          float2 uv_BumpMap;
          float3 worldPos;
		  float3 objPos;
      };
      sampler2D _MainTex;
      sampler2D _BumpMap;
	  float _PerComplete;
	  void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.objPos = v.vertex;
	  }

      void surf (Input IN, inout SurfaceOutput o) {
	  //If the texture alpha is less than a set value, clip it.
		 clip (_PerComplete - tex2D (_BumpMap, IN.uv_BumpMap).b);
	     o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
         //o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }