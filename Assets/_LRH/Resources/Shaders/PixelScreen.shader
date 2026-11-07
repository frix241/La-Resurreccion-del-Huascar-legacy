Shader "LRH/PixelScreen"
{
 Properties { _MainTex ("Frame",2D)="white"{} }
 SubShader
 {
  Cull Off ZWrite Off ZTest Always
  Pass
  {
   CGPROGRAM
   #pragma vertex vert_img
   #pragma fragment frag
   #pragma target 3.0
   #include "UnityCG.cginc"
   sampler2D _MainTex;
   fixed4 frag(v2f_img i):SV_Target
   {
    return float4(tex2D(_MainTex,i.uv).rgb,1);
   }
   ENDCG
  }
 }
}
