#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DualPantoToolkit
{
    [CustomEditor(typeof(SceneManager))]
    public class SceneManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SceneManager sceneManager = (SceneManager)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Scene Navigation", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous Scene"))
            {
                sceneManager.PreviousScene();
            }
            if (GUILayout.Button("Next Scene"))
            {
                sceneManager.NextScene();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Scene navigation buttons only work in Play Mode.", MessageType.Info);
            }
        }
    }
}
#endif
