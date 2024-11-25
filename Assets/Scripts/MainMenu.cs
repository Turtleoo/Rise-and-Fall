using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // This method will be called from UI buttons with the scene name as a parameter
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
