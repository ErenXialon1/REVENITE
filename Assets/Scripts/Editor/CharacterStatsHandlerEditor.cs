using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterStatsHandler))]
public class CharacterStatsHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CharacterStatsHandler handler = (CharacterStatsHandler)target;
        var stats = handler.Stats;

        if (stats == null)
        {
            EditorGUILayout.HelpBox("Stats not initialized yet. Run the game to see runtime stats.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Stats", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Current Health", stats.CurrentHealth.ToString("F1"));
        EditorGUILayout.LabelField("Max Health", stats.MaxHealth.ToString("F1"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("All Stats", EditorStyles.boldLabel);

        foreach (var statPair in stats.GetAllStats())
        {
            string name = statPair.Key;
            float value = statPair.Value.CalculateFinalValue();
            EditorGUILayout.LabelField(name, value.ToString("F1"));
        }
    }
}
