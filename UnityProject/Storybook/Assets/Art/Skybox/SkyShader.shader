// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.25;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:1,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-5378-OUT;n:type:ShaderForge.SFN_Tex2d,id:4985,x:31583,y:32866,ptovrint:False,ptlb:Stars,ptin:_Stars,varname:node_4985,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c5cf15dc16816a04aa9717de388a69f1,ntxv:0,isnm:False|UVIN-7903-OUT;n:type:ShaderForge.SFN_Multiply,id:6917,x:32139,y:32930,varname:node_6917,prsc:2|A-4985-RGB,B-5360-OUT,C-6413-OUT;n:type:ShaderForge.SFN_Vector1,id:5360,x:31775,y:32937,varname:node_5360,prsc:2,v1:8;n:type:ShaderForge.SFN_Tex2d,id:4542,x:32002,y:33124,ptovrint:False,ptlb:Smoke,ptin:_Smoke,varname:node_4542,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:44fdffd2d2b5504499ed618a1db56261,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:8461,x:30579,y:32809,varname:node_8461,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:7903,x:31385,y:32766,varname:node_7903,prsc:2|A-8461-UVOUT,B-5597-OUT;n:type:ShaderForge.SFN_Vector1,id:5597,x:31190,y:32864,varname:node_5597,prsc:2,v1:20;n:type:ShaderForge.SFN_Tex2d,id:5813,x:31071,y:33052,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_5813,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:91259e6c93a3edb47be8e3f60fb839d0,ntxv:0,isnm:False|UVIN-791-UVOUT;n:type:ShaderForge.SFN_Power,id:6413,x:31785,y:33039,varname:node_6413,prsc:2|VAL-1916-OUT,EXP-1801-OUT;n:type:ShaderForge.SFN_Vector1,id:1801,x:31577,y:33210,varname:node_1801,prsc:2,v1:3;n:type:ShaderForge.SFN_Panner,id:791,x:30846,y:33052,varname:node_791,prsc:2,spu:0,spv:0.05|UVIN-8461-UVOUT;n:type:ShaderForge.SFN_Add,id:5378,x:32369,y:32879,varname:node_5378,prsc:2|A-5237-OUT,B-6917-OUT,C-323-OUT;n:type:ShaderForge.SFN_Vector3,id:5237,x:32139,y:32833,varname:node_5237,prsc:2,v1:0.1098039,v2:0.01176471,v3:0.2941177;n:type:ShaderForge.SFN_Divide,id:323,x:32198,y:33154,varname:node_323,prsc:2|A-4542-RGB,B-9364-OUT;n:type:ShaderForge.SFN_Vector1,id:9364,x:32016,y:33389,varname:node_9364,prsc:2,v1:10;n:type:ShaderForge.SFN_Tex2d,id:6116,x:31071,y:33286,ptovrint:False,ptlb:Noise2,ptin:_Noise2,varname:_Noise_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:91259e6c93a3edb47be8e3f60fb839d0,ntxv:0,isnm:False|UVIN-8431-UVOUT;n:type:ShaderForge.SFN_Panner,id:8431,x:30846,y:33293,varname:node_8431,prsc:2,spu:-0.05,spv:0|UVIN-8461-UVOUT;n:type:ShaderForge.SFN_Multiply,id:1916,x:31420,y:33176,varname:node_1916,prsc:2|A-5813-RGB,B-6116-RGB,C-4121-OUT;n:type:ShaderForge.SFN_Vector1,id:4121,x:31071,y:33534,varname:node_4121,prsc:2,v1:2;proporder:4985-4542-5813-6116;pass:END;sub:END;*/

Shader "Shader Forge/SkyShader" {
    Properties {
        _Stars ("Stars", 2D) = "white" {}
        _Smoke ("Smoke", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _Noise2 ("Noise2", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Front
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Stars; uniform float4 _Stars_ST;
            uniform sampler2D _Smoke; uniform float4 _Smoke_ST;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform sampler2D _Noise2; uniform float4 _Noise2_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_7903 = (i.uv0*20.0);
                float4 _Stars_var = tex2D(_Stars,TRANSFORM_TEX(node_7903, _Stars));
                float4 node_4227 = _Time + _TimeEditor;
                float2 node_791 = (i.uv0+node_4227.g*float2(0,0.05));
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_791, _Noise));
                float2 node_8431 = (i.uv0+node_4227.g*float2(-0.05,0));
                float4 _Noise2_var = tex2D(_Noise2,TRANSFORM_TEX(node_8431, _Noise2));
                float4 _Smoke_var = tex2D(_Smoke,TRANSFORM_TEX(i.uv0, _Smoke));
                float3 emissive = (float3(0.1098039,0.01176471,0.2941177)+(_Stars_var.rgb*8.0*pow((_Noise_var.rgb*_Noise2_var.rgb*2.0),3.0))+(_Smoke_var.rgb/10.0));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
