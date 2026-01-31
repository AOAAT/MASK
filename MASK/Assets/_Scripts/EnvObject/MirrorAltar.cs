using UnityEngine;

public class MirrorAltar : Altar
{
    protected override void Awake()
    {
        base.Awake();
        countsTowardsProgress = false; // 镜像祭坛不计入过关进度
    }

    public override bool TrySacrifice(IMaskPower mask, PlayerController player)
    {
        // 逻辑：计算反方向 (例如 Vector2.up 变为 Vector2.down)
        Vector2 mirroredDir = -mask.Direction;
        string newName = "Mirrored " + mask.PowerName;

        // 创建反向面具并返还给玩家
        IMaskPower newMask = new MoveDirectionMask(newName, mirroredDir, mask.Icon);
        player.AddMaskDirectly(newMask);

        Debug.Log($"镜像成功：{mask.Direction} 变为 {mirroredDir}");

        // 镜像祭坛通常可以重复使用，所以我们不调用基类的 Activate()
        // 仅仅给予视觉上的反馈
        if (sr != null) sr.color = Color.cyan;
        return true;
    }
}