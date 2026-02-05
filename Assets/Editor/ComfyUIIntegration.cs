using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;

public class ComfyUIIntegration : EditorWindow
{
    private string positivePrompt = "Sleek modern sword icon, minimalist, white on black background";
    private string fileName = "NewEquipmentIcon";

    [MenuItem("Tools/ComfyUI/Icon Generator")]
    public static void ShowWindow()
    {
        GetWindow<ComfyUIIntegration>("ComfyUI Icon Gen");
    }

    private void OnGUI()
    {
        GUILayout.Label("ComfyUI - Unity Integration", EditorStyles.boldLabel);
        
        positivePrompt = EditorGUILayout.TextField("Prompt", positivePrompt);
        fileName = EditorGUILayout.TextField("File Name", fileName);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Check Server"))
        {
            RunBridge("--check", "");
        }

        if (GUILayout.Button("Generate and Import"))
        {
            RunBridge(positivePrompt, fileName);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void RunBridge(string arg1, string arg2)
    {
        string pythonPath = "python"; // 시스템 환경변수에 python이 등록되어 있어야 합니다.
        string scriptPath = Path.Combine(Application.dataPath, "Editor/ComfyUnityBridge.py");
        
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = pythonPath;
        
        if (arg1 == "--check")
        {
            start.Arguments = string.Format("\"{0}\" --check", scriptPath);
        }
        else
        {
            // 인자들을 큰따옴표로 감싸서 공백이 포함된 프롬프트도 처리할 수 있게 함
            start.Arguments = string.Format("\"{0}\" \"{1}\" \"{2}\"", scriptPath, arg1, arg2);
        }

        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.CreateNoWindow = true;

        UnityEngine.Debug.Log("[ComfyUI] Execution started...");

        using (Process process = Process.Start(start))
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
                UnityEngine.Debug.Log("[ComfyUI Output] " + output);
            
            if (!string.IsNullOrEmpty(error))
                UnityEngine.Debug.LogError("[ComfyUI Error] " + error);
        }

        if (arg1 != "--check")
        {
            AssetDatabase.Refresh();
            UnityEngine.Debug.Log("Asset Database Refreshed.");
        }
    }
}
