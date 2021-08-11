Shader "Custom/circleDam"
{
    Properties
    {
       [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}    //   当前的Sprite图（添加[PerRendererData]后在属性面板中不可见） 
       _Color("_Color", Color) = (0,0,0,1)
       load("load",Float) = 1.0
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
                 float load;

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
                     float nx = IN.uv_MainTex.x - 0.5;
                     float ny = IN.uv_MainTex.y - 0.5;
                     float nmod = sqrt(nx*nx+ny*ny);

                     ny /= nmod; //单位向量y轴
                     ny = -ny * 0.25 + 0.25; //左右两半圆 Range [0,0.5]
                     if (nx < 0) ny = 1 - ny;//右半圆 Range [0,0.5] ===> [0.5,1]

                     if (ny > load) discard;

                      


                     //正常的纹理采样，叠加_Color颜色    
                     half4 c = tex2D(_MainTex, IN.uv_MainTex);
                     c *= _Color;

                     return c;
                 }

                 ENDCG
             }
       }
}
