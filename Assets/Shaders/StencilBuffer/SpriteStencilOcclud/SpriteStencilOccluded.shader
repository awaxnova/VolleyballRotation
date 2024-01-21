// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "StencilRenderTexture/SpriteStencilOccluded"
{
    // PUT THIS ON THE OBJECT THAT"S BEHIND THE OCCLUDING OBJECT... this goes on the objects that 
    // could be hidden behind.

    //The shader code you provided is a simple occluder shader that can be used to create effects 
    // such as shadows and occlusion. The shader uses the stencil buffer to track which pixels 
    // have been rendered by the occluder, and then uses this information to prevent other objects 
    // from being rendered in those areas.
    //
    //The shader code works as follows:
    //
    //The Properties block defines the shader's properties, 
    // such as the texture that will be used for the occluder, 
    // the tint color, and the alpha cutoff value.
    //
    //The SubShader block defines the shader's subshader, 
    // which contains the code that is actually executed when the shader is rendered.
    //
    //The Pass block defines a single pass of the shader. 
    // In this case, the pass simply sets the stencil buffer to value 4 for all 
    // pixels that are rendered by the occluder.
    //
    //The CGPROGRAM block contains the C# code that is executed by the shader. 
    // This code loads the texture, tints the color, and then clips any pixels 
    // that are below the alpha cutoff value.
    //
    //The final effect of the shader is that any objects that are rendered 
    // after the occluder will be prevented from rendering in the areas where 
    // the occluder has been rendered. This can be used to create shadows, 
    // occlusion, and other effects.
    //
    //Here is a more detailed explanation of some of the key parts of the shader code:
    //
    //The Stencil block defines the stencil buffer settings for the shader. 
    // The Ref value specifies the value that will be written to the stencil 
    // buffer for each pixel that is rendered by the occluder. 
    // The Comp value specifies the condition that must be met for the 
    // stencil buffer to be written. In this case, the Comp value is Always, 
    // which means that the stencil buffer will be written for every pixel that 
    // is rendered by the occluder. The Pass value specifies what should happen 
    // when the stencil buffer is written. In this case, the Pass value is Replace, 
    // which means that the stencil buffer will be replaced with the value specified 
    // in the Ref value.
    //
    //The v2f struct defines the vertex data that will be passed to the fragment shader. 
    // The vertex member of the struct contains the vertex position in clip space, the 
    // texcoord member contains the texture coordinates for the vertex, and the color 
    // member contains the vertex color.
    //
    //The frag function is the fragment shader function. This function is responsible 
    // for calculating the final pixel color. In this case, the frag function simply 
    // loads the texture, tints the color, and then clips any pixels that are below 
    // the alpha cutoff value.


  Properties
  {
     [PerRendererData] _MainTex ( "Sprite Texture", 2D ) = "white" {}
      _Color ( "Tint", Color ) = ( 1, 1, 1, 1 )
     [MaterialToggle] PixelSnap ( "Pixel snap", Float ) = 0
      _AlphaCutoff ( "Alpha Cutoff", Range( 0.01, 1.0 ) ) = 0.1
  }


  SubShader
  {
      Tags
      {
          "Queue" = "Transparent"
          "IgnoreProjector" = "True"
          "RenderType" = "TransparentCutout"
          "PreviewType" = "Plane"
          "CanUseSpriteAtlas" = "True"
      }

      Cull Off
      Lighting Off
      ZWrite Off
      Blend One OneMinusSrcAlpha

      Pass
      {
          Stencil
          {
              Ref 4
              Comp Always
              Pass Replace
          }

      CGPROGRAM
          #pragma vertex vert
          #pragma fragment frag
          #pragma multi_compile _ PIXELSNAP_ON
          #include "UnityCG.cginc"

          struct appdata_t
          {
              float4 vertex   : POSITION;
              float4 color    : COLOR;
              float2 texcoord : TEXCOORD0;
          };

          struct v2f
          {
              float4 vertex   : SV_POSITION;
              fixed4 color    : COLOR;
              half2 texcoord  : TEXCOORD0;
          };

          fixed4 _Color;
          fixed _AlphaCutoff;

          v2f vert( appdata_t IN )
          {
              v2f OUT;
              OUT.vertex = UnityObjectToClipPos( IN.vertex );
              OUT.texcoord = IN.texcoord;
              OUT.color = IN.color * _Color;
              #ifdef PIXELSNAP_ON
              OUT.vertex = UnityPixelSnap( OUT.vertex );
              #endif

              return OUT;
          }

          sampler2D _MainTex;
          sampler2D _AlphaTex;


          fixed4 frag( v2f IN ) : SV_Target
          {
              fixed4 c = tex2D( _MainTex, IN.texcoord ) * IN.color;
              c.rgb *= c.a;

              // here we discard pixels below our _AlphaCutoff so the stencil buffer only gets written to
              // where there are actual pixels returned. If the occluders are all tight meshes (such as solid rectangles)
              // this is not necessary and a non-transparent shader would be a better fit.
              clip( c.a - _AlphaCutoff );

              return c;
          }
      ENDCG
      }
  }
}
