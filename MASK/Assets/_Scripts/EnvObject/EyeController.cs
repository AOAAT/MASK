using UnityEngine;
using System.Linq;

public class EyeController : MonoBehaviour
{
    // 定义眼睛的类型，对应四个移动方向
    public enum EyeType { Up, Down, Left, Right }

    [Header("类型设置")]
    public EyeType eyeType;

    [Header("视觉组件引用")]
    public Transform pupil;          // 眼珠子
    public SpriteRenderer eyeMainSR; // 眼眶

    [Header("素材替换")]
    public Sprite openEyeSprite;     // 睁眼状态贴图
    public Sprite closedEyeSprite;   // 闭眼状态贴图

    [Header("追踪配置")]
    public float maxOffset = 0.25f;  // 眼珠在眼眶内的最大偏移
    public float followSpeed = 5f;   // 眼珠注视玩家的平滑速度

    private PlayerController playerCtrl;
    private Transform playerTransform;
    private bool isClosed = false;
    private Vector2 targetDirection;

    void Start()
    {
        if (eyeMainSR == null) eyeMainSR = GetComponent<SpriteRenderer>();

        // 初始化对应的方向向量
        InitDirection();

        // 查找玩家引用
        playerCtrl = FindObjectOfType<PlayerController>();
        if (playerCtrl != null) playerTransform = playerCtrl.transform;

     
        UpdateVisualStatus(true);
    }

    void Update()
    {
        // 1. 只有睁眼时才执行追踪注视逻辑
        if (!isClosed && playerTransform != null && pupil != null)
        {
            Vector3 lookDir = (playerTransform.position - transform.position).normalized;
            // 限制眼珠不超出眼眶范围
            pupil.localPosition = Vector3.Lerp(pupil.localPosition, lookDir * maxOffset, Time.deltaTime * followSpeed);
        }

        // 2. 根据玩家能力状态实时切换视觉表现
        UpdateVisualStatus(false);
    }

    private void InitDirection()
    {
        switch (eyeType)
        {
            case EyeType.Up: targetDirection = Vector2.up; break;
            case EyeType.Down: targetDirection = Vector2.down; break;
            case EyeType.Left: targetDirection = Vector2.left; break;
            case EyeType.Right: targetDirection = Vector2.right; break;
        }
    }

    private void UpdateVisualStatus(bool forceUpdate)
    {
        if (playerCtrl == null) return;

        // 检查玩家当前持有的面具列表中是否有对应方向的能力
        bool hasPower = playerCtrl.GetOwnedMasks().Any(m => m.Direction == targetDirection);

        if (hasPower && (!isClosed || forceUpdate))
        {
            SetEyeState(true);
        }
        else if (!hasPower && (isClosed || forceUpdate))
        {
            SetEyeState(false);
        }
    }

    private void SetEyeState(bool shouldClose)
    {
        isClosed = shouldClose;

        // 1. 处理眼珠：闭眼时隐藏眼珠，睁眼时显示
        if (pupil != null)
        {
            pupil.gameObject.SetActive(!shouldClose);
        }

        // 2. 处理眼眶贴图替换
        if (eyeMainSR != null)
        {
            eyeMainSR.sprite = shouldClose ? closedEyeSprite : openEyeSprite;
        }

        // 调试
        Debug.Log($"[{gameObject.name}] 状态切换：{(shouldClose ? "闭合 (检测到面具)" : "睁开 (阻挡移动)")}");
    }

    public bool IsClosedState()
    {
        return isClosed;
    }
}