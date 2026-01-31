using UnityEngine;

public class GreedyAltar : Altar
{
    [Header("贪婪配置")]
    public int requiredCount = 2;
    private int currentCount = 0;

    [Header("贪婪视觉")]
    public Color intermediateColor = Color.yellow; 
    public Sprite intermediateSprite;

    public override int GetState() => currentCount;

    public override void SetStateFromUndo(int state)
    {
        currentCount = state;
        isActivated = (currentCount >= requiredCount);

        if (sr != null)
        {
            if (isActivated)
            {
                sr.sprite = activatedSprite;
                sr.color = activatedColor;
            }
            else if (currentCount > 0)
            {
             
                if (intermediateSprite != null) sr.sprite = intermediateSprite;
                sr.color = intermediateColor;
            }
            else
            {
                
                sr.sprite = originalSprite;
                sr.color = originalColor;
            }
        }
    }

    public override bool TrySacrifice(IMaskPower mask, PlayerController player)
    {
        if (isActivated) return false;
        currentCount++;

        if (currentCount >= requiredCount)
        {
            Activate();
        }
        else
        {
            
            if (sr != null)
            {
                sr.color = intermediateColor;
                if (intermediateSprite != null) sr.sprite = intermediateSprite;
            }
            Debug.Log($"贪婪祭坛吃到第 {currentCount} 个面具，产生色变！");
        }
        return true;
    }
}