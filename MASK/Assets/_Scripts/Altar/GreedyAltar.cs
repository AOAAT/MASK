using UnityEngine;

public class GreedyAltar : Altar
{
    [Header("贪婪配置")]
    public int requiredCount = 2;
    private int currentCount = 0;

    [Tooltip("投入第一个面具后的中间状态图标（可选）")]
    public Sprite intermediateSprite;

    public override bool TrySacrifice(IMaskPower mask, PlayerController player)
    {
        if (isActivated) return false;

        currentCount++;

        if (currentCount >= requiredCount)
        {
            Activate(); // 调用基类 Activate，切换到最终的 activatedSprite
        }
        else
        {
            // 切换到中间状态图标（如果有的话）
            if (sr != null && intermediateSprite != null)
            {
                sr.sprite = intermediateSprite;
            }
            else if (sr != null)
            {
                // 如果没有中间素材，就用半透明绿色提示进度
                sr.color = new Color(0.5f, 1f, 0.5f, 0.8f);
            }
            Debug.Log($"贪婪祭坛已接收面具 ({currentCount}/{requiredCount})");
        }

        return true;
    }
}