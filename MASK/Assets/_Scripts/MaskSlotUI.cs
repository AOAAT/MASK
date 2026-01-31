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
    private Vector3 startPosition;

    void Awake()
    {
        slotImage = GetComponent<Image>();
        slotText = GetComponentInChildren<TextMeshProUGUI>();
        player = FindObjectOfType<PlayerController>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        startPosition = transform.position;
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotIndex >= player.GetOwnedMasks().Count) return;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) => transform.position = Input.mousePosition;

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null)
        {
            Altar altar = hit.collider.GetComponent<Altar>();
            if (altar != null && !altar.isActivated)
            {
                if (Vector2.Distance(player.transform.position, altar.transform.position) <= 1.1f * player.gridSize)
                {
                    player.ExecuteSacrificeFromUI(slotIndex, altar);
                }
            }
        }
        transform.position = startPosition;
    }
}