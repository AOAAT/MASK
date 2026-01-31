using UnityEngine;

public class MoveDirectionMask : IMaskPower
{
    public string PowerName { get; private set; }
    public Vector2 Direction { get; private set; }
    public Sprite Icon { get; private set; } // 实现接口属性

    public MoveDirectionMask(string name, Vector2 direction, Sprite icon)
    {
        PowerName = name;
        Direction = direction;
        Icon = icon; // 存储传入的图标
    }

    public bool CanPerformAction(Vector2 inputDirection)
    {
        return inputDirection == Direction;
    }

    public void ExecutePower(Transform playerTransform, Vector2 direction)
    {
        // 扩展逻辑预留
    }
}