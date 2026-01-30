using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("移动配置")]
    [Tooltip("每个单元格的大小，修改此值可适配不同尺寸的地块")]
    public float gridSize = 1.0f;
    public float moveSpeed = 10f;

    [Header("碰撞与交互设置")]
    [Tooltip("请在下拉菜单中勾选你创建的 Wall 层")]
    public LayerMask wallLayer;
    [Tooltip("关卡中的终点/门")]
    public Door gameDoor;

    // 内部状态变量
    private List<IMaskPower> currentMasks = new List<IMaskPower>();
    private Vector3 targetPos;
    private bool isMoving = false;

    void Start()
    {
        // 初始位置强制对齐网格，防止放置偏差
        SnapToGrid();
        targetPos = transform.position;
    }

    void Update()
    {
        // 1. 处理平滑的一格一格移动逻辑
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.001f)
            {
                transform.position = targetPos;
                isMoving = false;
            }
            return;
        }

        // 2. 只有在静止状态下才接收移动输入
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        Vector2 inputDir = Vector2.zero;

        // 使用 GetKeyDown 确保按一次键只走一格
        if (Input.GetKeyDown(KeyCode.W)) inputDir = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S)) inputDir = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A)) inputDir = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D)) inputDir = Vector2.right;

        if (inputDir != Vector2.zero)
        {
            TryMove(inputDir);
        }
    }

    private void TryMove(Vector2 direction)
    {
        // A. 权限检查：是否拥有该方向的面具
        if (!currentMasks.Any(m => m.CanPerformAction(direction)))
        {
            Debug.Log($"缺失 {direction} 方向的面具，无法移动！");
            return;
        }

        // B. 碰撞检查：使用射线探测 Wall 层
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, gridSize, wallLayer);
        if (hit.collider != null)
        {
            Debug.Log("前面有墙，无法通过！");
            return;
        }

        // C. 设置移动目标
        targetPos = transform.position + (Vector3)(direction * gridSize);
        isMoving = true;
    }

    // --- 交互接口（供其他脚本调用） ---

    /// <summary>
    /// 被 MaskItem.cs 调用：拾取面具并加入背包
    /// </summary>
    public void PickUpMask(MaskItem item)
    {
        // 规则：每个方向的面具同时只能持有一个
        if (!currentMasks.Any(m => m.Direction == item.moveDirection))
        {
            currentMasks.Add(item.GetMaskPower());
            Debug.Log($"成功拾取: {item.maskName}");
            Destroy(item.gameObject); // 拾取后销毁场景中的物体
        }
        else
        {
            Debug.Log("你已经拥有该方向的面具了！");
        }
    }

    /// <summary>
    /// 被 MaskSlotUI.cs 调用：从 UI 拖拽到祭坛时执行献祭
    /// </summary>
    public void ExecuteSacrificeFromUI(int index, Altar altar)
    {
        if (index < currentMasks.Count)
        {
            IMaskPower sacrificedMask = currentMasks[index];
            currentMasks.RemoveAt(index); // 移除能力
            altar.Activate();            // 激活祭坛

            Debug.Log($"献祭了 {sacrificedMask.PowerName}，能力已消失。");
            CheckAllAltarsActivated();    // 检查是否全祭坛激活
        }
    }

    /// <summary>
    /// 供 UI 实时刷新显示的接口
    /// </summary>
    public List<IMaskPower> GetOwnedMasks() => currentMasks;

    private void CheckAllAltarsActivated()
    {
        // 查找场景中所有的祭坛
        Altar[] altars = FindObjectsOfType<Altar>();
        if (altars.Length > 0 && altars.All(a => a.isActivated))
        {
            Debug.Log("所有祭坛已点亮！门开启了。");
            if (gameDoor != null) gameDoor.OpenDoor();
        }
    }

    private void SnapToGrid()
    {
        float x = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float y = Mathf.Round(transform.position.y / gridSize) * gridSize;
        transform.position = new Vector3(x, y, transform.position.z);
    }
}