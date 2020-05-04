// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:37083,y:32314,varname:node_2865,prsc:2|diff-3483-OUT,spec-4808-OUT,gloss-9124-OUT,normal-3857-OUT,alpha-9809-OUT,refract-3251-OUT,voffset-5882-OUT,tess-2660-OUT;n:type:ShaderForge.SFN_Vector1,id:4808,x:33461,y:32828,varname:node_4808,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:9124,x:33304,y:32910,ptovrint:False,ptlb:Glossiness,ptin:_Glossiness,varname:node_9124,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.95;n:type:ShaderForge.SFN_Color,id:6937,x:32748,y:32189,ptovrint:False,ptlb:Color (Deep),ptin:_ColorDeep,varname:node_6937,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2578765,c2:0.3407347,c3:0.5754717,c4:1;n:type:ShaderForge.SFN_Color,id:4985,x:32748,y:32371,ptovrint:False,ptlb:Color (Shallow),ptin:_ColorShallow,varname:node_4985,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5189569,c2:0.7659891,c3:0.9245283,c4:1;n:type:ShaderForge.SFN_Lerp,id:2910,x:36031,y:31802,varname:node_2910,prsc:2|A-6937-RGB,B-4985-RGB,T-227-OUT;n:type:ShaderForge.SFN_Fresnel,id:227,x:32782,y:32569,varname:node_227,prsc:2|NRM-6324-OUT,EXP-6969-OUT;n:type:ShaderForge.SFN_NormalVector,id:6324,x:32347,y:32360,prsc:2,pt:True;n:type:ShaderForge.SFN_ValueProperty,id:8142,x:32347,y:32588,ptovrint:False,ptlb:Fresnel Strength,ptin:_FresnelStrength,varname:node_8142,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.336;n:type:ShaderForge.SFN_ConstantClamp,id:6969,x:32560,y:32620,varname:node_6969,prsc:2,min:0,max:4|IN-8142-OUT;n:type:ShaderForge.SFN_Tex2d,id:2153,x:32305,y:33038,varname:node_2153,prsc:2,ntxv:0,isnm:False|UVIN-5787-OUT,TEX-6209-TEX;n:type:ShaderForge.SFN_Tex2d,id:2894,x:32305,y:33239,varname:node_2894,prsc:2,ntxv:0,isnm:False|UVIN-435-OUT,TEX-6209-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:6209,x:32046,y:33056,ptovrint:False,ptlb:Normal Map,ptin:_NormalMap,varname:node_6209,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:4346,x:33123,y:33071,varname:node_4346,prsc:2|A-9519-OUT,B-1593-OUT,T-7309-OUT;n:type:ShaderForge.SFN_Slider,id:7309,x:33090,y:33328,ptovrint:False,ptlb:Normal Blend Strength,ptin:_NormalBlendStrength,varname:node_7309,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6087959,max:1;n:type:ShaderForge.SFN_Time,id:1589,x:31093,y:33732,varname:node_1589,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:6231,x:31588,y:32819,varname:node_6231,prsc:2;n:type:ShaderForge.SFN_Append,id:7240,x:31823,y:32819,varname:node_7240,prsc:2|A-6231-X,B-6231-Z;n:type:ShaderForge.SFN_Divide,id:9074,x:32016,y:32808,varname:node_9074,prsc:2|A-7240-OUT,B-2476-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2476,x:31910,y:32966,ptovrint:False,ptlb:UV Scale,ptin:_UVScale,varname:node_2476,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Set,id:4208,x:32241,y:32821,varname:_worldUV,prsc:2|IN-9074-OUT;n:type:ShaderForge.SFN_Set,id:86,x:31987,y:33330,varname:_UV1,prsc:2|IN-1067-OUT;n:type:ShaderForge.SFN_Set,id:317,x:31995,y:33709,varname:_UV2,prsc:2|IN-6283-OUT;n:type:ShaderForge.SFN_Vector4Property,id:3370,x:31093,y:33369,ptovrint:False,ptlb:UV1Animator,ptin:_UV1Animator,varname:node_3370,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Vector4Property,id:9801,x:31093,y:33556,ptovrint:False,ptlb:UV2Animator,ptin:_UV2Animator,varname:node_9801,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_ComponentMask,id:5552,x:31145,y:32953,varname:node_5552,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6494-OUT;n:type:ShaderForge.SFN_Get,id:9730,x:30368,y:33215,varname:node_9730,prsc:2|IN-4208-OUT;n:type:ShaderForge.SFN_ComponentMask,id:5191,x:31145,y:33144,varname:node_5191,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-4138-OUT;n:type:ShaderForge.SFN_Multiply,id:4593,x:31387,y:33330,varname:node_4593,prsc:2|A-3370-X,B-1589-TSL;n:type:ShaderForge.SFN_Multiply,id:1100,x:31387,y:33491,varname:node_1100,prsc:2|A-3370-Y,B-1589-TSL;n:type:ShaderForge.SFN_Add,id:579,x:31610,y:33330,varname:node_579,prsc:2|A-4593-OUT,B-5552-R;n:type:ShaderForge.SFN_Add,id:6068,x:31610,y:33491,varname:node_6068,prsc:2|A-1100-OUT,B-5552-G;n:type:ShaderForge.SFN_Append,id:1067,x:31800,y:33330,varname:node_1067,prsc:2|A-579-OUT,B-6068-OUT;n:type:ShaderForge.SFN_Add,id:6403,x:31610,y:33645,varname:node_6403,prsc:2|A-5191-R,B-7800-OUT;n:type:ShaderForge.SFN_Add,id:752,x:31610,y:33796,varname:node_752,prsc:2|A-5191-G,B-1040-OUT;n:type:ShaderForge.SFN_Multiply,id:1040,x:31393,y:34068,varname:node_1040,prsc:2|A-9801-Y,B-1589-TSL;n:type:ShaderForge.SFN_Multiply,id:7800,x:31393,y:33919,varname:node_7800,prsc:2|A-9801-X,B-1589-TSL;n:type:ShaderForge.SFN_Append,id:6283,x:31807,y:33709,varname:node_6283,prsc:2|A-6403-OUT,B-752-OUT;n:type:ShaderForge.SFN_Multiply,id:6494,x:30680,y:33286,varname:node_6494,prsc:2|A-9730-OUT,B-2312-OUT;n:type:ShaderForge.SFN_Multiply,id:4138,x:30680,y:33435,varname:node_4138,prsc:2|A-9730-OUT,B-3302-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2312,x:30434,y:33608,ptovrint:False,ptlb:UV1 Tiling,ptin:_UV1Tiling,varname:node_2312,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:3302,x:30434,y:33704,ptovrint:False,ptlb:UV2 Tiling,ptin:_UV2Tiling,varname:node_3302,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Get,id:5787,x:32190,y:32897,varname:node_5787,prsc:2|IN-86-OUT;n:type:ShaderForge.SFN_Get,id:435,x:32126,y:33302,varname:node_435,prsc:2|IN-317-OUT;n:type:ShaderForge.SFN_Multiply,id:1691,x:32721,y:32970,varname:node_1691,prsc:2|A-2953-OUT,B-77-OUT;n:type:ShaderForge.SFN_Slider,id:77,x:32550,y:33388,ptovrint:False,ptlb:Normal Strength 1,ptin:_NormalStrength1,varname:node_77,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Append,id:9519,x:32918,y:32940,varname:node_9519,prsc:2|A-1691-OUT,B-8280-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2953,x:32489,y:32960,varname:node_2953,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-2153-RGB;n:type:ShaderForge.SFN_Vector1,id:8280,x:32305,y:33179,varname:node_8280,prsc:2,v1:1;n:type:ShaderForge.SFN_ComponentMask,id:2760,x:32490,y:33129,varname:node_2760,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-2894-RGB;n:type:ShaderForge.SFN_Multiply,id:9951,x:32721,y:33139,varname:node_9951,prsc:2|A-2760-OUT,B-4344-OUT;n:type:ShaderForge.SFN_Slider,id:4344,x:32512,y:33504,ptovrint:False,ptlb:Normal Strength 2,ptin:_NormalStrength2,varname:node_4344,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Append,id:1593,x:32940,y:33139,varname:node_1593,prsc:2|A-9951-OUT,B-8280-OUT;n:type:ShaderForge.SFN_Set,id:7905,x:33440,y:33075,varname:_NormalMapping,prsc:2|IN-4346-OUT;n:type:ShaderForge.SFN_Get,id:3857,x:33317,y:32686,varname:node_3857,prsc:2|IN-7905-OUT;n:type:ShaderForge.SFN_Slider,id:6911,x:34374,y:32661,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_6911,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.230472,max:10;n:type:ShaderForge.SFN_Tex2d,id:3624,x:33951,y:33124,ptovrint:False,ptlb:Noise Map,ptin:_NoiseMap,varname:node_3624,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e958c6041cfe445e987c73751e8d4082,ntxv:0,isnm:False|UVIN-9166-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:993,x:34175,y:33056,varname:node_993,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-3624-RGB;n:type:ShaderForge.SFN_Multiply,id:3251,x:34400,y:33106,varname:node_3251,prsc:2|A-993-OUT,B-2826-OUT;n:type:ShaderForge.SFN_Slider,id:2826,x:34134,y:33298,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:node_2826,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0.9563187,max:1;n:type:ShaderForge.SFN_Panner,id:9166,x:33687,y:33226,varname:node_9166,prsc:2,spu:0.1,spv:0.1|UVIN-7929-UVOUT;n:type:ShaderForge.SFN_DepthBlend,id:4635,x:34749,y:32650,varname:node_4635,prsc:2|DIST-6911-OUT;n:type:ShaderForge.SFN_Divide,id:4126,x:35129,y:32536,varname:node_4126,prsc:2|A-4635-OUT,B-5208-OUT;n:type:ShaderForge.SFN_Dot,id:5208,x:35107,y:32706,varname:node_5208,prsc:2,dt:4|A-4493-OUT,B-606-OUT;n:type:ShaderForge.SFN_ViewVector,id:4493,x:34812,y:32819,varname:node_4493,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:606,x:34925,y:32908,prsc:2,pt:False;n:type:ShaderForge.SFN_Clamp01,id:7628,x:35377,y:32541,varname:node_7628,prsc:2|IN-4126-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:5290,x:35852,y:32470,varname:node_5290,prsc:2|IN-7628-OUT,IMIN-8916-OUT,IMAX-1998-OUT,OMIN-2673-OUT,OMAX-8182-OUT;n:type:ShaderForge.SFN_Vector1,id:2673,x:35648,y:32589,varname:node_2673,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:8182,x:35648,y:32651,varname:node_8182,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:8916,x:35473,y:32293,ptovrint:False,ptlb:FoamMin,ptin:_FoamMin,varname:node_8916,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:1998,x:35473,y:32400,ptovrint:False,ptlb:FoamMax,ptin:_FoamMax,varname:node_1998,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1589084,max:1;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:5098,x:35877,y:32723,varname:node_5098,prsc:2|IN-7628-OUT,IMIN-9641-OUT,IMAX-2459-OUT,OMIN-2673-OUT,OMAX-8182-OUT;n:type:ShaderForge.SFN_Slider,id:9641,x:35435,y:32832,ptovrint:False,ptlb:DepthMin,ptin:_DepthMin,varname:node_9641,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:2459,x:35435,y:32942,ptovrint:False,ptlb:DepthMax,ptin:_DepthMax,varname:node_2459,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Clamp01,id:8564,x:36016,y:32470,varname:node_8564,prsc:2|IN-5290-OUT;n:type:ShaderForge.SFN_ComponentMask,id:976,x:36334,y:32455,varname:node_976,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-5162-OUT;n:type:ShaderForge.SFN_Multiply,id:7401,x:36525,y:32465,varname:node_7401,prsc:2|A-976-OUT,B-2259-OUT;n:type:ShaderForge.SFN_Vector1,id:2259,x:36334,y:32627,varname:node_2259,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Clamp01,id:9809,x:36838,y:32541,varname:node_9809,prsc:2|IN-1643-OUT;n:type:ShaderForge.SFN_Multiply,id:982,x:36130,y:32295,varname:node_982,prsc:2|A-2835-OUT,B-4589-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2835,x:35973,y:32295,ptovrint:False,ptlb:FoamPower,ptin:_FoamPower,varname:node_2835,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.2;n:type:ShaderForge.SFN_Clamp01,id:5162,x:36565,y:32249,varname:node_5162,prsc:2|IN-5788-OUT;n:type:ShaderForge.SFN_Add,id:3359,x:36657,y:31849,varname:node_3359,prsc:2|A-2910-OUT,B-5162-OUT;n:type:ShaderForge.SFN_Clamp01,id:3483,x:36815,y:32020,varname:node_3483,prsc:2|IN-3359-OUT;n:type:ShaderForge.SFN_OneMinus,id:9114,x:36105,y:32627,varname:node_9114,prsc:2|IN-8564-OUT;n:type:ShaderForge.SFN_Tex2d,id:6968,x:35831,y:32028,ptovrint:False,ptlb:FoamTexture,ptin:_FoamTexture,varname:node_6968,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:832dc5a2501052b4d8f685a7cda3b30b,ntxv:0,isnm:False|UVIN-7530-UVOUT;n:type:ShaderForge.SFN_Multiply,id:6236,x:36279,y:32026,varname:node_6236,prsc:2|A-2910-OUT,B-8109-OUT;n:type:ShaderForge.SFN_Multiply,id:4589,x:36445,y:32075,varname:node_4589,prsc:2|A-6236-OUT,B-9114-OUT;n:type:ShaderForge.SFN_Add,id:5788,x:36392,y:32276,varname:node_5788,prsc:2|A-982-OUT,B-7565-OUT;n:type:ShaderForge.SFN_Power,id:7565,x:36313,y:32784,varname:node_7565,prsc:2|VAL-9114-OUT,EXP-9508-OUT;n:type:ShaderForge.SFN_Vector1,id:9508,x:36063,y:32869,varname:node_9508,prsc:2,v1:5;n:type:ShaderForge.SFN_OneMinus,id:8109,x:36050,y:32026,varname:node_8109,prsc:2|IN-6968-RGB;n:type:ShaderForge.SFN_Add,id:1643,x:36659,y:32619,varname:node_1643,prsc:2|A-7401-OUT,B-5098-OUT;n:type:ShaderForge.SFN_Panner,id:7530,x:35605,y:31969,varname:node_7530,prsc:2,spu:0.01,spv:0.01|UVIN-5235-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:5235,x:35283,y:31973,varname:node_5235,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_TexCoord,id:7929,x:33592,y:33403,varname:node_7929,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:293,x:36595,y:32903,ptovrint:False,ptlb:Tessel,ptin:_Tessel,varname:node_293,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.47925,max:10;n:type:ShaderForge.SFN_Tex2d,id:5709,x:36910,y:33050,ptovrint:False,ptlb:WaveDistortion,ptin:_WaveDistortion,varname:node_5709,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9250-UVOUT;n:type:ShaderForge.SFN_Panner,id:9250,x:36728,y:33045,varname:node_9250,prsc:2,spu:0.5,spv:0.5|UVIN-8735-UVOUT,DIST-8160-TSL;n:type:ShaderForge.SFN_TexCoord,id:8735,x:36466,y:32993,varname:node_8735,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:8160,x:36222,y:33126,varname:node_8160,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:2137,x:36397,y:33340,varname:node_2137,prsc:2;n:type:ShaderForge.SFN_Add,id:9460,x:36606,y:33215,varname:node_9460,prsc:2|A-2946-OUT,B-2137-X;n:type:ShaderForge.SFN_Multiply,id:7282,x:36822,y:33237,varname:node_7282,prsc:2|A-9460-OUT,B-743-OUT;n:type:ShaderForge.SFN_Vector1,id:743,x:36708,y:33408,varname:node_743,prsc:2,v1:10;n:type:ShaderForge.SFN_Sin,id:6146,x:36999,y:33237,varname:node_6146,prsc:2|IN-7282-OUT;n:type:ShaderForge.SFN_Multiply,id:5289,x:37191,y:33237,varname:node_5289,prsc:2|A-6146-OUT,B-5253-OUT;n:type:ShaderForge.SFN_Vector1,id:5253,x:36999,y:33390,varname:node_5253,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Add,id:2565,x:37388,y:33197,varname:node_2565,prsc:2|A-5289-OUT,B-4280-OUT;n:type:ShaderForge.SFN_Vector1,id:4280,x:37228,y:33380,varname:node_4280,prsc:2,v1:10;n:type:ShaderForge.SFN_RemapRange,id:9778,x:37591,y:33144,varname:node_9778,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-2565-OUT;n:type:ShaderForge.SFN_Multiply,id:1363,x:38136,y:33022,varname:node_1363,prsc:2|A-915-OUT,B-5767-OUT;n:type:ShaderForge.SFN_Multiply,id:915,x:37834,y:33191,varname:node_915,prsc:2|A-5709-R,B-9778-OUT;n:type:ShaderForge.SFN_Slider,id:5767,x:38189,y:33419,ptovrint:False,ptlb:WaveHeight,ptin:_WaveHeight,varname:node_5767,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4957161,max:1;n:type:ShaderForge.SFN_Append,id:6999,x:38422,y:33029,varname:node_6999,prsc:2|A-5175-OUT,B-1363-OUT;n:type:ShaderForge.SFN_Vector1,id:5175,x:38241,y:32769,varname:node_5175,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:8698,x:38718,y:33013,varname:node_8698,prsc:2|A-6999-OUT,B-9002-OUT;n:type:ShaderForge.SFN_Vector1,id:9002,x:38512,y:33197,varname:node_9002,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:2946,x:36397,y:33175,varname:node_2946,prsc:2|A-8160-TSL,B-8433-OUT;n:type:ShaderForge.SFN_Vector1,id:8433,x:36192,y:33272,varname:node_8433,prsc:2,v1:1;n:type:ShaderForge.SFN_ToggleProperty,id:8522,x:36579,y:32798,ptovrint:False,ptlb:Tesselation,ptin:_Tesselation,varname:node_8522,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_Multiply,id:2660,x:36953,y:32863,varname:node_2660,prsc:2|A-293-OUT,B-8522-OUT;n:type:ShaderForge.SFN_Multiply,id:5882,x:37664,y:32812,varname:node_5882,prsc:2|A-8698-OUT,B-8522-OUT;proporder:6937-4985-8142-9124-6209-77-4344-7309-3370-9801-2476-2312-3302-6911-3624-2826-8916-1998-2835-6968-9641-2459-293-5709-5767-8522;pass:END;sub:END;*/

