using UnityEngine;
using System.Linq;

public class MirrorAltar : Altar
{
    [Header("标准素材库")]
    [Tooltip("请在此处拖入对应的标准面具图标")]
    public Sprite upIcon;
    public Sprite downIcon;
    public Sprite leftIcon;
    public Sprite rightIcon;

    protected override void Awake()
    {
        base.Awake();
        countsTowardsProgress = false;
        activatedColor = Color.gray;
    }

    public override bool TrySacrifice(IMaskPower mask, PlayerController player)
    {

        if (isActivated)
        {
            Debug.Log("该镜像祭坛能量已耗尽。");
            return false;
        }

 
        Vector2 mirroredDir = -mask.Direction;

        // 查重检定
        if (player.GetOwnedMasks().Any(m => m.Direction == mirroredDir))
        {
            Debug.LogWarning($"[镜像失败] 玩家已持有 {GetDirName(mirroredDir)} 方向，拒绝浪费祭坛次数。");
            return false;
        }

        // 获取标准素材
        string standardName = GetDirName(mirroredDir);
        Sprite standardIcon = GetDirIcon(mirroredDir);

        // 创建新面具并给玩家
        IMaskPower newMask = new MoveDirectionMask(standardName, mirroredDir, standardIcon);
        player.AddMaskDirectly(newMask);

        Debug.Log($"[镜像成功] {GetDirName(mask.Direction)} -> {standardName}");

        // 4.标记为“已使用”
      
        Activate();

        return true;
    }


    private string GetDirName(Vector2 dir)
    {
        if (dir == Vector2.up) return "Up";
        if (dir == Vector2.down) return "Down";
        if (dir == Vector2.left) return "Left";
        if (dir == Vector2.right) return "Right";
        return "Unknown";
    }

    private Sprite GetDirIcon(Vector2 dir)
    {
        if (dir == Vector2.up) return upIcon;
        if (dir == Vector2.down) return downIcon;
        if (dir == Vector2.left) return leftIcon;
        if (dir == Vector2.right) return rightIcon;
        return null;
    }
}