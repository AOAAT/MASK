using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VisionManager : MonoBehaviour
{
    public static VisionManager Instance;

    private List<EyeController> allEyes = new List<EyeController>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEye(EyeController eye)
    {
        if (!allEyes.Contains(eye))
        {
            allEyes.Add(eye);
        }
    }


    public bool IsAnyEyeWatching()
    {
        return allEyes.Any(eye => !eye.IsClosedState());
    }

    public void SetAllEyesState(bool shouldClose)
    {
       
    }
}