Shader "Shader Forge/Ocean" {
    Properties {
        _ColorDeep ("Color (Deep)", Color) = (0.2578765,0.3407347,0.5754717,1)
        _ColorShallow ("Color (Shallow)", Color) = (0.5189569,0.7659891,0.9245283,1)
        _FresnelStrength ("Fresnel Strength", Float ) = 1.336
        _Glossiness ("Glossiness", Range(0, 0.95)) = 0
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength1 ("Normal Strength 1", Range(0, 1)) = 0
        _NormalStrength2 ("Normal Strength 2", Range(0, 1)) = 0
        _NormalBlendStrength ("Normal Blend Strength", Range(0, 1)) = 0.6087959
        _UV1Animator ("UV1Animator", Vector) = (0,0,0,0)
        _UV2Animator ("UV2Animator", Vector) = (0,0,0,0)
        _UVScale ("UV Scale", Float ) = 1
        _UV1Tiling ("UV1 Tiling", Float ) = 0
        _UV2Tiling ("UV2 Tiling", Float ) = 0
        _Opacity ("Opacity", Range(0, 10)) = 4.230472
        _NoiseMap ("Noise Map", 2D) = "white" {}
        _Refraction ("Refraction", Range(-1, 1)) = 0.9563187
        _FoamMin ("FoamMin", Range(0, 1)) = 0
        _FoamMax ("FoamMax", Range(0, 1)) = 0.1589084
        _FoamPower ("FoamPower", Float ) = 1.2
        _FoamTexture ("FoamTexture", 2D) = "white" {}
        _DepthMin ("DepthMin", Range(0, 1)) = 0
        _DepthMax ("DepthMax", Range(0, 1)) = 1
        _Tessel ("Tessel", Range(0, 10)) = 2.47925
        _WaveDistortion ("WaveDistortion", 2D) = "white" {}
        _WaveHeight ("WaveHeight", Range(0, 1)) = 0.4957161
        [MaterialToggle] _Tesselation ("Tesselation", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float _Glossiness;
            uniform float4 _ColorDeep;
            uniform float4 _ColorShallow;
            uniform float _FresnelStrength;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _NormalBlendStrength;
            uniform float _UVScale;
            uniform float4 _UV1Animator;
            uniform float4 _UV2Animator;
            uniform float _UV1Tiling;
            uniform float _UV2Tiling;
            uniform float _NormalStrength1;
            uniform float _NormalStrength2;
            uniform float _Opacity;
            uniform sampler2D _NoiseMap; uniform float4 _NoiseMap_ST;
            uniform float _Refraction;
            uniform float _FoamMin;
            uniform float _FoamMax;
            uniform float _DepthMin;
            uniform float _DepthMax;
            uniform float _FoamPower;
            uniform sampler2D _FoamTexture; uniform float4 _FoamTexture_ST;
            uniform float _Tessel;
            uniform sampler2D _WaveDistortion; uniform float4 _WaveDistortion_ST;
            uniform float _WaveHeight;
            uniform fixed _Tesselation;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 projPos : TEXCOORD7;
                UNITY_FOG_COORDS(8)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD9;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_8160 = _Time;
                float2 node_9250 = (o.uv0+node_8160.r*float2(0.5,0.5));
                float4 _WaveDistortion_var = tex2Dlod(_WaveDistortion,float4(TRANSFORM_TEX(node_9250, _WaveDistortion),0.0,0));
                v.vertex.xyz += (float3(float2(0.0,((_WaveDistortion_var.r*(((sin((((node_8160.r*1.0)+mul(unity_ObjectToWorld, v.vertex).r)*10.0))*0.3)+10.0)*0.5+0.5))*_WaveHeight)),0.0)*_Tesselation);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                    float2 texcoord1 : TEXCOORD1;
                    float2 texcoord2 : TEXCOORD2;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    o.texcoord2 = v.texcoord2;
                    return o;
                }
                float Tessellation(TessVertex v){
                    return (_Tessel*_Tesselation);
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_1589 = _Time;
                float2 _worldUV = (float2(i.posWorld.r,i.posWorld.b)/_UVScale);
                float2 node_9730 = _worldUV;
                float2 node_5552 = (node_9730*_UV1Tiling).rg;
                float2 _UV1 = float2(((_UV1Animator.r*node_1589.r)+node_5552.r),((_UV1Animator.g*node_1589.r)+node_5552.g));
                float2 node_5787 = _UV1;
                float3 node_2153 = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(node_5787, _NormalMap)));
                float node_8280 = 1.0;
                float2 node_5191 = (node_9730*_UV2Tiling).rg;
                float2 _UV2 = float2((node_5191.r+(_UV2Animator.r*node_1589.r)),(node_5191.g+(_UV2Animator.g*node_1589.r)));
                float2 node_435 = _UV2;
                float3 node_2894 = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(node_435, _NormalMap)));
                float3 _NormalMapping = lerp(float3((node_2153.rgb.rg*_NormalStrength1),node_8280),float3((node_2894.rgb.rg*_NormalStrength2),node_8280),_NormalBlendStrength);
                float3 normalLocal = _NormalMapping;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_9912 = _Time;
                float2 node_9166 = (i.uv0+node_9912.g*float2(0.1,0.1));
                float4 _NoiseMap_var = tex2D(_NoiseMap,TRANSFORM_TEX(node_9166, _NoiseMap));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (_NoiseMap_var.rgb.rg*_Refraction);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Glossiness;
                float perceptualRoughness = 1.0 - _Glossiness;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = 0.0;
                float specularMonochrome;
                float3 node_2910 = lerp(_ColorDeep.rgb,_ColorShallow.rgb,pow(1.0-max(0,dot(normalDirection, viewDirection)),clamp(_FresnelStrength,0,4)));
                float2 node_7530 = (i.uv0+node_9912.g*float2(0.01,0.01));
                float4 _FoamTexture_var = tex2D(_FoamTexture,TRANSFORM_TEX(node_7530, _FoamTexture));
                float node_7628 = saturate((saturate((sceneZ-partZ)/_Opacity)/0.5*dot(viewDirection,i.normalDir)+0.5));
                float node_2673 = 0.0;
                float node_8182 = 1.0;
                float node_9114 = (1.0 - saturate((node_2673 + ( (node_7628 - _FoamMin) * (node_8182 - node_2673) ) / (_FoamMax - _FoamMin))));
                float3 node_5162 = saturate(((_FoamPower*((node_2910*(1.0 - _FoamTexture_var.rgb))*node_9114))+pow(node_9114,5.0)));
                float3 diffuseColor = saturate((node_2910+node_5162)); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,saturate(((node_5162.r*0.5)+(node_2673 + ( (node_7628 - _DepthMin) * (node_8182 - node_2673) ) / (_DepthMax - _DepthMin))))),1);
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
            ZWrite Off
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float _Glossiness;
            uniform float4 _ColorDeep;
            uniform float4 _ColorShallow;
            uniform float _FresnelStrength;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _NormalBlendStrength;
            uniform float _UVScale;
            uniform float4 _UV1Animator;
            uniform float4 _UV2Animator;
            uniform float _UV1Tiling;
            uniform float _UV2Tiling;
            uniform float _NormalStrength1;
            uniform float _NormalStrength2;
            uniform float _Opacity;
            uniform sampler2D _NoiseMap; uniform float4 _NoiseMap_ST;
            uniform float _Refraction;
            uniform float _FoamMin;
            uniform float _FoamMax;
            uniform float _DepthMin;
            uniform float _DepthMax;
            uniform float _FoamPower;
            uniform sampler2D _FoamTexture; uniform float4 _FoamTexture_ST;
            uniform float _Tessel;
            uniform sampler2D _WaveDistortion; uniform float4 _WaveDistortion_ST;
            uniform float _WaveHeight;
            uniform fixed _Tesselation;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 projPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_8160 = _Time;
                float2 node_9250 = (o.uv0+node_8160.r*float2(0.5,0.5));
                float4 _WaveDistortion_var = tex2Dlod(_WaveDistortion,float4(TRANSFORM_TEX(node_9250, _WaveDistortion),0.0,0));
                v.vertex.xyz += (float3(float2(0.0,((_WaveDistortion_var.r*(((sin((((node_8160.r*1.0)+mul(unity_ObjectToWorld, v.vertex).r)*10.0))*0.3)+10.0)*0.5+0.5))*_WaveHeight)),0.0)*_Tesselation);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                    float2 texcoord1 : TEXCOORD1;
                    float2 texcoord2 : TEXCOORD2;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    o.texcoord2 = v.texcoord2;
                    return o;
                }
                float Tessellation(TessVertex v){
                    return (_Tessel*_Tesselation);
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_1589 = _Time;
                float2 _worldUV = (float2(i.posWorld.r,i.posWorld.b)/_UVScale);
                float2 node_9730 = _worldUV;
                float2 node_5552 = (node_9730*_UV1Tiling).rg;
                float2 _UV1 = float2(((_UV1Animator.r*node_1589.r)+node_5552.r),((_UV1Animator.g*node_1589.r)+node_5552.g));
                float2 node_5787 = _UV1;
                float3 node_2153 = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(node_5787, _NormalMap)));
                float node_8280 = 1.0;
                float2 node_5191 = (node_9730*_UV2Tiling).rg;
                float2 _UV2 = float2((node_5191.r+(_UV2Animator.r*node_1589.r)),(node_5191.g+(_UV2Animator.g*node_1589.r)));
                float2 node_435 = _UV2;
                float3 node_2894 = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(node_435, _NormalMap)));
                float3 _NormalMapping = lerp(float3((node_2153.rgb.rg*_NormalStrength1),node_8280),float3((node_2894.rgb.rg*_NormalStrength2),node_8280),_NormalBlendStrength);
                float3 normalLocal = _NormalMapping;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_9015 = _Time;
                float2 node_9166 = (i.uv0+node_9015.g*float2(0.1,0.1));
                float4 _NoiseMap_var = tex2D(_NoiseMap,TRANSFORM_TEX(node_9166, _NoiseMap));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (_NoiseMap_var.rgb.rg*_Refraction);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Glossiness;
                float perceptualRoughness = 1.0 - _Glossiness;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = 0.0;
                float specularMonochrome;
                float3 node_2910 = lerp(_ColorDeep.rgb,_ColorShallow.rgb,pow(1.0-max(0,dot(normalDirection, viewDirection)),clamp(_FresnelStrength,0,4)));
                float2 node_7530 = (i.uv0+node_9015.g*float2(0.01,0.01));
                float4 _FoamTexture_var = tex2D(_FoamTexture,TRANSFORM_TEX(node_7530, _FoamTexture));
                float node_7628 = saturate((saturate((sceneZ-partZ)/_Opacity)/0.5*dot(viewDirection,i.normalDir)+0.5));
                float node_2673 = 0.0;
                float node_8182 = 1.0;
                float node_9114 = (1.0 - saturate((node_2673 + ( (node_7628 - _FoamMin) * (node_8182 - node_2673) ) / (_FoamMax - _FoamMin))));
                float3 node_5162 = saturate(((_FoamPower*((node_2910*(1.0 - _FoamTexture_var.rgb))*node_9114))+pow(node_9114,5.0)));
                float3 diffuseColor = saturate((node_2910+node_5162)); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * saturate(((node_5162.r*0.5)+(node_2673 + ( (node_7628 - _DepthMin) * (node_8182 - node_2673) ) / (_DepthMax - _DepthMin)))),0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform float _Tessel;
            uniform sampler2D _WaveDistortion; uniform float4 _WaveDistortion_ST;
            uniform float _WaveHeight;
            uniform fixed _Tesselation;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
                float2 uv2 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                float4 node_8160 = _Time;
                float2 node_9250 = (o.uv0+node_8160.r*float2(0.5,0.5));
                float4 _WaveDistortion_var = tex2Dlod(_WaveDistortion,float4(TRANSFORM_TEX(node_9250, _WaveDistortion),0.0,0));
                v.vertex.xyz += (float3(float2(0.0,((_WaveDistortion_var.r*(((sin((((node_8160.r*1.0)+mul(unity_ObjectToWorld, v.vertex).r)*10.0))*0.3)+10.0)*0.5+0.5))*_WaveHeight)),0.0)*_Tesselation);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                    float2 texcoord1 : TEXCOORD1;
                    float2 texcoord2 : TEXCOORD2;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    o.texcoord2 = v.texcoord2;
                    return o;
                }
                float Tessellation(TessVertex v){
                    return (_Tessel*_Tesselation);
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform sampler2D _CameraDepthTexture;
            uniform float _Glossiness;
            uniform float4 _ColorDeep;
            uniform float4 _ColorShallow;
            uniform float _FresnelStrength;
            uniform float _Opacity;
            uniform float _FoamMin;
            uniform float _FoamMax;
            uniform float _FoamPower;
            uniform sampler2D _FoamTexture; uniform float4 _FoamTexture_ST;
            uniform float _Tessel;
            uniform sampler2D _WaveDistortion; uniform float4 _WaveDistortion_ST;
            uniform float _WaveHeight;
            uniform fixed _Tesselation;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float4 projPos : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_8160 = _Time;
                float2 node_9250 = (o.uv0+node_8160.r*float2(0.5,0.5));
                float4 _WaveDistortion_var = tex2Dlod(_WaveDistortion,float4(TRANSFORM_TEX(node_9250, _WaveDistortion),0.0,0));
                v.vertex.xyz += (float3(float2(0.0,((_WaveDistortion_var.r*(((sin((((node_8160.r*1.0)+mul(unity_ObjectToWorld, v.vertex).r)*10.0))*0.3)+10.0)*0.5+0.5))*_WaveHeight)),0.0)*_Tesselation);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                    float2 texcoord1 : TEXCOORD1;
                    float2 texcoord2 : TEXCOORD2;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    o.texcoord2 = v.texcoord2;
                    return o;
                }
                float Tessellation(TessVertex v){
                    return (_Tessel*_Tesselation);
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : SV_Target {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float3 node_2910 = lerp(_ColorDeep.rgb,_ColorShallow.rgb,pow(1.0-max(0,dot(normalDirection, viewDirection)),clamp(_FresnelStrength,0,4)));
                float4 node_7190 = _Time;
                float2 node_7530 = (i.uv0+node_7190.g*float2(0.01,0.01));
                float4 _FoamTexture_var = tex2D(_FoamTexture,TRANSFORM_TEX(node_7530, _FoamTexture));
                float node_7628 = saturate((saturate((sceneZ-partZ)/_Opacity)/0.5*dot(viewDirection,i.normalDir)+0.5));
                float node_2673 = 0.0;
                float node_8182 = 1.0;
                float node_9114 = (1.0 - saturate((node_2673 + ( (node_7628 - _FoamMin) * (node_8182 - node_2673) ) / (_FoamMax - _FoamMin))));
                float3 node_5162 = saturate(((_FoamPower*((node_2910*(1.0 - _FoamTexture_var.rgb))*node_9114))+pow(node_9114,5.0)));
                float3 diffColor = saturate((node_2910+node_5162));
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, 0.0, specColor, specularMonochrome );
                float roughness = 1.0 - _Glossiness;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
