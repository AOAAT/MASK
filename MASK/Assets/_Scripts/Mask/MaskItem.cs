using UnityEngine;

public class MaskItem : MonoBehaviour
{
    public string maskName;
    public Vector2 moveDirection;
    public Sprite maskIcon; // 新增：在编辑器中拖入该面具对应的图片

    private void OnMouseDown()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= 1.1f)
            {
                player.PickUpMask(this);
            }
        }
    }

    public IMaskPower GetMaskPower()
    {
        // 将图片传递给能力实例
        return new MoveDirectionMask(maskName, moveDirection, maskIcon);
    }
}