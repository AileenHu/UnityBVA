using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using UnityEditor.Rendering;

namespace BVA
{
    public sealed class URPSettingsValidator : ISettingsValidator
    {
        public bool IsValid => GetHeaderDescription()==null;
        public string HeaderDescription => GetHeaderDescriptionFull();
        public string CurrentValueDescription => "";
        public string RecommendedValueDescription => IsValid ? ExportCommon.Localization("���޸�", "OK") : ExportCommon.Localization("�޸�", "Fix it");

        public bool CanFix => true;

        string GetHeaderDescriptionFull()
        {
            var result = GetHeaderDescription();
            if (result == null)
            {
                result = ExportCommon.Localization("URPSetting�����޳�ͻ", "Color Space Settings is valid");
            }
            return result;
        }

        string GetHeaderDescription()
        {
            if (QualitySettings.renderPipeline == null || GraphicsSettings.renderPipelineAsset == null)
            {
                return ExportCommon.Localization("����ʹ�� BVA ��Ӧ����Ҫ��Ч��ͨ����Ⱦ������Դ", "A valid Universal Render Pipeline Asset is required for build the App that using BVA");
            }

            return null;
        }

        public void Validate()
        {
            UniversalRenderPipelineAsset pipeLineAsset = AssetDatabase.LoadAssetAtPath("Packages/com.bilibili.bva/URP/BVAUniversalRenderPipelineAsset.asset", typeof(UniversalRenderPipelineAsset)) as UniversalRenderPipelineAsset;
            if (pipeLineAsset == null)
            {
                pipeLineAsset = AssetDatabase.LoadAssetAtPath("Assets/BVA/URP/BVAUniversalRenderPipelineAsset.asset", typeof(UniversalRenderPipelineAsset)) as UniversalRenderPipelineAsset;
            }
            Debug.Assert(pipeLineAsset != null);

            QualitySettings.renderPipeline = GraphicsSettings.renderPipelineAsset = pipeLineAsset;
        }
    }

    public sealed class ColorSpaceSettingsValidator : ISettingsValidator
    {
        public bool IsValid => GetHeaderDescription() == null;
        public string HeaderDescription => GetHeaderDescriptionFull();
        public string CurrentValueDescription => "";
        public string RecommendedValueDescription => IsValid ? ExportCommon.Localization("���޸�", "OK") : ExportCommon.Localization("�޸�", "Fix it");

        public bool CanFix => true;

        string GetHeaderDescriptionFull()
        {
            var result = GetHeaderDescription();
            if (result == null)
            {
                result = ExportCommon.Localization("ɫ�ʿռ������޳�ͻ", "Color Space Settings is valid");
            }
            return result;
        }
        string GetHeaderDescription()
        {
            if (PlayerSettings.colorSpace != UnityEngine.ColorSpace.Linear)
            {
                return ExportCommon.Localization("ɫ�ʿռ䣺Ӧ�ڡ�������á��������������ɫ�ʿռ��Ի�����Ч������ǰ����Ϊ", "COLORSPACE: Linear color space should be enabled on Player Settings Panel for best results. Currently set to "
                ) + PlayerSettings.colorSpace.ToString();
            }
            
            return null;
        }
        public void Validate()
        {
            PlayerSettings.colorSpace = UnityEngine.ColorSpace.Linear;
        }
    }

    public sealed class ReflectionProbeSettingsValidator : ISettingsValidator
    {
        public bool IsValid => GetHeaderDescription() == null;
        public string HeaderDescription => GetHeaderDescriptionFull();
        public string CurrentValueDescription => "";
        public string RecommendedValueDescription => IsValid ? ExportCommon.Localization("���޸�", "OK") : ExportCommon.Localization("�޸�", "Fix it");

        public bool CanFix => true;

        string GetHeaderDescriptionFull()
        {
            var result = GetHeaderDescription();
            if (result == null)
            {
                result = ExportCommon.Localization("����̽�������޳�ͻ", "Reflection Probe Settings is valid");
            }
            return result;
        }

