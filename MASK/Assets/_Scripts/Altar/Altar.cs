using UnityEngine;

public class Altar : MonoBehaviour
{
    [Header("状态配置")]
    public bool isActivated = false;
    public bool countsTowardsProgress = true;

    [Header("视觉配置")]
    [Tooltip("激活后的新图标")]
    public Sprite activatedSprite;

    protected SpriteRenderer sr;
    private Sprite originalSprite; // 记录初始图标

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalSprite = sr.sprite;
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

        if (sr != null && activatedSprite != null)
        {
            sr.sprite = activatedSprite;
            sr.color = Color.white; 
        }

        Debug.Log($"{gameObject.name} 祭坛已点亮！");
    }
}