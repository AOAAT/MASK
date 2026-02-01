using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [Header("UI 配置")]
    public GameObject buttonPrefab; 
    public Transform contentParent; 

    [Header("配置")]
    public int startLevelIndex = 2; 
    void Start()
    {
        GenerateLevelButtons();
    }

    void GenerateLevelButtons()
    {
        
        foreach (Transform child in contentParent) Destroy(child.gameObject);

      
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        int displayIndex = 1;

        for (int i = startLevelIndex; i < sceneCount; i++)
        {
          
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);

          
            GameObject btnObj = Instantiate(buttonPrefab, contentParent);
            LevelButton script = btnObj.GetComponent<LevelButton>();

            if (script != null)
            {
                script.Setup(displayIndex, sceneName);
                displayIndex++;
            }
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}