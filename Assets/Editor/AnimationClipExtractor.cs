// AnimationClipExtractor.cs

using UnityEngine;
using UnityEditor;
using System.IO; // 파일 및 디렉토리 경로를 다루기 위해 필요합니다.

public class AnimationClipExtractor : EditorWindow
{
    private string sourceFolderPath = "Assets/"; // FBX가 있는 소스 폴더 경로
    private string destinationFolderPath = "Assets/"; // 애니메이션을 저장할 폴더 경로

    [MenuItem("My Tools/Animation Clip Extractor")]
    public static void ShowWindow()
    {
        GetWindow<AnimationClipExtractor>("애니메이션 클립 추출기");
    }

    private void OnGUI()
    {
        GUILayout.Label("FBX 애니메이션 클립 추출기", EditorStyles.boldLabel);

        // --- 소스 폴더 선택 UI ---
        EditorGUILayout.LabelField("1. FBX가 있는 폴더를 선택하세요:");
        EditorGUILayout.BeginHorizontal();
        // 텍스트 필드는 경로를 보여주는 역할만 합니다. 직접 수정은 비활성화(disabled)
        EditorGUILayout.TextField(sourceFolderPath, GUI.skin.textField);
        if (GUILayout.Button("선택", GUILayout.Width(60)))
        {
            // 사용자가 폴더를 선택할 수 있는 패널을 엽니다.
            string path = EditorUtility.OpenFolderPanel("소스 폴더 선택", "Assets", "");
            // 사용자가 폴더를 선택했다면 경로를 업데이트합니다.
            if (!string.IsNullOrEmpty(path))
            {
                // 전체 경로(C:/...)를 Assets/로 시작하는 상대 경로로 변경해야 합니다.
                sourceFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();

        // --- 저장 폴더 선택 UI ---
        EditorGUILayout.LabelField("2. .anim 파일을 저장할 폴더를 선택하세요:");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField(destinationFolderPath, GUI.skin.textField);
        if (GUILayout.Button("선택", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("저장 폴더 선택", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                destinationFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(20); // UI에 여백 추가

        // --- 추출 시작 버튼 ---
        if (GUILayout.Button("3. 애니메이션 클립 추출 시작"))
        {
            ExtractAnimationClips();
        }
    }

    private void ExtractAnimationClips()
    {
        // 폴더 경로가 유효한지 확인합니다.
        if (string.IsNullOrEmpty(sourceFolderPath) || string.IsNullOrEmpty(destinationFolderPath))
        {
            EditorUtility.DisplayDialog("경고", "소스 폴더와 저장 폴더를 모두 지정해야 합니다.", "확인");
            return;
        }

        // 소스 폴더에서 모든 FBX 파일의 GUID(고유 ID)를 찾습니다. "t:Model"은 모델 에셋을 의미합니다.
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { sourceFolderPath });

        int extractedCount = 0;
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            // FBX 파일 안에 있는 모든 에셋(메시, 머티리얼, 애니메이션 등)을 불러옵니다.
            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            foreach (Object asset in allAssets)
            {
                // 불러온 에셋이 AnimationClip 타입인지 확인합니다.
                if (asset is AnimationClip)
                {
                    AnimationClip originalClip = asset as AnimationClip;

                    // Unity가 내부적으로 생성하는 __preview__ 클립은 제외합니다.
                    if (originalClip.name.StartsWith("__preview__")) continue;

                    // 애니메이션 클립을 복제하여 새로운 인스턴스를 만듭니다.
                    AnimationClip newClip = Object.Instantiate(originalClip);

                    // 저장할 경로와 파일명을 조합합니다. (예: Assets/Animations/MyModel.anim)
                    string fbxName = Path.GetFileNameWithoutExtension(assetPath);
                    string newPath = Path.Combine(destinationFolderPath, fbxName + ".anim");

                    // 에셋 데이터베이스에 새로운 애니메이션 클립 에셋을 생성(저장)합니다.
                    AssetDatabase.CreateAsset(newClip, newPath);

                    extractedCount++;
                    break; // FBX 하나당 하나의 클립만 추출한다고 가정하고 루프를 빠져나갑니다.
                           // 만약 FBX 안에 클립이 여러 개이고 모두 추출하고 싶다면 이 'break'를 지우세요.
                }
            }
        }

        // 작업 완료 후 에셋 데이터베이스를 새로고침합니다.
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 사용자에게 작업 완료를 알립니다.
        EditorUtility.DisplayDialog("완료", $"{extractedCount}개의 애니메이션 클립을 성공적으로 추출했습니다.", "확인");
    }
}