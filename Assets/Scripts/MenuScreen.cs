using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    [SerializeField] private Button startButton;

    void OnEnable()
    {
        startButton.onClick.AddListener(ChangeScene);
    }

    void OnDisable()
    {
        startButton.onClick.RemoveListener(ChangeScene);
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}