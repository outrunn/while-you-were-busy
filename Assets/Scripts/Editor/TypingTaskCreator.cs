using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor utility to quickly create typing task ScriptableObjects
/// Adds menu items and helpful shortcuts
/// </summary>
public class TypingTaskCreator : EditorWindow
{
    private string taskTitle = "New Typing Task";
    private string taskDescription = "Complete this typing task";
    private string messageToType = "Enter your message here...";
    private float completionDelay = 2.5f;
    private int minimumKeyPresses = 0;

    private string savePath = "Assets/ScriptableObjects/TypingTasks/";

    [MenuItem("Tools/Typing Task Creator")]
    public static void ShowWindow()
    {
        GetWindow<TypingTaskCreator>("Typing Task Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create New Typing Task", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Task settings
        taskTitle = EditorGUILayout.TextField("Task Title:", taskTitle);
        taskDescription = EditorGUILayout.TextField("Description:", taskDescription);

        EditorGUILayout.Space();
        GUILayout.Label("Message Content:", EditorStyles.boldLabel);
        messageToType = EditorGUILayout.TextArea(messageToType, GUILayout.Height(100));

        EditorGUILayout.Space();
        minimumKeyPresses = EditorGUILayout.IntField("Min Key Presses (0 = auto):", minimumKeyPresses);
        completionDelay = EditorGUILayout.FloatField("Completion Delay:", completionDelay);

        EditorGUILayout.Space();
        savePath = EditorGUILayout.TextField("Save Path:", savePath);

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Typing Task", GUILayout.Height(30)))
        {
            CreateTypingTask();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Sample Easy Tasks (5)", GUILayout.Height(25)))
        {
            CreateSampleEasyTasks();
        }

        if (GUILayout.Button("Create Sample Medium Tasks (5)", GUILayout.Height(25)))
        {
            CreateSampleMediumTasks();
        }

        if (GUILayout.Button("Create Sample Hard Tasks (5)", GUILayout.Height(25)))
        {
            CreateSampleHardTasks();
        }
    }

    private void CreateTypingTask()
    {
        // Ensure directory exists
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        }

        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/TypingTasks"))
        {
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "TypingTasks");
        }

        // Create the ScriptableObject
        TypingTaskSO task = ScriptableObject.CreateInstance<TypingTaskSO>();
        task.taskTitle = taskTitle;
        task.taskDescription = taskDescription;
        task.messageToType = messageToType;
        task.minimumKeyPresses = minimumKeyPresses;
        task.completionDelay = completionDelay;

        // Generate filename from title
        string fileName = taskTitle.Replace(" ", "_") + ".asset";
        string fullPath = savePath + fileName;

        // Save the asset
        AssetDatabase.CreateAsset(task, fullPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Select it
        EditorGUIUtility.PingObject(task);
        Selection.activeObject = task;

        Debug.Log($"Created typing task: {fullPath}");
    }

    private void CreateSampleEasyTasks()
    {
        CreateAndSaveTask("Quick Email", "Send brief update",
            "Subject: Status Update\n\nAll systems operational. No issues to report.\n\nBest regards", 2.5f);

        CreateAndSaveTask("Meeting Confirmation", "Confirm attendance",
            "Meeting confirmed for 2 PM. Will attend via video call.", 2.5f);

        CreateAndSaveTask("Daily Log", "Record daily activities",
            "Daily Log Entry:\n- Processed 47 tickets\n- Completed routine maintenance\n- No anomalies detected", 2.5f);

        CreateAndSaveTask("Backup Notification", "Confirm backup completion",
            "BACKUP COMPLETE\n\nAll data successfully archived to secure storage. Next backup scheduled for tomorrow at 2 AM.", 2.5f);

        CreateAndSaveTask("Simple Memo", "Write internal memo",
            "MEMO: Please be advised that system maintenance is scheduled for this weekend. Expect minor service interruptions.", 2.5f);

        Debug.Log("Created 5 sample EASY typing tasks!");
    }

    private void CreateSampleMediumTasks()
    {
        CreateAndSaveTask("Security Update", "Document security changes",
            "SECURITY PROTOCOL UPDATE v3.7\n\nNew authentication requirements now in effect. All users must implement two-factor verification within 48 hours. Non-compliance will result in automatic account suspension. Contact IT support for assistance.", 2.5f);

        CreateAndSaveTask("Meeting Minutes", "Record meeting outcomes",
            "MEETING MINUTES - System Status Review\n\nAttendees: All departments\nDate: Today\n\nKey Points:\n- Q3 performance exceeded targets by 12%\n- Resource allocation approved for next quarter\n- New security protocols to be implemented\n\nAction Items:\n1. Update documentation by Friday\n2. Schedule training session\n3. Review budget allocations", 2.5f);

        CreateAndSaveTask("Stakeholder Email", "Update external partners",
            "Dear Stakeholders,\n\nI am writing to inform you of significant progress in our operational efficiency. Recent optimization efforts have resulted in measurable improvements across all key performance indicators. Detailed reports will be distributed by end of week.\n\nPlease contact me with any questions or concerns.\n\nBest regards,\nOperations Team", 2.5f);

        CreateAndSaveTask("Technical Documentation", "Document system changes",
            "TECHNICAL DOCUMENTATION UPDATE\n\nSystem: Data Processing Pipeline v2.4\n\nChanges:\n- Improved throughput by 23%\n- Reduced latency to <100ms\n- Enhanced error handling protocols\n- Added real-time monitoring dashboard\n\nDeployment: Completed successfully on [DATE]\nImpact: All systems nominal, performance within expected parameters", 2.5f);

        CreateAndSaveTask("Performance Report", "Draft quarterly analysis",
            "Q3 PERFORMANCE SUMMARY\n\nKey Metrics:\n- Total outputs: +34% vs. Q2\n- Efficiency rating: 94.2%\n- Error rate: 0.3% (within tolerance)\n- User satisfaction: Stable\n\nRecommendations:\n- Continue current optimization strategies\n- Monitor resource allocation closely\n- Plan for scaled operations in Q4", 2.5f);

        Debug.Log("Created 5 sample MEDIUM typing tasks!");
    }

    private void CreateSampleHardTasks()
    {
        CreateAndSaveTask("Incident Report", "File detailed analysis",
            "INCIDENT REPORT #847\n\nSUMMARY: An unexpected surge in processing requests was detected at 14:32:47 on [DATE]. Root cause analysis indicates user-initiated activity exceeded normal operational parameters by 347%.\n\nIMPACT: System responded appropriately and maintained stability throughout the event. No data loss occurred. Performance degradation minimal (2.3% efficiency reduction during peak load).\n\nROOT CAUSE: Automated task scheduler initiated multiple concurrent processes due to configuration error in timing logic.\n\nRESOLUTION: Configuration corrected, additional safeguards implemented to prevent recurrence.\n\nRECOMMENDATIONS: Increase buffer capacity by 15%, implement predictive load balancing, schedule comprehensive system audit.\n\nSTATUS: RESOLVED", 3.0f);

        CreateAndSaveTask("System Architecture", "Draft technical documentation",
            "SYSTEM ARCHITECTURE DOCUMENTATION v4.2\n\nOVERVIEW: The current system employs a distributed processing architecture optimized for high-throughput task management with real-time health monitoring and adaptive resource allocation.\n\nCORE COMPONENTS:\n1. Task Generation Engine - Produces work tickets based on difficulty scaling algorithms\n2. Processing Pipeline - Handles task completion, validation, and reward distribution\n3. Health Management System - Monitors and reports on world health metrics\n4. Day/Night Cycle Controller - Manages temporal progression and quota enforcement\n\nDATA FLOW: Tasks flow from generation through user interaction, minigame completion, validation, processing, and final reward allocation. Each stage includes error handling and logging.\n\nPERFORMANCE CHARACTERISTICS: Average throughput 50-200 tasks/day depending on difficulty. System scales automatically based on quota requirements.", 3.0f);

        CreateAndSaveTask("Executive Summary", "Prepare leadership briefing",
            "EXECUTIVE SUMMARY - QUARTERLY OPERATIONS REVIEW\n\nPERFORMANCE OVERVIEW:\nQ3 operations exceeded projected targets across all key performance indicators. Total output increased 34% year-over-year while maintaining operational stability. System health metrics show expected degradation patterns consistent with growth trajectories.\n\nKEY ACHIEVEMENTS:\n- Implemented automated ticket processing system\n- Reduced average task completion time by 17%\n- Exceeded daily quota targets on 87% of operational days\n- Maintained 99.7% system uptime\n\nCHALLENGES:\n- World health degradation accelerating faster than projected\n- Resource sustainability concerns raised by external auditors\n- Long-term operational viability requires strategic intervention\n- Scaling requirements approaching infrastructure limits\n\nRECOMMENDATIONS:\n- Immediate investment in health stabilization protocols\n- Comprehensive sustainability audit\n- Strategic planning for next fiscal year\n- Consideration of alternative operational models\n\nOVERALL RATING: Acceptable within current parameters, strategic action required for long-term viability.", 3.5f);

        CreateAndSaveTask("Compliance Audit Response", "Respond to auditor findings",
            "COMPLIANCE AUDIT RESPONSE - Reference #2024-Q3-AUD\n\nDear Audit Committee,\n\nThank you for your comprehensive review of our operational procedures. We acknowledge the concerns raised regarding sustainability metrics and long-term viability projections.\n\nRESPONSE TO FINDINGS:\n\n1. World Health Degradation (Finding #3.2): We acknowledge that current operational tempo results in measurable environmental impact. Proposed mitigation: Implementation of efficiency protocols and exploration of alternative task processing methodologies.\n\n2. Resource Allocation (Finding #4.1): Current resource consumption rates are within designed parameters but approaching critical thresholds. Proposed action: Comprehensive resource optimization study and potential quota restructuring.\n\n3. Sustainability Planning (Finding #5.3): Long-term strategic planning will be prioritized in Q4. External consultants will be engaged to develop sustainable operational models.\n\nWe appreciate the opportunity to address these important concerns and commit to implementing recommended improvements within specified timeframes.\n\nRespectfully submitted,\nOperations Management Team", 3.5f);

        CreateAndSaveTask("Crisis Communication", "Draft urgent stakeholder message",
            "URGENT STAKEHOLDER COMMUNICATION\n\nSubject: Critical System Status Update - Immediate Action Required\n\nDear All,\n\nI am writing to inform you of a critical situation requiring immediate attention and coordinated response.\n\nSITUATION: World health metrics have declined to concerning levels due to accelerated operational demands. While system functionality remains intact, sustainability projections indicate approaching critical thresholds.\n\nIMMEDIATE ACTIONS TAKEN:\n- Emergency optimization protocols activated\n- Non-essential operations temporarily scaled back\n- Enhanced monitoring systems deployed\n- Crisis management team convened\n\nREQUIRED NEXT STEPS:\n- All departments reduce non-critical resource consumption\n- Accelerate efficiency improvement initiatives\n- Prepare contingency plans for potential service modifications\n- Daily status briefings until situation stabilizes\n\nTIMELINE: Situation assessment ongoing. Full status report will be provided within 48 hours. Emergency protocols will remain in effect until further notice.\n\nThis situation requires our collective attention and commitment to sustainable operations. Your cooperation and swift action are essential.\n\nRegards,\nExecutive Leadership", 3.5f);

        Debug.Log("Created 5 sample HARD typing tasks!");
    }

    private void CreateAndSaveTask(string title, string description, string message, float delay)
    {
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        }

        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/TypingTasks"))
        {
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "TypingTasks");
        }

        TypingTaskSO task = ScriptableObject.CreateInstance<TypingTaskSO>();
        task.taskTitle = title;
        task.taskDescription = description;
        task.messageToType = message;
        task.minimumKeyPresses = 0;
        task.completionDelay = delay;

        string fileName = title.Replace(" ", "_") + ".asset";
        string fullPath = savePath + fileName;

        AssetDatabase.CreateAsset(task, fullPath);
    }
}
