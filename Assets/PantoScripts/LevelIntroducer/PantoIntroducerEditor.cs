using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PantoIntroducer))]
public class PantoIntroducerEditor : Editor
{
    float heightToFlattenTo = 0.2f;

    public override void OnInspectorGUI()
    {
        PantoIntroducer targetIntroducer = (PantoIntroducer)target;
        DrawDefaultInspector();
        if (!targetIntroducer.useActiveChildObjects && targetIntroducer.introductions.Count == 0)
            EditorGUILayout.HelpBox("Warning: Either tick above box or specify introductions.", MessageType.Error);


        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Path convenience", EditorStyles.boldLabel);
        heightToFlattenTo = EditorGUILayout.FloatField("height to flatten to", heightToFlattenTo);
        if (GUILayout.Button("Flatten tracing paths and back-and-forths"))
        {
            foreach (LineRenderer lr in targetIntroducer.GetComponentsInChildren<LineRenderer>())
            {
                float localHeight = heightToFlattenTo;
                if (!lr.useWorldSpace)
                {
                    localHeight = lr.transform.InverseTransformPoint(new Vector3(0, heightToFlattenTo, 0)).y;
                }
                for (int i = 0; i < lr.positionCount; i++)
                {
                    lr.SetPosition(i, new Vector3(lr.GetPosition(i).x, localHeight, lr.GetPosition(i).z));
                }
            }
        }


        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Visualizations in Editor", EditorStyles.boldLabel);
        if (GUILayout.Button("Hide all intro visualizations"))
        {
            targetIntroducer.HideAllIntroVisuals();
        }
        if (GUILayout.Button("Show all intro visualizations"))
        {
            targetIntroducer.ShowAllIntroVisuals();
        }
        EditorGUILayout.LabelField("While in-game, visualizations are only there during intros.");
    }
}
