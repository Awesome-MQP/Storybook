// Shader created with Shader Forge v1.21 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.21;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:4013,x:32719,y:32712,varname:node_4013,prsc:2|diff-605-OUT,custl-1053-OUT;n:type:ShaderForge.SFN_Tex2d,id:3139,x:31956,y:32434,ptovrint:False,ptlb:BaseTex,ptin:_BaseTex,varname:node_3139,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7143,x:31964,y:32704,ptovrint:False,ptlb:Brush,ptin:_Brush,varname:node_7143,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:1a00525cc2798914692165d0bf02e68c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:605,x:32286,y:32658,varname:node_605,prsc:2|A-3139-RGB,B-7143-RGB;n:type:ShaderForge.SFN_LightVector,id:2927,x:30959,y:32858,varname:node_2927,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:2473,x:30969,y:33033,prsc:2,pt:True;n:type:ShaderForge.SFN_Dot,id:2544,x:31160,y:32950,varname:node_2544,prsc:2,dt:1|A-2927-OUT,B-2473-OUT;n:type:ShaderForge.SFN_Ceil,id:7087,x:31662,y:32952,varname:node_7087,prsc:2|IN-5742-OUT;n:type:ShaderForge.SFN_Multiply,id:5742,x:31449,y:32939,varname:node_5742,prsc:2|A-2544-OUT,B-5184-OUT;n:type:ShaderForge.SFN_Vector1,id:5184,x:31192,y:33133,varname:node_5184,prsc:2,v1:4;n:type:ShaderForge.SFN_Divide,id:4432,x:31899,y:32952,varname:node_4432,prsc:2|A-7087-OUT,B-5184-OUT;n:type:ShaderForge.SFN_Add,id:3597,x:32150,y:32952,varname:node_3597,prsc:2|A-4432-OUT,B-2544-OUT,C-4432-OUT;n:type:ShaderForge.SFN_Multiply,id:1053,x:32537,y:32960,varname:node_1053,prsc:2|A-6945-OUT,B-4840-RGB;n:type:ShaderForge.SFN_LightColor,id:4840,x:31657,y:33380,varname:node_4840,prsc:2;n:type:ShaderForge.SFN_Divide,id:6945,x:32351,y:32936,varname:node_6945,prsc:2|A-3597-OUT,B-6626-OUT;n:type:ShaderForge.SFN_Vector1,id:6626,x:32123,y:33079,varname:node_6626,prsc:2,v1:3;proporder:3139-7143;pass:END;sub:END;*/

Shader "Shader Forge/Test" {
    Properties {
        _BaseTex ("BaseTex", 2D) = "white" {}
        _Brush ("Brush", 2D) = "white" {}
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
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _BaseTex; uniform float4 _BaseTex_ST;
            uniform sampler2D _Brush; uniform float4 _Brush_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float node_2544 = max(0,dot(lightDirection,normalDirection));
                float node_5184 = 4.0;
                float node_4432 = (ceil((node_2544*node_5184))/node_5184);
                float3 finalColor = (((node_4432+node_2544+node_4432)/3.0)*_LightColor0.rgb);
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _BaseTex; uniform float4 _BaseTex_ST;
            uniform sampler2D _Brush; uniform float4 _Brush_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float node_2544 = max(0,dot(lightDirection,normalDirection));
                float node_5184 = 4.0;
                float node_4432 = (ceil((node_2544*node_5184))/node_5184);
                float3 finalColor = (((node_4432+node_2544+node_4432)/3.0)*_LightColor0.rgb);
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
