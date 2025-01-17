/// based on: UniversalStructs.cs

using UnityEditor.ShaderGraph;

namespace Lilium.ToonGraph.Editor
{
    static class UniversalStructs
    {
        public static StructDescriptor Varyings = new StructDescriptor()
        {
            name = "Varyings",
            packFields = true,
            fields = new FieldDescriptor[]
            {
                StructFields.Varyings.positionCS,
                StructFields.Varyings.positionWS,
                StructFields.Varyings.normalWS,
                StructFields.Varyings.tangentWS,
                StructFields.Varyings.texCoord0,
                StructFields.Varyings.texCoord1,
                StructFields.Varyings.texCoord2,
                StructFields.Varyings.texCoord3,
                StructFields.Varyings.color,
                StructFields.Varyings.screenPosition,
                UniversalStructFields.Varyings.lightmapUV,
                UniversalStructFields.Varyings.sh,
                UniversalStructFields.Varyings.fogFactorAndVertexLight,
                UniversalStructFields.Varyings.shadowCoord,
                StructFields.Varyings.instanceID,
                UniversalStructFields.Varyings.stereoTargetEyeIndexAsBlendIdx0,
                UniversalStructFields.Varyings.stereoTargetEyeIndexAsRTArrayIdx,
                StructFields.Varyings.cullFace,
            }
        };
    }
}
