using UnityEngine;

public interface IMaskPower
{
    string PowerName { get; }
    Vector2 Direction { get; } // 面具代表的方向
    bool CanPerformAction(Vector2 inputDirection);
    void ExecutePower(Transform playerTransform, Vector2 direction);
}