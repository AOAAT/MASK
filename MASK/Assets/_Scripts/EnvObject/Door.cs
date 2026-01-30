using UnityEngine;

public class Door : MonoBehaviour
{
    public void OpenDoor()
    {
        Debug.Log("所有的祭坛都被激活了，门打开");
        
        gameObject.SetActive(false); 
    }
}