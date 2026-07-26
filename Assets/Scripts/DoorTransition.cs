using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTransition : MonoBehaviour
{
    [SerializeField] private Door door;
    [SerializeField] private string loadScene = "example";

    void OnEnable()
    {
        door.OnRoomChangedW += ChangeScene;
    }

    void OnDisable()
    {
        door.OnRoomChangedW -= ChangeScene;
    }

    private void ChangeScene()
    {
        StartCoroutine(WaitAFrame());
    }

    private IEnumerator WaitAFrame()
    {
        yield return null;
        SceneManager.LoadScene(loadScene);
    }
}