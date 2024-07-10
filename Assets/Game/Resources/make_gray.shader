Shader "Unlit/GreyScale" {
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
 
    SubShader
    {    	
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }
 
		Cull Off
		Lighting Off
		ZWrite Off
		Offset -1, -1
		Fog { Mode Off }
		ColorMask RGB
		AlphaTest Greater .01
		Blend SrcAlpha OneMinusSrcAlpha
 
             Pass {
 
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 
 #include "UnityCG.cginc"
 
 sampler2D _MainTex;
 
 struct v2f {
     float4  pos : SV_POSITION;
     float2  uv : TEXCOORD0;
 };
 
 float4 _MainTex_ST;
 
 v2f vert (appdata_base v)
 {
     v2f o;
     o.pos = UnityObjectToClipPos (v.vertex);
     o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
     return o;
 }
 
 half4 frag (v2f i) : COLOR
 {
     half4 texcol = tex2D (_MainTex, i.uv);
     texcol.rgb = dot(texcol.rgb, float3(0.3, 0.59, 0.11));
	 texcol.b =texcol.b*1.3;
     return texcol;
 }
 ENDCG
 
     }
 }
     
    Fallback "Sprites/Default"
 } 