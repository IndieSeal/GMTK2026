using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private EventReference clickSound;
    [SerializeField] private float delay = 1;

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
        startButton.interactable = false;
        StartCoroutine(WaitFor());
    }

    private IEnumerator WaitFor()
    {
        RuntimeManager.PlayOneShot(clickSound);
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Newspaper");
    }
}