using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
            SceneManager.LoadScene(0);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            SceneManager.LoadScene(1);
    }
}
