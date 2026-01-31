using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [Header("移动配置")]
    public float gridSize = 1.0f;
    public float moveSpeed = 10f;

    [Header("碰撞与交互设置")]
    public LayerMask wallLayer;
    public Door gameDoor;

    private List<IMaskPower> currentMasks = new List<IMaskPower>();
    private Vector3 targetPos;
    private bool isMoving = false;

    void Start()
    {
        SnapToGrid();
        targetPos = transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                transform.position = targetPos;
                isMoving = false;
            }
            return;
        }

        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        Vector2 inputDir = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.W)) inputDir = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S)) inputDir = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A)) inputDir = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D)) inputDir = Vector2.right;

        if (inputDir != Vector2.zero) TryMove(inputDir);
    }

    private void TryMove(Vector2 direction)
    {
        // 逻辑核心：只要有面具，就能发起移动
        if (!currentMasks.Any(m => m.CanPerformAction(direction)))
        {
            Debug.Log($"缺失 {direction} 方向的面具，无法移动！");
            return;
        }

        // 仅保留墙壁/障碍物的物理碰撞检查
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, gridSize, wallLayer);
        if (hit.collider != null)
        {
            Debug.Log("前面有墙，无法通过！");
            return;
        }

        targetPos = transform.position + (Vector3)(direction * gridSize);
        isMoving = true;
    }

    public void PickUpMask(MaskItem item)
    {
        if (!currentMasks.Any(m => m.Direction == item.moveDirection))
        {
            currentMasks.Add(item.GetMaskPower());
            Destroy(item.gameObject);
        }
    }

    public void ExecuteSacrificeFromUI(int index, Altar altar)
    {
        if (index < currentMasks.Count)
        {
            currentMasks.RemoveAt(index);
            altar.Activate();
            CheckAllAltarsActivated();
        }
    }

    public List<IMaskPower> GetOwnedMasks() => currentMasks;

    private void CheckAllAltarsActivated()
    {
        Altar[] altars = FindObjectsOfType<Altar>();
        if (altars.Length > 0 && altars.All(a => a.isActivated))
        {
            if (gameDoor != null) gameDoor.OpenDoor();
        }
    }

    private void SnapToGrid()
    {
        float x = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float y = Mathf.Round(transform.position.y / gridSize) * gridSize;
        transform.position = new Vector3(x, y, transform.position.z);
    }
}