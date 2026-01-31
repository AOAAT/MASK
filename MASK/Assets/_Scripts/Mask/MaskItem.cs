using UnityEngine;
using TMPro;

public class MaskItem : MonoBehaviour
{
    public string maskName;
    public Vector2 moveDirection;
    public Sprite maskIcon;

    private void Awake()
    {
      
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
        }

      
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.sortingOrder = 10;

        TextMeshPro textComp = GetComponentInChildren<TextMeshPro>();
        if (textComp != null) textComp.GetComponent<MeshRenderer>().sortingOrder = 11;
    }

    private void OnMouseDown()
    {
        // 1. 获取玩家引用
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        // 2. 距离检测 (使用 sqrMagnitude 性能更好，且增加一点容错)
        float distance = Vector2.Distance(transform.position, player.transform.position);

        // 调试 Log：如果点不动，请看控制台输出的距离是多少
        Debug.Log($"点击面具：{maskName}，当前距离玩家：{distance}，允许距离：{1.2f * player.gridSize}");

        // 3. 执行拾取
        if (distance <= 1.2f * player.gridSize)
        {
            player.PickUpMask(this);
            Debug.Log($"成功拾取面具：{maskName}");
        }
        else
        {
            Debug.LogWarning("太远了，手够不着！");
        }
    }

    public IMaskPower GetMaskPower()
    {
        return new MoveDirectionMask(maskName, moveDirection, maskIcon);
    }
}