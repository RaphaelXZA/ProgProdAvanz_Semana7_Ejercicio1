using UnityEngine;
using GameJolt.UI;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    void Start()
    {
        GameJoltUI.Instance.ShowSignIn((success) =>
        {
            if (success)
            {
                SceneManager.LoadScene("MenuScene");
            }
            else
            {
                Debug.LogError("Login error");
            }
        });
    }
}