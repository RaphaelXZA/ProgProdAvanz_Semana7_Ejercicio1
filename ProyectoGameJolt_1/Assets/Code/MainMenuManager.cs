using UnityEngine;
using GameJolt.API;
using UnityEngine.UI;
using GameJolt.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Button trophiesListButton;

    private void Awake()
    {
        trophiesListButton.onClick.AddListener(() =>
        {
            GameJoltUI.Instance.ShowTrophies();
        });
    }
    void Start()
    {
        Trophies.Unlock(267754);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
