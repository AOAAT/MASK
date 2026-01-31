using UnityEngine;

public interface IMaskPower
{
    string PowerName { get; }
    Vector2 Direction { get; }
    Sprite Icon { get; } // 用于 UI 显示的图标
    bool CanPerformAction(Vector2 inputDirection);
    void ExecutePower(Transform playerTransform, Vector2 direction);
}