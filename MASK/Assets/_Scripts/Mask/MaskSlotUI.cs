using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MaskSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("设置")]
    public int slotIndex;

    private Image slotImage;
    private TextMeshProUGUI slotText;
    private PlayerController player;
    private CanvasGroup canvasGroup;

    // 用于处理布局归位的变量
    private Transform originalParent;
    private int originalIndex;
    private Vector3 startPosition;

    void Awake()
    {
        slotImage = GetComponent<Image>();
        slotText = GetComponentInChildren<TextMeshProUGUI>();
        player = FindObjectOfType<PlayerController>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

        // 记录 UI 所在的 Layout Group 父物体
        originalParent = transform.parent;
    }

    void Update()
    {
        if (player == null) return;
        var masks = player.GetOwnedMasks();

        if (slotIndex < masks.Count)
        {
            IMaskPower mask = masks[slotIndex];
            if (mask.Icon != null)
            {
                slotImage.sprite = mask.Icon;
                slotImage.color = Color.white;
                slotText.text = "";
            }
            else
            {
                slotImage.sprite = null;
                slotText.text = mask.PowerName;
                SetDirectionColor(mask.Direction);
            }
        }
        else
        {
            slotImage.sprite = null;
            slotText.text = "";
            slotImage.color = new Color(1, 1, 1, 0.2f);
        }
    }

    private void SetDirectionColor(Vector2 dir)
    {
        if (dir == Vector2.up) slotImage.color = new Color(1, 0.4f, 0.4f);
        else if (dir == Vector2.right) slotImage.color = new Color(1, 0.9f, 0.4f);
        else if (dir == Vector2.down) slotImage.color = new Color(0.4f, 0.7f, 1);
        else if (dir == Vector2.left) slotImage.color = new Color(0.4f, 1, 0.4f);
    }

    // --- 拖拽系统修改部分 ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 如果该槽位没有面具，禁止拖拽
        if (player == null || slotIndex >= player.GetOwnedMasks().Count) return;

        // 1. 记录它在布局组中的原始序号
        originalIndex = transform.GetSiblingIndex();

        // 2. 将父级临时设为更高一层的 Canvas，脱离布局组限制
        // 这样移动时不会被布局组强行拉回，且能显示在其他 UI 之上
        transform.SetParent(originalParent.parent);

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false; // 允许射线穿透 UI 击中场景中的祭坛
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 跟随鼠标移动
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 射线检测：检查鼠标释放位置是否有祭坛
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            Altar altar = hit.collider.GetComponent<Altar>();
            // 确保祭坛未被激活且距离玩家足够近
            if (altar != null && !altar.isActivated)
            {
                if (Vector2.Distance(player.transform.position, altar.transform.position) <= 1.1f * player.gridSize)
                {
                    player.ExecuteSacrificeFromUI(slotIndex, altar);
                }
            }
        }

        // 3. 核心修复：将物体重新放回布局组父物体下
        transform.SetParent(originalParent);

        // 4. 恢复它在列表中的原始位置序号
        // Layout Group 会在下一帧自动根据 SiblingIndex 将它移动回正确坐标
        transform.SetSiblingIndex(originalIndex);
    }
}