using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BVA.Component;
using System.Linq;
using System.IO;

#if ENABLE_CRYPTO
using BVA.FileEncryptor;
#endif

namespace BVA
{
    public class ExportAvatarMenu : EditorWindow
    {
        [MenuItem("BVA/Export/Export Avatar")]
        static void Init()
        {
            ExportAvatarMenu window = (ExportAvatarMenu)EditorWindow.GetWindow(typeof(ExportAvatarMenu), false, ExportCommon.Localization("������ɫģ��", "Export Avatar"));
            window.Show();
            window.Root = Selection.activeGameObject;
            GLTFSceneExporter.SetDefaultAvatarExportSetting();
        }
        public GameObject Root;

        ExportInfo exportInfo;
        #region Container Export
        AudioClipContainer audioClipContainer;
        SkyboxContainer skyboxContainer;
        Dictionary<System.Type, Editor> containerEditors;
        static bool _folderAudioEditor, _folderSkyboxEditor;
        private Vector2 scrollPosition;

        bool ExportAudios => _folderAudioEditor && audioClipContainer.audioClips.Count > 0;

        void ShowContainerGUI<T>(T container) where T : MonoBehaviour
        {
            if (container == null) return;
            if (containerEditors == null) containerEditors = new Dictionary<System.Type, Editor>();
            if (containerEditors.TryGetValue(container.GetType(), out Editor editor))
            {
                editor.OnInspectorGUI();
            }
            else
            {
                containerEditors.Add(container.GetType(), Editor.CreateEditor(container));
            }
        }
        #endregion
        string TextFixIt => ExportCommon.Localization("�޸�", "Fix it");
        string TextAdd => ExportCommon.Localization("���", "Add");

