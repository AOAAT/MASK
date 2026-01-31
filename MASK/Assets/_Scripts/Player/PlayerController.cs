using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

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
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                transform.position = targetPos;
                isMoving = false;
                CheckForDoorEntrance();
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
        if (!currentMasks.Any(m => m.CanPerformAction(direction))) return;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, gridSize, wallLayer);
        if (hit.collider != null) return;

        targetPos = transform.position + (Vector3)(direction * gridSize);
        isMoving = true;
    }

    // 核心修复点：拾取面具逻辑
    public void PickUpMask(MaskItem item)
    {
        // 检查是否已经拥有该方向的面具，如果没有则添加
        if (!currentMasks.Any(m => m.Direction == item.moveDirection))
        {
            currentMasks.Add(item.GetMaskPower());

            // 确保物体被立即销毁，防止重复拾取
            Destroy(item.gameObject);

            Debug.Log($"[数据层] 成功添加面具: {item.maskName}, 当前持有数: {currentMasks.Count}");
        }
    }

    public void AddMaskDirectly(IMaskPower newMask)
    {
        currentMasks.Add(newMask);
    }

    public void ExecuteSacrificeFromUI(int index, Altar altar)
    {
        if (index < currentMasks.Count)
        {
            IMaskPower maskToSacrifice = currentMasks[index];
            if (altar.TrySacrifice(maskToSacrifice, this))
            {
                currentMasks.RemoveAt(index);
                CheckAllAltarsActivated();
            }
        }
    }

    private void CheckAllAltarsActivated()
    {
        var targetAltars = FindObjectsOfType<Altar>().Where(a => a.countsTowardsProgress);
        if (targetAltars.All(a => a.isActivated))
        {
            if (gameDoor != null) gameDoor.OpenDoor();
        }
    }

    public List<IMaskPower> GetOwnedMasks() => currentMasks;

    private void CheckForDoorEntrance()
    {
        Door[] allDoors = FindObjectsOfType<Door>();
        foreach (Door door in allDoors)
        {
            if (door.isOpen && Vector2.Distance(transform.position, door.transform.position) < gridSize * 0.5f)
            {
                door.EnterDoor();
                return;
            }
        }
    }

    private void SnapToGrid()
    {
        float x = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float y = Mathf.Round(transform.position.y / gridSize) * gridSize;
        transform.position = new Vector3(x, y, transform.position.z);
    }
}