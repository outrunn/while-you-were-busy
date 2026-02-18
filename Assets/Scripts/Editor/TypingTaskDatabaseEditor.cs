using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom inspector for TypingTaskDatabase with helpful buttons and info
/// </summary>
[CustomEditor(typeof(TypingTaskDatabase))]
public class TypingTaskDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TypingTaskDatabase database = (TypingTaskDatabase)target;

        // Title
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Typing Task Database Manager", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "This database stores all typing tasks that can appear in the game. " +
            "Add typing task ScriptableObjects to the list below, or enable runtime creation for testing.",
            MessageType.Info);

        EditorGUILayout.Space();

        // Quick stats
        DrawStatsSection(database);

        EditorGUILayout.Space();

        // Draw default inspector
        DrawDefaultInspector();

        EditorGUILayout.Space();

        // Helpful buttons
        DrawHelpfulButtons();
    }

    private void DrawStatsSection(TypingTaskDatabase database)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Current Status", EditorStyles.boldLabel);

        SerializedProperty taskList = serializedObject.FindProperty("allTypingTasks");
        int assetCount = taskList.arraySize;

        SerializedProperty runtimeCreation = serializedObject.FindProperty("createSampleTasksAtRuntime");
        bool createsRuntime = runtimeCreation.boolValue;

        EditorGUILayout.LabelField($"Asset Tasks: {assetCount}");
        EditorGUILayout.LabelField($"Runtime Tasks: {(createsRuntime ? "5 (will be created on play)" : "0")}");

        int totalTasks = assetCount + (createsRuntime ? 5 : 0);
        EditorGUILayout.LabelField($"Total Available: {totalTasks}", EditorStyles.boldLabel);

        if (totalTasks == 0)
        {
            EditorGUILayout.HelpBox("⚠️ No tasks available! Enable runtime creation or add assets.", MessageType.Warning);
        }
        else if (totalTasks < 5)
        {
            EditorGUILayout.HelpBox("⚠️ Only " + totalTasks + " tasks. Consider adding more for variety.", MessageType.Warning);
        }
        else if (totalTasks >= 10)
        {
            EditorGUILayout.HelpBox("✓ Good variety! " + totalTasks + " tasks available.", MessageType.Info);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawHelpfulButtons()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("Open Typing Task Creator Tool", GUILayout.Height(30)))
        {
            EditorWindow.GetWindow<TypingTaskCreator>("Typing Task Creator");
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Select All Typing Task Assets in Project"))
        {
            SelectAllTypingTaskAssets();
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Selected Assets to List"))
        {
            AddSelectedToList();
        }

        if (GUILayout.Button("Clear List"))
        {
            if (EditorUtility.DisplayDialog("Clear Task List?",
                "This will remove all typing tasks from the list. Are you sure?",
                "Yes, Clear", "Cancel"))
            {
                ClearList();
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void SelectAllTypingTaskAssets()
    {
        string[] guids = AssetDatabase.FindAssets("t:TypingTaskSO");
        Object[] tasks = new Object[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            tasks[i] = AssetDatabase.LoadAssetAtPath<TypingTaskSO>(path);
        }

        Selection.objects = tasks;

        if (tasks.Length > 0)
        {
            Debug.Log($"Selected {tasks.Length} typing task assets");
        }
        else
        {
            Debug.Log("No typing task assets found in project");
        }
    }

    private void AddSelectedToList()
    {
        TypingTaskDatabase database = (TypingTaskDatabase)target;
        SerializedProperty taskList = serializedObject.FindProperty("allTypingTasks");

        int added = 0;
        foreach (Object obj in Selection.objects)
        {
            if (obj is TypingTaskSO task)
            {
                // Check if already in list
                bool alreadyExists = false;
                for (int i = 0; i < taskList.arraySize; i++)
                {
                    if (taskList.GetArrayElementAtIndex(i).objectReferenceValue == task)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (!alreadyExists)
                {
                    taskList.InsertArrayElementAtIndex(taskList.arraySize);
                    taskList.GetArrayElementAtIndex(taskList.arraySize - 1).objectReferenceValue = task;
                    added++;
                }
            }
        }

        serializedObject.ApplyModifiedProperties();

        if (added > 0)
        {
            Debug.Log($"Added {added} typing tasks to the list");
        }
        else
        {
            Debug.Log("No new typing tasks to add (either none selected or already in list)");
        }
    }

    private void ClearList()
    {
        SerializedProperty taskList = serializedObject.FindProperty("allTypingTasks");
        taskList.ClearArray();
        serializedObject.ApplyModifiedProperties();
        Debug.Log("Cleared all typing tasks from list");
    }
}
