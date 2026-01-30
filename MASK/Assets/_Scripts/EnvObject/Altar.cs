using UnityEngine;

public class Altar : MonoBehaviour
{
    public bool isActivated { get; private set; } = false;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Activate()
    {
        isActivated = true;
        if (sr != null) sr.color = Color.green; // 激活后变绿
        Debug.Log("祭坛激活！");
    }
}