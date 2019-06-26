// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:8058,x:33127,y:32723,varname:node_8058,prsc:2|emission-5264-OUT,alpha-6954-OUT;n:type:ShaderForge.SFN_Tex2d,id:224,x:32278,y:32604,ptovrint:False,ptlb:node_224,ptin:_node_224,varname:node_224,prsc:2,tex:701f6a884a30ebc45b99b09ad889e095,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5068,x:32287,y:32882,ptovrint:False,ptlb:node_5068,ptin:_node_5068,varname:node_5068,prsc:2,tex:fd81083c46cf448438acb5b5f72326b4,ntxv:0,isnm:False|UVIN-8037-UVOUT;n:type:ShaderForge.SFN_Add,id:5264,x:32730,y:32823,varname:node_5264,prsc:2|A-9308-OUT,B-5068-RGB;n:type:ShaderForge.SFN_Multiply,id:6954,x:32827,y:33070,varname:node_6954,prsc:2|A-1116-R,B-5068-A;n:type:ShaderForge.SFN_Multiply,id:9308,x:32583,y:32661,varname:node_9308,prsc:2|A-6490-OUT,B-224-A;n:type:ShaderForge.SFN_Vector1,id:6490,x:32388,y:32522,varname:node_6490,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Tex2d,id:2816,x:31489,y:32882,ptovrint:False,ptlb:0,ptin:_0,varname:node_2816,prsc:2,tex:3f7a3153736f09b4287f34949975e617,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Panner,id:8037,x:32123,y:32856,varname:node_8037,prsc:2,spu:-1,spv:0|UVIN-9053-OUT;n:type:ShaderForge.SFN_Multiply,id:6327,x:31784,y:32936,varname:node_6327,prsc:2|A-6048-OUT,B-2816-R;n:type:ShaderForge.SFN_Slider,id:6048,x:31463,y:32633,ptovrint:False,ptlb:node_6048,ptin:_node_6048,varname:node_6048,prsc:2,min:0,cur:0.08687337,max:1;n:type:ShaderForge.SFN_Add,id:9053,x:31966,y:32866,varname:node_9053,prsc:2|A-9386-UVOUT,B-6327-OUT;n:type:ShaderForge.SFN_TexCoord,id:9386,x:31798,y:32751,varname:node_9386,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:1116,x:32517,y:33109,ptovrint:False,ptlb:node_1116,ptin:_node_1116,varname:node_1116,prsc:2,tex:e98a5d6f2af26e5448f9db5c17622382,ntxv:0,isnm:False;proporder:224-5068-2816-6048-1116;pass:END;sub:END;*/

Shader "Shader Forge/z1" {
    Properties {
        _node_224 ("node_224", 2D) = "white" {}
        _node_5068 ("node_5068", 2D) = "white" {}
        _0 ("0", 2D) = "white" {}
        _node_6048 ("node_6048", Range(0, 1)) = 0.08687337
        _node_1116 ("node_1116", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform sampler2D _node_224; uniform float4 _node_224_ST;
            uniform sampler2D _node_5068; uniform float4 _node_5068_ST;
            uniform sampler2D _0; uniform float4 _0_ST;
            uniform float _node_6048;
            uniform sampler2D _node_1116; uniform float4 _node_1116_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                #ifndef LIGHTMAP_OFF
                    float4 uvLM : TEXCOORD2;
                #else
                    float3 shLight : TEXCOORD2;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float4 _node_224_var = tex2D(_node_224,TRANSFORM_TEX(i.uv0, _node_224));
                float4 node_523 = _Time + _TimeEditor;
                float4 _0_var = tex2D(_0,TRANSFORM_TEX(i.uv0, _0));
                float2 node_8037 = ((i.uv0+(_node_6048*_0_var.r))+node_523.g*float2(-1,0));
                float4 _node_5068_var = tex2D(_node_5068,TRANSFORM_TEX(node_8037, _node_5068));
                float3 emissive = ((0.3*_node_224_var.a)+_node_5068_var.rgb);
                float3 finalColor = emissive;
                float4 _node_1116_var = tex2D(_node_1116,TRANSFORM_TEX(i.uv0, _node_1116));
                fixed4 finalRGBA = fixed4(finalColor,(_node_1116_var.r*_node_5068_var.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
