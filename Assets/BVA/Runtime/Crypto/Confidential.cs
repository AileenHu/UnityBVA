#define ENABLE_CRYPTO
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BVA.FileEncryptor
{
    public static class Confidential
    {
        public const string DEFAULT_PASSWORD = "bilibili's@hope";
    }

#if UNITY_EDITOR
    public static class EditorConfidential
    {
        static bool enableCrypto;
        static string password = Confidential.DEFAULT_PASSWORD;

        public static bool GUIPassword(out string _password)
        {
            enableCrypto = EditorGUILayout.Toggle("ʹ�ü���",enableCrypto);
            if (enableCrypto)
            {
                EditorGUILayout.BeginHorizontal();
                password = EditorGUILayout.TextField("����", password);
                if (GUILayout.Button("Ĭ������"))
                    password = Confidential.DEFAULT_PASSWORD;
                EditorGUILayout.EndHorizontal();
            }
            _password = password;
            return enableCrypto;
        }
    }
#endif
}