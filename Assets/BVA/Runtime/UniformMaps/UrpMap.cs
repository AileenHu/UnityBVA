﻿using GLTF.Schema;
using UnityEngine;
using UnityEngine.Rendering;
using Material = UnityEngine.Material;
using Texture = UnityEngine.Texture;
using BlendMode = GLTF.Schema.BlendMode;
namespace BVA
{
    public class UrpMap : IUniformMap
    {
        protected Material _material;
        private AlphaMode _alphaMode = AlphaMode.OPAQUE;
        private BlendMode _blendMode = BlendMode.ALPHA;
        private float _alphaCutoff = 0.5f;

        private Vector2 normalOffset = new Vector2(0, 0);
        private Vector2 occlusionOffset = new Vector2(0, 0);
        private Vector2 emissiveOffset = new Vector2(0, 0);

        protected UrpMap(string shaderName, int MaxLOD = 1000)
        {
            var s = Shader.Find(shaderName);
            if (s == null)
            {
                throw new ShaderNotFoundException(shaderName + " not found. Did you forget to add it to the build?");
            }

            s.maximumLOD = MaxLOD;
            _material = new Material(s);
        }

        protected UrpMap(Material mat, int MaxLOD = 1000)
        {
            mat.shader.maximumLOD = MaxLOD;
            _material = mat;

            if (mat.HasProperty("_Cutoff"))
            {
                _alphaCutoff = mat.GetFloat("_Cutoff");
            }

            switch (mat.renderQueue)
            {
                case (int)RenderQueue.AlphaTest:
                    _alphaMode = AlphaMode.MASK;
                    break;
                case (int)RenderQueue.Transparent:
                    _alphaMode = AlphaMode.BLEND;
                    break;
                case (int)RenderQueue.Geometry:
                default:
                    _alphaMode = AlphaMode.OPAQUE;
                    break;
            }
        }

        public Material Material { get { return _material; } }

        public virtual Texture NormalTexture
        {
            get { return _material.GetTexture("_BumpMap") ?? _material.GetTexture("_NormalMap"); }
            set
            {
                if (_material.HasProperty("_BumpMap"))
                {
                    _material.SetTexture("_BumpMap", value);
                    _material.EnableKeyword("_NORMALMAP");
                }else if (_material.HasProperty("_NormalMap"))
                {
                    _material.SetTexture("_NormalMap", value);
                    _material.EnableKeyword("_NORMALMAP");
                }
                else
                {
                    _material.DisableKeyword("_NORMALMAP");
                    Debug.LogWarning("Tried to set a normal map texture to a material that does not support it.");
                }
            }
        }

        // not implemented by the Standard shader
        public virtual int NormalTexCoord
        {
            get { return 0; }
            set { return; }
        }

        public virtual Vector2 NormalXOffset
        {
            get { return normalOffset; }
            set
            {
                normalOffset = value;
                var unitySpaceVec = new Vector2(normalOffset.x, 1 - NormalXScale.y - normalOffset.y);
                _material.SetTextureOffset("_BumpMap", unitySpaceVec);
            }
        }

        public virtual float NormalXRotation
        {
            get { return 0; }
            set { return; }
        }

        public virtual Vector2 NormalXScale
        {
            get { return _material.GetTextureScale("_BumpMap"); }
            set
            {
                _material.SetTextureScale("_BumpMap", value);
                NormalXOffset = normalOffset;
            }
        }

        public virtual int NormalXTexCoord
        {
            get { return 0; }
            set { return; }
        }

        public virtual float NormalTexScale
        {
            get { return _material.HasProperty("_BumpScale") ? _material.GetFloat("_BumpScale") : 1; }
            set
            {
                if (_material.HasProperty("_BumpScale"))
                {
                    _material.SetFloat("_BumpScale", (float)value);
                }
                else
                {
                    Debug.LogWarning("Tried to set a normal map scale to a material that does not support it.");
                }
            }
        }

        public virtual Texture OcclusionTexture
        {
            get { return _material.HasProperty("_OcclusionMap") ? _material.GetTexture("_OcclusionMap") : null; }
            set
            {
                if (_material.HasProperty("_OcclusionMap"))
                {
                    _material.SetTexture("_OcclusionMap", value);
                    _material.EnableKeyword("_OCCLUSIONMAP");
                }
                else
                {
                    _material.DisableKeyword("_OCCLUSIONMAP");
                    Debug.LogWarning("Tried to set an occlusion map to a material that does not support it.");
                }
            }
        }