        void CheckRootValidity()
        {
            if (Root == null) return;
            #region Animator Human Check
            var animator = Root.GetComponent<Animator>();
            if (animator == null)
            {
                EditorGUILayout.HelpBox(ExportCommon.Localization("�Ҳ���Animator���", "not find Animator component"), MessageType.Error);
                return;
            }
            if (!animator.isHuman)
            {
                EditorGUILayout.HelpBox(ExportCommon.Localization("������Ч��Avatar���壬�Ҳ���������ЧAvatar��Animator���", "Not a valid avatar gameObject, not find Animator component with human avatar"), MessageType.Error);

                if (animator.avatar != null)
                {
                    string avatarPath = AssetDatabase.GetAssetPath(animator.avatar);
                    ModelImporter importer = AssetImporter.GetAtPath(avatarPath) as ModelImporter;
                    if (importer != null && importer.animationType != ModelImporterAnimationType.Human)
                    {
                        if (GUILayout.Button(TextFixIt))
                        {
                            importer.animationType = ModelImporterAnimationType.Human;
                            importer.SaveAndReimport();
                        }
                    }
                }
                else
                {
                    return;
                }

            }
            #endregion

            #region Mesh isReadWrite Check

            var meshs = Root.GetComponentsInChildren<SkinnedMeshRenderer>().Where(x => !x.sharedMesh.isReadable).ToArray();

            if (meshs.Length != 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox(ExportCommon.Localization("ģ�Ͱ�����Ч�����������read/writeӦ�ñ���ѡ", "Model containning invalid mesh, mesh's read/write should be enabled"), MessageType.Error);

                if (GUILayout.Button(ExportCommon.Localization("�޸�", TextFixIt)))
                {
                    for (int i = 0; i < meshs.Length; i++)
                    {
                        string meshPath = AssetDatabase.GetAssetPath(animator.avatar);
                        ModelImporter importer = AssetImporter.GetAtPath(meshPath) as ModelImporter;
                        if (importer.isReadable != true)
                        {
                            importer.isReadable = true;
                            importer.SaveAndReimport();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion

            #region Root Transform Check
            if (Root.transform.localRotation != Quaternion.identity || Root.transform.localPosition != Vector3.zero || Root.transform.localScale != Vector3.one)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox(ExportCommon.Localization("��Ҫ���ø��ڵ�", "Please reset the root transform!"), MessageType.Error);
                if (GUILayout.Button(TextFixIt))
                {
                    Root.transform.localRotation = Quaternion.identity;
                    Root.transform.localPosition = Vector3.zero;
                    Root.transform.localScale = Vector3.one;
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion
            #region MetaInfo Check
            bool isValid = true;
            var meta = Root.GetComponent<BVAMetaInfo>();
            if (meta == null || meta.metaInfo == null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox(ExportCommon.Localization($"����Ҫ���һ��{nameof(BVAMetaInfo)}�Ա㵼����ɫģ�͵ķ�����Ϣ", $"You need assign a {nameof(BVAMetaInfo)} to export avatar for legal use"), MessageType.Error);
                if (GUILayout.Button(TextFixIt))
                {
                    ExportCommon.FixMissingMetaInfo(Root);
                }
                EditorGUILayout.EndHorizontal();
                isValid = false;
            }
            var mixer = Root.GetComponent<BlendShapeMixer>();
            if (mixer == null || mixer.keys == null || mixer.keys.Count == 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox(ExportCommon.Localization($"����Ҫ���һ��{nameof(BlendShapeMixer)}�����֧���沶", $"You need assign a {nameof(BlendShapeMixer)} to export avatar"), MessageType.Error);
                if (GUILayout.Button(TextFixIt))
                {
                    ExportCommon.FixMissingBlendshapeMixer(Root);
                }
                EditorGUILayout.EndHorizontal();
                isValid = false;
            }

            #endregion
            #region Material Check
            ExportCommon.CheckModelShaderIsVaild(Root);
            #endregion
            if (!isValid) return;

            _folderAudioEditor = EditorGUILayout.BeginToggleGroup(ExportCommon.Localization("������Ƶ", " Export AudioClip"), _folderAudioEditor);
            if (_folderAudioEditor)
            {
                audioClipContainer = Root.GetComponent<AudioClipContainer>();
                if (audioClipContainer == null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox(ExportCommon.Localization($"û�з���{nameof(AudioClipContainer)}����Ҫ���һ���µ������", $"Not find {nameof(AudioClipContainer)} on avatar, do you need to assign a new one"), MessageType.Info);
                    if (GUILayout.Button(TextAdd))
                    {
                        audioClipContainer = Root.AddComponent<AudioClipContainer>();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    if (GUILayout.Button(ExportCommon.Localization("���һ��AudioClip��AudioSouce��", "Add AudioClip in AudioSource")))
                    {
                        ExportCommon.AddExistAudioClipToContainer(audioClipContainer, Root);
                    }
                }
                ShowContainerGUI(audioClipContainer);
            }
            EditorGUILayout.EndToggleGroup();

            _folderSkyboxEditor = EditorGUILayout.BeginToggleGroup(ExportCommon.Localization(" ������պ�", " Export Skybox"), _folderSkyboxEditor);
            if (_folderSkyboxEditor)
            {
                skyboxContainer = Root.GetComponent<SkyboxContainer>();
                if (skyboxContainer == null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox(ExportCommon.Localization($"û�з���{nameof(SkyboxContainer)}����Ҫ���һ���µ������", $"Not find {nameof(SkyboxContainer)} on avatar, do you need to assign a new one"), MessageType.Info);
                    if (GUILayout.Button(TextAdd))
                    {
                        skyboxContainer = Root.AddComponent<SkyboxContainer>();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                ShowContainerGUI(skyboxContainer);
            }
            EditorGUILayout.EndToggleGroup();
            ////////////////////////////////////////////////////////////////////////////////
            EditorGUILayout.Separator();
#if ENABLE_CRYPTO
            bool encrypto = EditorConfidential.GUIPassword(out string password);
#endif
            if (GUILayout.Button(ExportCommon.Localization("����", "Export")))
            {
                var exportOptions = new ExportOptions { TexturePathRetriever = AssetDatabase.GetAssetPath, ExportAvatar = true };
                var exporter = new GLTFSceneExporter(new Transform[] { Root.transform }, exportOptions);

                var path = EditorUtility.OpenFolderPanel(ExportCommon.Localization("BVA��ɫ����·��", "BVA Avatar Export Path"), EditorPrefs.GetString("avatar_export_path"), "");
                if (!string.IsNullOrEmpty(path))
                {
                    EditorPrefs.SetString("avatar_export_path", path);
#if ENABLE_CRYPTO
                    if (encrypto)
                        exporter.SaveBVACompressed(Path.Combine(path, Root.name) + $".{BVAConst.EXTENSION_BVA}", Root.name, password);
                    else
                        exporter.SaveGLB(path, Root.name);
#else
                    exporter.SaveGLB(path, Root.name);
#endif
                    EditorUtility.DisplayDialog(ExportCommon.Localization("�����ɹ�", "export success"), ExportCommon.Localization($"����{exporter.ExportDuration.TotalSeconds}����ɵ���", $"spend {exporter.ExportDuration.TotalSeconds}s finish export!"), "OK");
                }
            }
        }
        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            ExportCommon.ShowLanguageSwitchButton();
            EditorGUILayout.LabelField(ExportCommon.Localization("����", "Export"), EditorStyles.boldLabel);

            ExportCommon.EditorGUICheckBuildPlatform();

            EditorGUILayout.BeginHorizontal();
            Root = EditorGUILayout.ObjectField(ExportCommon.Localization("���ڵ�", "Export root"), Root, typeof(GameObject), true) as GameObject;
            if (GUILayout.Button(ExportCommon.Localization("ѡ��", "Select")))
            {
                Root = Selection.activeGameObject;
            }

            if (GUILayout.Button(ExportCommon.Localization("�ռ�������Ϣ", "Collect Export Info")))
            {
                exportInfo = new ExportInfo(Root);
            }
            EditorGUILayout.EndHorizontal();
            //EditorGUILayout.Separator();
            if (exportInfo != null)
                ExportCommon.EditorGUICollectExportInfo(exportInfo);

            GLTFSceneExporter.ExportAnimationClips = EditorGUILayout.Toggle(ExportCommon.Localization("��������", "Export Animations"), GLTFSceneExporter.ExportAnimationClips);
            if (GLTFSceneExporter.ExportAnimationClips)
                EditorGUILayout.HelpBox(ExportCommon.Localization("����Animation����ϱ��Ϊlegacy�Ķ�������ת��Animator����ϵĶ���Ϊ������Ƥ��������", $"Export legacy animation on Animation, as well convert mecanim animation to legacy skin animation and export it"), MessageType.Info);

            EditorGUILayout.Separator();
            CheckRootValidity();
            EditorGUILayout.Separator();
            ExportCommon.EditorGUIExportLog();
            EditorGUILayout.HelpBox(ExportCommon.Localization($"BVA �汾{BVAConst.FORMAT_VERSION}", $"BVA version {BVAConst.FORMAT_VERSION}"), MessageType.Info);

            EditorGUILayout.EndScrollView();
        }

    }
}