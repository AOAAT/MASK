using UnityEngine;
using System.Linq;

public class MirrorAltar : Altar
{
    [Header("素材库")]
    [Tooltip("拖入对应的标准面具图标")]
    public Sprite upIcon;
    public Sprite downIcon;
    public Sprite leftIcon;
    public Sprite rightIcon;

    protected override void Awake()
    {
        base.Awake();
        // 镜像祭坛作为功能性机关，不计入通关所需的献祭总数
        countsTowardsProgress = false;
    }

    public override bool TrySacrifice(IMaskPower mask, PlayerController player)
    {
       
        Vector2 mirroredDir = -mask.Direction;

      
        if (player.GetOwnedMasks().Any(m => m.Direction == mirroredDir))
        {
            Debug.LogWarning($"[镜像失败] 玩家已持有 {GetDirName(mirroredDir)} 方向，拒绝镜像。");
           
            return false;
        }

  
        string standardName = GetDirName(mirroredDir);
        Sprite standardIcon = GetDirIcon(mirroredDir);

     
        IMaskPower newMask = new MoveDirectionMask(standardName, mirroredDir, standardIcon);

        player.AddMaskDirectly(newMask);

        Debug.Log($"[镜像成功] {GetDirName(mask.Direction)} -> {standardName}");


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