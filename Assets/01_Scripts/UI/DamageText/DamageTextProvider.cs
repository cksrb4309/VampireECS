using UnityEngine;
using DamageTextRequest = DamageTextVfxBatchEmitter.DamageTextRequest;
using TextEmitParams = DamageTextVfxBatchEmitter.TextEmitParams;

public static class DamageTextProvider
{
    private static TextEmitParams defaultParams = new TextEmitParams(
        position: Vector3.zero,
        lifetime: 0.5f,
        fontSize: 1.0f,
        fontColor: Color.white,
        outlineColor: Color.black
    );

    public static void ShowDamageText(Vector3 position, int damage, Color color)
    {
        var emitter = DamageTextVfxBatchEmitter.Instance;

        if (emitter == null)
        {
            Debug.LogWarning("DamageTextVfxBatchEmitter Instance is null. Make sure it exists in the scene.");
            return;
        }

        // 데미지 텍스트 정보 생성 (관심사 분리: 데이터 구성)
        var emitParams = defaultParams;
        emitParams.Position = position;
        //emitParams.FontColor = color;
        // OutlineColor는 기본적으로 FontColor를 따라가도록 설정되어 있음 (TextEmitParams 생성자 참고)
        //emitParams.OutlineColor = color; 

        // VFX 실행 요청
        emitter.EnqueueText(damage, emitParams);
    }
}
