using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("移动配置")]
    public float gridSize = 1.0f;
    public float moveSpeed = 10f;

    [Header("引用设置")]
    public LayerMask wallLayer;
    public Door gameDoor;

    [Header("视觉设置")]
    [Tooltip("如果不手动赋值，脚本会自动在子物体中查找")]
    public SpriteRenderer visualRenderer; 

    private List<IMaskPower> currentMasks = new List<IMaskPower>();
    private Vector3 targetPos;
    private bool isMoving = false;
    private Stack<GameStateSnapshot> undoStack = new Stack<GameStateSnapshot>();

    void Start()
    {

        if (visualRenderer == null)
        {
            visualRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (visualRenderer == null) Debug.LogError("Player 子物体中没找到 SpriteRenderer，且未手动赋值！");

        SnapToGrid();
        targetPos = transform.position;
        SaveState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (Input.GetKeyDown(KeyCode.Z) && !isMoving) Undo();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0); // 加载主菜单场景
        }

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
        // 1. 面具能力检定
        if (!currentMasks.Any(m => m.CanPerformAction(direction))) return;

        // 2. 图形翻转逻辑
        if (visualRenderer != null)
        {
            if (direction == Vector2.right)
            {
                visualRenderer.flipX = true; // 向右翻转
            }
            else if (direction == Vector2.left)
            {
                visualRenderer.flipX = false; // 向左复原
            }
            // 上下移动保持原状
        }

        // 3. 墙壁碰撞检定
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, gridSize, wallLayer);
        if (hit.collider != null) return;

        // 4. 执行移动
        SaveState();

        targetPos = transform.position + (Vector3)(direction * gridSize);
        isMoving = true;
    }

    public void SaveState()
    {
        Altar[] allAltars = FindObjectsOfType<Altar>();
        int[] aStates = allAltars.Select(a => a.GetState()).ToArray();

        MaskItem[] allItems = FindObjectsOfType<MaskItem>(true);
        bool[] mStates = allItems.Select(m => m.gameObject.activeSelf).ToArray();

        undoStack.Push(new GameStateSnapshot(transform.position, currentMasks, aStates, mStates));
    }

    private void Undo()
    {
        if (undoStack.Count <= 1) return;

        undoStack.Pop();
        GameStateSnapshot prev = undoStack.Peek();

        transform.position = prev.playerPosition;
        targetPos = transform.position;
        currentMasks = new List<IMaskPower>(prev.ownedMasks);

        Altar[] altars = FindObjectsOfType<Altar>();
        for (int i = 0; i < altars.Length; i++) altars[i].SetStateFromUndo(prev.altarStates[i]);

        MaskItem[] items = FindObjectsOfType<MaskItem>(true);
        for (int i = 0; i < items.Length; i++) items[i].gameObject.SetActive(prev.maskActiveStates[i]);
    }

    public void PickUpMask(MaskItem item)
    {
        if (!currentMasks.Any(m => m.Direction == item.moveDirection))
        {
            SaveState();
            currentMasks.Add(item.GetMaskPower());
            item.gameObject.SetActive(false);
        }
    }

    public void AddMaskDirectly(IMaskPower newMask) => currentMasks.Add(newMask);
    public List<IMaskPower> GetOwnedMasks() => currentMasks;
    private void SnapToGrid() { transform.position = new Vector3(Mathf.Round(transform.position.x / gridSize) * gridSize, Mathf.Round(transform.position.y / gridSize) * gridSize, transform.position.z); }
    private void CheckForDoorEntrance() { Door[] doors = FindObjectsOfType<Door>(); foreach (var d in doors) { if (d.isOpen && Vector2.Distance(transform.position, d.transform.position) < gridSize * 0.5f) d.EnterDoor(); } }
    private void CheckAllAltarsActivated() { if (FindObjectsOfType<Altar>().Where(a => a.countsTowardsProgress).All(a => a.isActivated)) gameDoor?.OpenDoor(); }

    public void ExecuteSacrificeFromUI(int index, Altar altar)
    {
        if (index < currentMasks.Count)
        {
            SaveState();
            if (altar.TrySacrifice(currentMasks[index], this))
            {
                currentMasks.RemoveAt(index);
                CheckAllAltarsActivated();
            }
            else
            {
                undoStack.Pop();
            }
        }
    }
}