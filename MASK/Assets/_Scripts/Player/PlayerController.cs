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
        // R键快捷重置关卡
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            {
                transform.position = targetPos;
                isMoving = false;

                // 移动停止后检测是否进入了“门”
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
    private void CheckForDoorEntrance()
    {
        // 寻找场景中所有的 Door 脚本（包括挂在祭坛上的）
        Door[] allDoors = FindObjectsOfType<Door>();
        foreach (Door door in allDoors)
        {
            if (door.isOpen)
            {
                // 只要玩家与门中心的距离小于 0.5 个格子，就判定为进入
                float dist = Vector2.Distance(transform.position, door.transform.position);
                if (dist < gridSize * 0.5f)
                {
                    door.EnterDoor();
                    return;
                }
            }
        }
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

    private void CheckAllAltarsActivated()
    {
        Altar[] altars = FindObjectsOfType<Altar>();
        if (altars.Length > 0 && altars.All(a => a.isActivated))
        {
            if (gameDoor != null) gameDoor.OpenDoor();

            foreach (var altar in altars)
            {
                Door d = altar.GetComponent<Door>();
                if (d != null) d.OpenDoor();
            }
        }
    }

    public List<IMaskPower> GetOwnedMasks() => currentMasks;

    private void SnapToGrid()
    {
        float x = Mathf.Round(transform.position.x / gridSize) * gridSize;
        float y = Mathf.Round(transform.position.y / gridSize) * gridSize;
        transform.position = new Vector3(x, y, transform.position.z);
    }
}