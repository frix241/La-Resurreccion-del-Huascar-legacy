Shader "LRH/TerrainBlend"
{
 Properties
 {
  _Splat ("Terrain weights", 2D) = "red" {}
  _Water ("Deep water", 2D) = "white" {}
  _Shallow ("Shallow water", 2D) = "white" {}
  _Sand ("Sand", 2D) = "white" {}
  _Stone ("Stone", 2D) = "white" {}
  _Size ("World size", Vector) = (96,64,0,0)
 }
 SubShader
 {
  Tags { "Queue"="Transparent" "RenderType"="Transparent" }
  Cull Off ZWrite Off
  Pass
  {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   sampler2D _Splat, _Water, _Shallow, _Sand, _Stone;
   float4 _Size;
   struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };
   v2f vert(appdata_base v) { v2f o; o.pos=UnityObjectToClipPos(v.vertex); o.uv=v.texcoord; return o; }
   fixed4 frag(v2f i):SV_Target
   {
    float4 w=tex2D(_Splat,i.uv);
    w=max(w,0); w/=max(dot(w,float4(1,1,1,1)),.001);
    float2 uv=i.uv*_Size.xy;
    float3 water=tex2D(_Water,uv).rgb;

    float3 color=water*w.r+tex2D(_Shallow,uv).rgb*w.g
     +tex2D(_Sand,uv).rgb*w.b+tex2D(_Stone,uv).rgb*w.a;
    float surf=saturate(w.g*w.b*2.5);
    color=lerp(color,float3(.72,.94,.82),surf*.5);
    return float4(color,1);
   }
   ENDCG
  }
 }
}
