using UnityEngine;

public class Altar : MonoBehaviour
{
    // 修改点：Header 必须放在普通字段上方，不能放在属性 { get; set; } 上方
    [Header("状态配置")]
    public bool isActivated = false;

    [Header("功能设置")]
    // 是否计入通关总数（镜像祭坛设为 false，贪婪祭坛设为 true）
    public bool countsTowardsProgress = true;

    protected SpriteRenderer sr;

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // 定义虚方法，由子类实现具体的献祭逻辑
    public virtual bool TrySacrifice(IMaskPower mask, PlayerController player)
    {
        if (isActivated) return false;

        Activate();
        return true;
    }

    protected virtual void Activate()
    {
        isActivated = true;
        // 视觉反馈：激活后变色
        if (sr != null) sr.color = Color.green;
        Debug.Log($"{gameObject.name} 已激活！");
    }
}