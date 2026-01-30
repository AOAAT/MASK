using UnityEngine;

public class MaskItem : MonoBehaviour
{
    public string maskName;
    public Vector2 moveDirection;

    // 当鼠标点击这个面具时触发
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
        return new MoveDirectionMask(maskName, moveDirection);
    }
}