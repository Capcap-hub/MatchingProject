using UnityEngine;

public class AlignParticleToUIButton : MonoBehaviour
{
    public RectTransform uiButton; // 按钮的 RectTransform
    public ParticleSystem particleSystem; // 粒子系统

    void Update()
    {
        if (uiButton != null && particleSystem != null)
        {
            // 获取按钮的宽度和高度
            float centerX = uiButton.rect.width / 2f;
            float centerY = -uiButton.rect.height / 2f; // 负数，因为 Pivot 是 (0, 1)
            
            // 计算粒子系统的位置
            Vector3 localPosition = new Vector3(centerX, centerY, 0f);
            particleSystem.transform.localPosition = localPosition;
        }
    }

}