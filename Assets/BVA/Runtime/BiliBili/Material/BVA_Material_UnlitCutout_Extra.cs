using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using GLTF.Extensions;
using BVA.Extensions;
using System.Threading.Tasks;
using UnityEngine;
using Color = UnityEngine.Color;
using Vector4 = UnityEngine.Vector4;

namespace GLTF.Schema.BVA
{
public class BVA_Material_UnlitCutout_Extra : MaterialExtra
    {
public const string PROPERTY = "BVA_Material_UnlitCutout_Extra";
public const string SHADER_NAME = "VRM/UnlitCutout";
public const string MAINTEX = "_MainTex";
public const string CUTOFF = "_Cutoff";
public MaterialTextureParam parameter__MainTex = new MaterialTextureParam(MAINTEX);
public MaterialParam<float> parameter__Cutoff = new MaterialParam<float>(CUTOFF, 1.0f);
public string[] keywords;
public BVA_Material_UnlitCutout_Extra(Material material, ExportTextureInfo exportTextureInfo, ExportTextureInfo exportNormalTextureInfo, ExportCubemapInfo exportCubemapInfo)
{
keywords = material.shaderKeywords;
var parameter__maintex_temp = material.GetTexture(parameter__MainTex.ParamName);
if (parameter__maintex_temp != null) parameter__MainTex.Value = exportTextureInfo(parameter__maintex_temp);
parameter__Cutoff.Value = material.GetFloat(parameter__Cutoff.ParamName);
}
public static async Task Deserialize(GLTFRoot root, JsonReader reader, Material matCache,AsyncLoadTexture loadTexture, AsyncLoadTexture loadNormalMap, AsyncLoadCubemap loadCubemap)
{
while (reader.Read())
{
if (reader.TokenType == JsonToken.PropertyName)
{
var curProp = reader.Value.ToString();
switch (curProp)
{
case BVA_Material_UnlitCutout_Extra.MAINTEX:
{
var texInfo = TextureInfo.Deserialize(root, reader);
var tex = await loadTexture(texInfo.Index);
matCache.SetTexture(BVA_Material_UnlitCutout_Extra.MAINTEX, tex);
}
break;
case BVA_Material_UnlitCutout_Extra.CUTOFF:
matCache.SetFloat(BVA_Material_UnlitCutout_Extra.CUTOFF, reader.ReadAsFloat());
break;
case nameof(keywords):
{
var keywords = reader.ReadStringList();
foreach (var keyword in keywords)
matCache.EnableKeyword(keyword);
}
break;
}
}
}
}
public override JProperty Serialize()
{
JObject jo = new JObject();
if (parameter__MainTex != null && parameter__MainTex.Value != null) jo.Add(parameter__MainTex.ParamName, parameter__MainTex.Serialize());
jo.Add(parameter__Cutoff.ParamName, parameter__Cutoff.Value);
if(keywords != null && keywords.Length > 0)
{
JArray jKeywords = new JArray();
foreach (var keyword in jKeywords)
jKeywords.Add(keyword);
jo.Add(nameof(keywords), jKeywords);
}
return new JProperty(BVA_Material_UnlitCutout_Extra.SHADER_NAME, jo);
}
}
}