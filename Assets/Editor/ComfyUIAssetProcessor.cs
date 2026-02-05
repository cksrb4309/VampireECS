using UnityEditor;
using UnityEngine;

/// <summary>
/// ComfyUI 브릿지를 통해 생성된 아이콘 에셋의 임포트 설정을 자동으로 최적화합니다.
/// </summary>
public class ComfyUIAssetProcessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        // 파일 경로에 ComfyUI 아이콘 경로가 포함되어 있는지 확인
        // (Python 브릿지에서 지정한 UNITY_ASSET_PATH: Assets/02_Art/Sprites/Icons)
        if (assetPath.Contains("02_Art/Sprites/Icons"))
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;

            // 이미 설정이 되어있다면 중복 실행 방지
            if (textureImporter.textureType == TextureImporterType.Sprite &&
                textureImporter.alphaSource == TextureImporterAlphaSource.FromGrayScale)
            {
                return;
            }

            // 스프라이트로 설정
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Single;

            // 사용자의 요청대로 Alpha를 그레이스케일에서 가져오도록 설정
            textureImporter.alphaSource = TextureImporterAlphaSource.FromGrayScale;
            textureImporter.alphaIsTransparency = true;

            // UI에서 색상 변경이 용이하도록 sRGB 및 미압축 고려 (필요 시)
            textureImporter.mipmapEnabled = false;

            Debug.Log($"[ComfyUI] {assetPath}의 임포트 설정을 자동으로 최적화했습니다 (Alpha from GrayScale).");
        }
    }
}
