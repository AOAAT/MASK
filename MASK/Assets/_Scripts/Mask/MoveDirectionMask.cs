using UnityEngine;

public class MoveDirectionMask : IMaskPower
{
    public string PowerName { get; private set; }
    public Vector2 Direction { get; private set; }

    public MoveDirectionMask(string name, Vector2 direction)
    {
        PowerName = name;
        Direction = direction;
    }

    public bool CanPerformAction(Vector2 inputDirection)
    {
        return inputDirection == Direction;
    }

    public void ExecutePower(Transform playerTransform, Vector2 direction)
    {
    }
}