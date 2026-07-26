using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionAfter : MonoBehaviour
{
    [SerializeField] private float waitFor = 6;
    [SerializeField] private string sceneName = "SampleScene";

    void Awake()
    {
        StartCoroutine(MoveTo());
    }

    private IEnumerator MoveTo()
    {
        yield return new WaitForSeconds(waitFor);

        SceneManager.LoadScene(sceneName);
    }
}