// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Yash/SpotLight" {
    Properties {
      _Color("Main Color",Color) = (1,1,1,.5)
    	_Size("Size",Float) = 1
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" }
        Blend One One
        Pass
        {
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"


  			fixed4 _Color;
        struct v2f
        {
            float4 pos : SV_POSITION;
            float e : texcoord1;
            fixed4 color : Color;
            
        };
            
            

       float _Size;

        v2f vert(appdata_full v)
        {
            v2f o;
            float e = 1-(v.vertex.y+1) * 0.5;
          	o.e = e;
            e *= _Size;
            v.vertex.xz *= float2(e,e);
            o.pos = UnityObjectToClipPos(v.vertex);
            return o;

        }
            
            
        fixed4 frag(v2f i) : COLOR {
        
            return _Color * pow(i.e,2);
            
        }

            ENDCG
        }
    } 
    FallBack "Mobile/Transparent/Vertex Color"
}