        string GetHeaderDescription()
        {
            if (Graphics.activeTier == GraphicsTier.Tier1)
            {
                if (EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier1).reflectionProbeBlending)
                {
                    return ExportCommon.Localization("ͼ�ε� 1 �㣺��֧�ַ���̽ͷ��ϡ��� Tier 1 ��������Ͻ��÷���̽ͷ����Ի�����Ч��"
               , "GRAPHICS TIER 1: Reflection probe blending not supported. Disable reflection probe blending on Tier 1 Settings Panel for best results.");
                }
            }
            else if (Graphics.activeTier == GraphicsTier.Tier2)
            {
                if (EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier2).reflectionProbeBlending)
                {
                    return ExportCommon.Localization("ͼ�ε� 2 �㣺��֧�ַ���̽ͷ��ϡ��� Tier2 ��������Ͻ��÷���̽ͷ����Ի�����Ч��"
               , "GRAPHICS TIER 1: Reflection probe blending not supported. Disable reflection probe blending on Tier 1 Settings Panel for best results.");
                }
            }
            else if (Graphics.activeTier == GraphicsTier.Tier3 && EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier3).reflectionProbeBlending)
            {
                return ExportCommon.Localization("ͼ�ε� 3 �㣺��֧�ַ���̽ͷ��ϡ��� Tier 3 ��������Ͻ��÷���̽ͷ����Ի�����Ч��"
               , "GRAPHICS TIER 1: Reflection probe blending not supported. Disable reflection probe blending on Tier 1 Settings Panel for best results.");
            }
            return null;
        }

        public void Validate()
        {
            var setting = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, Graphics.activeTier);
            setting.reflectionProbeBlending = false;
            EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Standalone, Graphics.activeTier, setting);
        }

    }

    public sealed class LightmapSettingsValidator : ISettingsValidator
    {
        public bool IsValid => GetHeaderDescription() == null;
        public string HeaderDescription => GetHeaderDescriptionFull();
        public string CurrentValueDescription => "";
        public string RecommendedValueDescription => IsValid ? ExportCommon.Localization("���޸�", "OK") : ExportCommon.Localization("�޸�", "Fix it");

        public bool CanFix => true;

        string GetHeaderDescriptionFull()
        {
            var result = GetHeaderDescription();
            if (result == null)
            {
                result = ExportCommon.Localization("���������޳�ͻ", "Lightmap Settings is valid");
            }
            return result;
        }

        string GetHeaderDescription()
        {
            if (Lightmapping.realtimeGI)
            {
                return ExportCommon.Localization("������ͼ��Ӧ�ڡ����ա�����Ͻ���ʵʱȫ�������Ի�����Ч��", "LIGHTMAPS: Realtime global illumination should be disabled on Lighting Panel for best results.");
            }
            if (!Lightmapping.bakedGI)
            {
                return ExportCommon.Localization("������ͼ��Ӧ�ڡ����ա���������ú決��ȫ�������Ի�����Ч��", "LIGHTMAPS: Baked global illumination should be enabled on Lighting Panel for best results.");
            }

            if (Lightmapping.bakedGI)
            {
                if (GetLightmapBakeMode() != MixedLightingMode.Subtractive)
                {
                    return ExportCommon.Localization("������ͼ������ʹ�ü�������ģʽ����Ӧ�ڡ�������塱��ѡ���ģʽ�Ի�����Ч��", "LIGHTMAPS: Subtractive lighting mode is recommended and should not be selected on Lighting Panel for best results.");
                }
                if (GetLightmapDirectionMode() != LightmapsMode.NonDirectional)
                {
                    //Debug.LogWarning("LIGHTMAPS: Non directional lighting mode should be selected on Lighting Panel for best results.");
                }
                if (GetLightmapCompressionEnabled())
                {
                    return ExportCommon.Localization("������ͼ��Ӧ�ڡ����ա�����Ͻ�������ѹ��ģʽ�Ի�����Ч����", "LIGHTMAPS: Texture compression mode should be disabled on Lighting Panel for best results.");
                }
            }
            return null;
        }
        public static LightmapsMode GetLightmapDirectionMode()
        {
            LightmapsMode result = LightmapsMode.CombinedDirectional;
            LightingSettings lightingSettings = GetLightingSettings();
            if (lightingSettings != null)
            {
                result = lightingSettings.directionalityMode;
            }
            else
            {
                //Debug.LogWarning("LIGHTMAPS: Failed to get lighting editor settings");
            }
            return result;
        }

        public static bool GetLightmapCompressionEnabled()
        {
            bool result = true;
            LightingSettings lightingSettings = GetLightingSettings();
            if (lightingSettings != null)
            {
#if UNITY_2021_1_OR_NEWER
                result = lightingSettings.lightmapCompression != LightmapCompression.None;
#endif
            }
            else
            {
                //Debug.LogWarning("LIGHTMAPS: Failed to get lighting editor settings");
            }
            return result;
        }
        public static LightingSettings GetLightingSettings()
        {
            try
            {
                return Lightmapping.lightingSettings;
            }
            catch (Exception)
            {

                return null;
            }

        }
        public static MixedLightingMode GetLightmapBakeMode()
        {
            MixedLightingMode result = 0;
            LightingSettings lightingSettings = GetLightingSettings();
            if (lightingSettings != null)
            {
                result = lightingSettings.mixedBakeMode;
            }
            else
            {
                //Debug.LogWarning("LIGHTMAPS: Failed to get lighting editor settings");
            }
            return result;
        }
        public void Validate()
        {
            if (Lightmapping.bakedGI)
            {
                LightingSettings setting = GetLightingSettings();
                if (setting != null)
                {
#if UNITY_2021_1_OR_NEWER
                    setting.lightmapCompression = LightmapCompression.None;
#endif
                    setting.mixedBakeMode = MixedLightingMode.Subtractive;
                    setting.directionalityMode = LightmapsMode.NonDirectional;
                }
            }
            Lightmapping.realtimeGI = false;

        }

    }

    public sealed class PlayerSettingsValidator : ISettingsValidator
    {
        public bool IsValid => GetHeaderDescription() == null;
        public string HeaderDescription => GetHeaderDescriptionFull();
        public string CurrentValueDescription => "";
        public string RecommendedValueDescription => IsValid ? ExportCommon.Localization("���޸�", "OK") : ExportCommon.Localization("�޸�", "Fix it");

        public bool CanFix => true;

        string GetHeaderDescriptionFull()
        {
            var result = GetHeaderDescription();
            if (result == null)
            {
                result = ExportCommon.Localization("PlayerSetting�޳�ͻ", "Player Settings is valid");
            }
            return result;
        }
        string GetHeaderDescription()
        {
            if (
                PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Standalone) != ManagedStrippingLevel.Disabled ||
                 PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android) != ManagedStrippingLevel.Disabled ||
                  PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.iOS) != ManagedStrippingLevel.Disabled
            )
            {
                return ExportCommon.Localization("Managed Stripping Level û�йر�", "Managed Stripping Level is not Disabled");
            }
            if (PlayerSettings.assemblyVersionValidation)
            {
                return ExportCommon.Localization("Assembly Version Validation Ӧ�ò���ѡ", "Assembly Version Validation should be false");
            }
            if (!PlayerSettings.allowUnsafeCode)
            {
                return ExportCommon.Localization("Allow Unsafe Code Ӧ�ò���ѡ", "Allow Unsafe Code should be true");
            }
            return null;
        }

        public void Validate()
        {
            PlayerSettings.assemblyVersionValidation = false;
            PlayerSettings.allowUnsafeCode = true;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Standalone, ManagedStrippingLevel.Disabled);
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Disabled);
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS, ManagedStrippingLevel.Disabled);
        }

    }

    public sealed class NormalMapsValidator : ISettingsValidator
    {
        public bool IsValid => GetHeaderDescription() == null;
        public string HeaderDescription => GetHeaderDescription();
        public string CurrentValueDescription => "";
        public string RecommendedValueDescription => IsValid ? ExportCommon.Localization("���޸�", "OK"): ExportCommon.Localization("�޸�", "Fix it");
        public bool CanFix => false;



        string GetHeaderDescription()
        {
            if (
                 PlayerSettings.GetNormalMapEncoding(BuildTargetGroup.Standalone) != NormalMapEncoding.XYZ ||
                  PlayerSettings.GetNormalMapEncoding(BuildTargetGroup.Android) != NormalMapEncoding.XYZ ||
                   PlayerSettings.GetNormalMapEncoding(BuildTargetGroup.iOS) != NormalMapEncoding.XYZ
                )
            {
                return ExportCommon.Localization("Normal Map Encoding ����XYZ", "Normal Map Encoding is not XYZ");
            }
            return null;
        }

        public void Validate()
        {
            PlayerSettings.SetNormalMapEncoding(BuildTargetGroup.Standalone, NormalMapEncoding.XYZ);
            PlayerSettings.SetNormalMapEncoding(BuildTargetGroup.Android, NormalMapEncoding.XYZ);
            PlayerSettings.SetNormalMapEncoding(BuildTargetGroup.iOS, NormalMapEncoding.XYZ);

            if (GetHeaderDescription() != null)
            {
            }
        }

    }
}
