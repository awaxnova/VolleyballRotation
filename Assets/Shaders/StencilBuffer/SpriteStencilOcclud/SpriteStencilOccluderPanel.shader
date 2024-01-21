// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "StencilRenderTexture/SpriteStencilOccluderPanel"
{
// PUT THIS ON THE OBJECT THAT"S IN FRONT OF THE OTHER OBJECTS< OCCLUDING THEM, but 
// ALLOWING objects that use the SpriteStencilOccluderMat to be seen through it.

// The shader code you provided is a more advanced occluder shader than the previous one. 

// This shader uses two passes to render the occluder:

// The first pass renders the occluder as normal, 
//  but it also sets the stencil buffer to value 4 
//  for all pixels that are rendered.
// The second pass renders any objects that are behind the occluder. 
//  This pass uses the stencil buffer to determine which pixels should 
//  be rendered. Only pixels that have the stencil buffer set to 
//  value 4 will be rendered in this pass.
// The shader code also defines a new property called _OccludedColor. 
//  This property specifies the color that will be used to render 
//  objects that are behind the occluder. The default value of this 
//  property is (0, 0, 0, 0.5), which means that objects that are 
//  behind the occluder will be rendered as a semi-transparent black.
// 
// Here is a more detailed explanation of some of the key parts of the shader code:
// 
// The Stencil block defines the stencil buffer settings for the shader. 
//  The Ref value specifies the value that will be written to the stencil 
//  buffer for each pixel that is rendered by the occluder. The Comp value 
//  specifies the condition that must be met for the stencil buffer to be 
//  written. In this case, the Comp value is NotEqual, which means that the 
//  stencil buffer will only be written if the value in the stencil buffer is not equal 
//  to value specified in the Ref value. The Pass value specifies what should happen 
//  when the stencil buffer is written. In this case, the Pass value is None, 
//  which means that the stencil buffer will not be written in this pass.
// The fixed4 _OccludedColor property defines the color that will be used to 
//  render objects that are behind the occluder.
// The frag function in the second pass is responsible for rendering objects 
//  that are behind the occluder. This function simply loads the texture, 
//  tints the color, and then multiplies the color by the value of the _OccludedColor property.

  Properties
  {
     [PerRendererData] _MainTex ( "Sprite Texture", 2D ) = "white" {}
      _Color ( "Tint", Color ) = ( 1, 1, 1, 1 )
     [MaterialToggle] PixelSnap ( "Pixel snap", Float ) = 0
      _OccludedColor ( "Occluded Tint", Color ) = ( 0, 0, 0, 0.5 )
  }


CGINCLUDE

// shared structs and vert program used in both the vert and frag programs
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
sampler2D _MainTex;


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

ENDCG



  SubShader
  {
      Tags
      {
          "Queue" = "Transparent"
          "IgnoreProjector" = "True"
          "RenderType" = "Transparent"
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
              Comp NotEqual
          }


      CGPROGRAM
          #pragma vertex vert
          #pragma fragment frag
          #pragma multi_compile _ PIXELSNAP_ON
          #include "UnityCG.cginc"


          fixed4 frag( v2f IN ) : SV_Target
          {
              fixed4 c = tex2D( _MainTex, IN.texcoord ) * IN.color;
              c.rgb *= c.a;
              return c;
          }
      ENDCG
      }


      // occluded pixel pass. Anything rendered here is behind an occluder
      Pass
      {
          Stencil
          {
              Ref 4
              Comp Equal
          }

      CGPROGRAM
          #pragma vertex vert
          #pragma fragment frag
          #pragma multi_compile _ PIXELSNAP_ON
          #include "UnityCG.cginc"

          fixed4 _OccludedColor;


          fixed4 frag( v2f IN ) : SV_Target
          {
              fixed4 c = tex2D( _MainTex, IN.texcoord );
              return _OccludedColor * c.a;
          }
      ENDCG
      }
  }
}