        // not implemented by the Standard shader
        public virtual int OcclusionTexCoord
        {
            get { return 0; }
            set { return; }
        }

        public virtual Vector2 OcclusionXOffset
        {
            get { return occlusionOffset; }
            set
            {
                occlusionOffset = value;
                var unitySpaceVec = new Vector2(occlusionOffset.x, 1 - OcclusionXScale.y - occlusionOffset.y);
                _material.SetTextureOffset("_OcclusionMap", unitySpaceVec);
            }
        }

        public virtual float OcclusionXRotation
        {
            get { return 0; }
            set { return; }
        }

        public virtual Vector2 OcclusionXScale
        {
            get { return _material.GetTextureScale("_OcclusionMap"); }
            set
            {
                _material.SetTextureScale("_OcclusionMap", value);
                OcclusionXOffset = occlusionOffset;
            }
        }

        public virtual int OcclusionXTexCoord
        {
            get { return 0; }
            set { return; }
        }

        public virtual float OcclusionTexStrength
        {
            get { return _material.HasProperty("_OcclusionStrength") ? _material.GetFloat("_OcclusionStrength") : 1; }
            set
            {
                if (_material.HasProperty("_OcclusionStrength"))
                {
                    _material.SetFloat("_OcclusionStrength", (float)value);
                }
                else
                {
                    Debug.LogWarning("Tried to set occlusion strength to a material that does not support it.");
                }
            }
        }

        public virtual Texture EmissiveTexture
        {
            get { return _material.HasProperty("_EmissionMap") ? _material.GetTexture("_EmissionMap") : null; }
            set
            {
                if (value == null)
                {
                    _material.DisableKeyword("_EMISSION");
                    _material.SetFloat("_EmissionEnabled", 0);
                    return;
                }
                if (_material.HasProperty("_EmissionMap"))
                {
                    _material.EnableKeyword("_EMISSION");
                    _material.SetFloat("_EmissionEnabled", 1);
                    _material.globalIlluminationFlags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                    _material.SetTexture("_EmissionMap", value);
                }
                else
                {
                    Debug.LogWarning("Tried to set an emission map to a material that does not support it.");
                }
            }
        }

        // not implemented by the Standard shader
        public virtual int EmissiveTexCoord
        {
            get { return 0; }
            set { return; }
        }

        public virtual Vector2 EmissiveXOffset
        {
            get { return emissiveOffset; }
            set
            {
                emissiveOffset = value;
                var unitySpaceVec = new Vector2(emissiveOffset.x, 1 - EmissiveXScale.y - emissiveOffset.y);
                _material.SetTextureOffset("_EmissionMap", unitySpaceVec);
            }
        }

        public virtual float EmissiveXRotation
        {
            get { return 0; }
            set { return; }
        }

        public virtual Vector2 EmissiveXScale
        {
            get { return _material.GetTextureScale("_EmissionMap"); }
            set
            {
                _material.SetTextureScale("_EmissionMap", value);
                EmissiveXOffset = emissiveOffset;
            }
        }

        public virtual int EmissiveXTexCoord
        {
            get { return 0; }
            set { return; }
        }

        public virtual Color EmissiveFactor
        {
            get { return _material.HasProperty("_EmissionColor") ? _material.GetColor("_EmissionColor") : Color.white; }
            set
            {
                if(value == Color.black)
                {
                    _material.DisableKeyword("_EMISSION");
                    _material.SetFloat("_EmissionEnabled", 0);
                    return;
                }
                if (_material.HasProperty("_EmissionColor"))
                {
                    _material.EnableKeyword("_EMISSION");
                    _material.globalIlluminationFlags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                    _material.SetColor("_EmissionColor", value);
                }
                else
                {
                    Debug.LogWarning("Tried to set an emission factor to a material that does not support it.");
                }
            }
        }

        public float ClearcoatFactor
        {
            get
            {
                return _material.IsKeywordEnabled("_CLEARCOAT") && _material.GetFloat("_ClearCoat") > 0 ? _material.GetFloat("_ClearCoatMask") : 0;
            }
            set
            {
                _material.SetFloat("_ClearCoatMask", (float)value);
                if (value == 0)
                {
                    _material.SetFloat("_ClearCoat", 0);
                }
                else
                {
                    _material.SetFloat("_ClearCoat", 1.0f);
                    _material.EnableKeyword("_CLEARCOAT");
                }
            }
        }

