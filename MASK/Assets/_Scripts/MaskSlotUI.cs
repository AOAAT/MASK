using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // 必须引用此命名空间来控制文字

public class MaskSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("设置")]
    public int slotIndex; // 在 Inspector 面板分别填 0, 1, 2, 3

    private Image slotImage;
    private TextMeshProUGUI slotText; // 用于显示面具名称的 UI 文字
    private PlayerController player;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;

    void Awake()
    {
        slotImage = GetComponent<Image>();
        // 自动获取子物体中的文字组件
        slotText = GetComponentInChildren<TextMeshProUGUI>();
        player = FindObjectOfType<PlayerController>();

        // 确保有 CanvasGroup 用于处理拖拽时的射线穿透
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        startPosition = transform.position;
    }

    void Update()
    {
        if (player == null) return;

        var masks = player.GetOwnedMasks();

        // 如果当前槽位索引对应玩家拥有的面具
        if (slotIndex < masks.Count)
        {
            // 1. 同步显示面具的名称
            slotText.text = masks[slotIndex].PowerName;
            slotImage.color = Color.white;

            // 2. 根据方向设置 ICON 识别色（增强直观性）
            Vector2 dir = masks[slotIndex].Direction;
            if (dir == Vector2.up) slotImage.color = new Color(1, 0.4f, 0.4f);    // 浅红
            else if (dir == Vector2.right) slotImage.color = new Color(1, 0.9f, 0.4f); // 浅黄
            else if (dir == Vector2.down) slotImage.color = new Color(0.4f, 0.7f, 1);  // 浅蓝
            else if (dir == Vector2.left) slotImage.color = new Color(0.4f, 1, 0.4f);  // 浅绿
        }
        else
        {
            // 3. 没面具时清空文字并使槽位半透明
            slotText.text = "";
            slotImage.color = new Color(1, 1, 1, 0.2f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotIndex >= player.GetOwnedMasks().Count) return;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; // 让鼠标能“穿过”图标看到场景里的祭坛
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slotIndex >= player.GetOwnedMasks().Count) return;
        transform.position = Input.mousePosition; // 图标跟手移动
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 射线检测：看拖拽到了场景中的哪个祭坛
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            Altar altar = hit.collider.GetComponent<Altar>();
            if (altar != null && !altar.isActivated)
            {
                // 校验玩家是否在祭坛相邻格子
                if (Vector2.Distance(player.transform.position, altar.transform.position) <= 1.1f * player.gridSize)
                {
                    player.ExecuteSacrificeFromUI(slotIndex, altar);
                }
            }
        }

        transform.position = startPosition; // 拖拽结束图标回到原位
    }
}