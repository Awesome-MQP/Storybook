// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.25;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:32716,y:32678,varname:node_4795,prsc:2|emission-2393-OUT,alpha-798-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31577,y:32683,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5f46e4b955ecddf4daca1faa98ec627a,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32507,y:32678,varname:node_2393,prsc:2|A-5788-OUT,B-2053-RGB,C-797-RGB,D-9248-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32235,y:32772,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:32235,y:32930,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:32235,y:33081,varname:node_9248,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:798,x:32507,y:32892,varname:node_798,prsc:2|A-6074-A,B-2053-A,C-797-A;n:type:ShaderForge.SFN_Tex2d,id:9731,x:31343,y:32274,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_9731,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:f1be560a6455fca4e8444a3ae674e1b1,ntxv:0,isnm:False|UVIN-4911-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:5550,x:31343,y:32487,ptovrint:False,ptlb:Circle,ptin:_Circle,varname:node_5550,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3a5a96df060a5cf4a9cc0c59e13486b7,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:460,x:31585,y:32407,varname:node_460,prsc:2|A-9731-RGB,B-5550-RGB,C-6913-OUT;n:type:ShaderForge.SFN_Rotator,id:4911,x:31122,y:32274,varname:node_4911,prsc:2|UVIN-3665-UVOUT,SPD-463-OUT;n:type:ShaderForge.SFN_TexCoord,id:3665,x:30804,y:32142,varname:node_3665,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector1,id:463,x:30804,y:32427,varname:node_463,prsc:2,v1:-1;n:type:ShaderForge.SFN_RemapRange,id:2997,x:31775,y:32529,varname:node_2997,prsc:2,frmn:0,frmx:1,tomn:0.8,tomx:1|IN-460-OUT;n:type:ShaderForge.SFN_Multiply,id:7942,x:32026,y:32668,varname:node_7942,prsc:2|A-2997-OUT,B-6074-RGB,C-6074-A;n:type:ShaderForge.SFN_Vector1,id:6913,x:31324,y:32668,varname:node_6913,prsc:2,v1:5;n:type:ShaderForge.SFN_Tex2d,id:1994,x:31585,y:32157,ptovrint:False,ptlb:Stars,ptin:_Stars,varname:node_1994,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c5cf15dc16816a04aa9717de388a69f1,ntxv:0,isnm:False|UVIN-8980-OUT;n:type:ShaderForge.SFN_Multiply,id:2613,x:31819,y:32272,varname:node_2613,prsc:2|A-1994-RGB,B-460-OUT;n:type:ShaderForge.SFN_TexCoord,id:1324,x:31205,y:32002,varname:node_1324,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:8980,x:31402,y:32014,varname:node_8980,prsc:2|A-1324-UVOUT,B-273-OUT;n:type:ShaderForge.SFN_Vector1,id:273,x:31205,y:32155,varname:node_273,prsc:2,v1:1;n:type:ShaderForge.SFN_Add,id:5788,x:32273,y:32517,varname:node_5788,prsc:2|A-2613-OUT,B-7942-OUT;proporder:6074-797-9731-5550-1994;pass:END;sub:END;*/

Shader "Shader Forge/GalaxyShader" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _Noise ("Noise", 2D) = "white" {}
        _Circle ("Circle", 2D) = "white" {}
        _Stars ("Stars", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
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
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform sampler2D _Circle; uniform float4 _Circle_ST;
            uniform sampler2D _Stars; uniform float4 _Stars_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_8980 = (i.uv0*1.0);
                float4 _Stars_var = tex2D(_Stars,TRANSFORM_TEX(node_8980, _Stars));
                float4 node_7340 = _Time + _TimeEditor;
                float node_4911_ang = node_7340.g;
                float node_4911_spd = (-1.0);
                float node_4911_cos = cos(node_4911_spd*node_4911_ang);
                float node_4911_sin = sin(node_4911_spd*node_4911_ang);
                float2 node_4911_piv = float2(0.5,0.5);
                float2 node_4911 = (mul(i.uv0-node_4911_piv,float2x2( node_4911_cos, -node_4911_sin, node_4911_sin, node_4911_cos))+node_4911_piv);
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_4911, _Noise));
                float4 _Circle_var = tex2D(_Circle,TRANSFORM_TEX(i.uv0, _Circle));
                float3 node_460 = (_Noise_var.rgb*_Circle_var.rgb*5.0);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = (((_Stars_var.rgb*node_460)+((node_460*0.2+0.8)*_MainTex_var.rgb*_MainTex_var.a))*i.vertexColor.rgb*_TintColor.rgb*1.0);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,(_MainTex_var.a*i.vertexColor.a*_TintColor.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