        public Texture ClearcoatTexture
        {
            get { return _material.HasProperty("_ClearCoatMap") ? _material.GetTexture("_ClearCoatMap") : null; }
            set
            {
                if (_material.HasProperty("_ClearCoatMap"))
                {
                    if (value != null)
                    {
                        _material.EnableKeyword("_CLEARCOATMAP");
                        _material.SetTexture("_ClearCoatMap", value);
                    }
                }
                else
                {
                    Debug.LogWarning("Tried to set an emission map to a material that does not support it.");
                }
            }
        }

        public float ClearcoatRoughnessFactor
        {
            get
            {
                return 1.0f - _material.GetFloat("_ClearCoatSmoothness");
            }
            set
            {
                _material.SetFloat("_ClearCoatSmoothness", 1.0f - (float)value);
                _material.EnableKeyword("_CLEARCOAT");
            }
        }

        public Texture ClearcoatRoughnessTexture
        {
            get { return null; }
            set
            {
                Debug.LogWarning("does not support ClearcoatRoughnessTexture on URP");
            }
        }

        public virtual AlphaMode AlphaMode
        {
            get { return _alphaMode; }
            set
            {
                if (value == AlphaMode.MASK)
                {
                    _material.SetOverrideTag("RenderType", "TransparentCutout");
                    _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    _material.SetInt("_ZWrite", 1);
                    _material.EnableKeyword("_ALPHATEST_ON");
                    _material.DisableKeyword("_ALPHABLEND_ON");
                    _material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    _material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    if (_material.HasProperty("_Cutoff"))
                    {
                        _material.SetFloat("_Cutoff", (float)_alphaCutoff);
                    }
                }
                else if (value == AlphaMode.BLEND)
                {
                    _material.SetOverrideTag("RenderType", "Transparent");
                    _material.SetInt("_Surface", 1);
                    _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    _material.SetInt("_ZWrite", 0);
                    _material.DisableKeyword("_ALPHATEST_ON");
                    _material.EnableKeyword("_ALPHABLEND_ON");
                    _material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    _material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }
                else
                {
                    _material.SetOverrideTag("RenderType", "Opaque");
                    _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    _material.SetInt("_ZWrite", 1);
                    _material.DisableKeyword("_ALPHATEST_ON");
                    _material.DisableKeyword("_ALPHABLEND_ON");
                    _material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    _material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                }

                _alphaMode = value;
            }
        }

        public virtual float AlphaCutoff
        {
            get { return _alphaCutoff; }
            set
            {
                if ((_alphaMode == AlphaMode.MASK) && _material.HasProperty("_Cutoff"))
                {
                    _material.SetFloat("_Cutoff", (float)value);
                }
                _alphaCutoff = value;
            }
        }

        public BlendMode BlendMode
        {
            get => _blendMode;
            set
            {
                _blendMode = value;
                _material.SetFloat("_Blend", (float)_blendMode);
            }
        }

        public virtual bool DoubleSided
        {
            get { return _material.GetInt("_Cull") == (int)CullMode.Off; }
            set
            {
                if (value)
                {
                    _material.SetFloat("_Cull", (float)CullMode.Off);
                }
                else
                {
                    _material.SetFloat("_Cull", (float)CullMode.Back);
                }
            }
        }

        public virtual bool VertexColorsEnabled
        {
            get { return _material.IsKeywordEnabled("VERTEX_COLOR_ON"); }
            set
            {
                if (value)
                    _material.EnableKeyword("VERTEX_COLOR_ON");
                else
                    _material.DisableKeyword("VERTEX_COLOR_ON");
            }
        }

        public bool EnableInstance
        {
            get { return _material.enableInstancing; }
            set { _material.enableInstancing = value; }
        }

        public virtual IUniformMap Clone()
        {
            var ret = new UrpMap(new Material(_material));
            ret._alphaMode = _alphaMode;
            ret._alphaCutoff = _alphaCutoff;
            return ret;
        }

        protected virtual void Copy(IUniformMap o)
        {
            var other = (UrpMap)o;
            other._material = _material;
            other._alphaCutoff = _alphaCutoff;
            other._alphaMode = _alphaMode;
        }
    }
}
