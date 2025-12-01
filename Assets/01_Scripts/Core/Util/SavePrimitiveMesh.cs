//using UnityEngine;
//using UnityEditor;

//public class SavePrimitiveMesh : EditorWindow
//{
//    PrimitiveType selectedType = PrimitiveType.Sphere; // 기본값

//    [MenuItem("Tools/Save Primitive Mesh")]
//    public static void ShowWindow()
//    {
//        GetWindow<SavePrimitiveMesh>("Save Primitive Mesh");
//    }
//    void OnGUI()
//    {
//        EditorGUILayout.LabelField("Select Primitive Type", EditorStyles.boldLabel);
//        selectedType = (PrimitiveType)EditorGUILayout.EnumPopup("Primitive", selectedType);

//        if (GUILayout.Button("Save Mesh"))
//        {
//            SaveSelectedPrimitiveMesh(selectedType);
//        }
//    }

//    void SaveSelectedPrimitiveMesh(PrimitiveType type)
//    {
//        // 임시 GameObject 생성
//        GameObject temp = GameObject.CreatePrimitive(type);

//        Mesh mesh = temp.GetComponent<MeshFilter>().sharedMesh;

//        // 저장 경로 지정
//        string path = $"Assets/{type}Mesh.asset";

//        // 메쉬 복사 후 저장
//        Mesh meshCopy = Object.Instantiate(mesh);
//        AssetDatabase.CreateAsset(meshCopy, path);
//        AssetDatabase.SaveAssets();

//        GameObject.DestroyImmediate(temp);

//        Debug.Log($"Saved {type} Mesh to {path}");
//    }
//}
