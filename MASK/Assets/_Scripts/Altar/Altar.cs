using UnityEngine;

public class Altar : MonoBehaviour
{
    [Header("状态配置")]
    public bool isActivated = false;
    public bool countsTowardsProgress = true;

    [Header("视觉配置")]
    public Sprite activatedSprite;
    public Color activatedColor = Color.green; // 激活后的目标颜色

    protected SpriteRenderer sr;
    protected Sprite originalSprite;
    protected Color originalColor; 

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalSprite = sr.sprite;
            originalColor = sr.color; 
        }
    }

    public virtual int GetState() => isActivated ? 1 : 0;

    public virtual void SetStateFromUndo(int state)
    {
        isActivated = (state > 0);
        if (sr != null)
        {
            sr.sprite = isActivated ? activatedSprite : originalSprite;
            sr.color = isActivated ? activatedColor : originalColor; // 恢复初始色
        }
    }

    public virtual bool TrySacrifice(IMaskPower mask, PlayerController player)
    {
        if (isActivated) return false;
        Activate();
        return true;
    }

    protected virtual void Activate()
    {
        isActivated = true;
        if (sr != null)
        {
            if (activatedSprite != null) sr.sprite = activatedSprite;
            sr.color = activatedColor;
        }
    }
}