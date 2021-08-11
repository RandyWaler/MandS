// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/circleRange"
{
    Properties
    {
       [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}    //   当前的Sprite图（添加[PerRendererData]后在属性面板中不可见） 
       _Color("_Color", Color) = (0,0,0,1)
       uvRange("uvRange",Float) = 0.25
    }
        SubShader
       {
           //Sprite图一般均为透明贴图，需要做以下处理
             Tags
             {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
             }

             Pass
             {
               //Sprite图一般均为透明贴图，需要做以下处理
                Cull Off
                Lighting Off
                ZWrite Off
                Fog { Mode Off }
                Blend SrcAlpha OneMinusSrcAlpha    //Sprite图一般均为透明贴图，需要做以下处理

                 CGPROGRAM
                 #pragma vertex vert
                 #pragma fragment frag

                 sampler2D _MainTex;
                 float4 _Color;
                 float uvRange;

                 struct Vertex
                 {
                     float4 vertex : POSITION;
                     float2 uv_MainTex : TEXCOORD0;
                 };

                 struct Fragment
                 {
                     float4 vertex : POSITION;
                     float2 uv_MainTex : TEXCOORD0;
                 };

                 Fragment vert(Vertex v)
                 {
                     Fragment o;

                     o.vertex = UnityObjectToClipPos(v.vertex);
                     o.uv_MainTex = v.uv_MainTex;

                     return o;
                 }

                 float4 frag(Fragment IN) : COLOR
                 {
                     //discard计算
                     float uvx = abs(IN.uv_MainTex.x - 0.5f);
                     float uvy = abs(IN.uv_MainTex.y - 0.5f);
                     if (uvx*uvx + uvy*uvy < uvRange) discard;


                     //正常的纹理采样，叠加_Color颜色    
                     half4 c = tex2D(_MainTex, IN.uv_MainTex);
                     c *= _Color;

                     return c;
                 }

                 ENDCG
             }
       }
}

