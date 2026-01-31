using UnityEngine;

public class GreedyAltar : Altar
{
    [Header("贪婪设置")]
    public int requiredCount = 2;
    private int currentCount = 0;

    public override bool TrySacrifice(IMaskPower mask, PlayerController player)
    {
        if (isActivated) return false;

        currentCount++;
        Debug.Log($"贪婪祭坛接收面具 ({currentCount}/{requiredCount})");

        if (currentCount >= requiredCount)
        {
            Activate();
        }
        else
        {
            // 投入一个后的过渡颜色
            if (sr != null) sr.color = new Color(1, 0.5f, 0);
        }

        return true;
    }
}