using UnityEngine;

public class EndTutorial : MonoBehaviour
{
    [SerializeField] private Door finalDoor;

    void OnEnable()
    {
        finalDoor.OnCrossed += FinishTutorial;
    }

    void OnDisable()
    {
        finalDoor.OnCrossed -= FinishTutorial;
    }

    private void FinishTutorial()
    {
        Debug.Log("finish");
        foreach(Tutorial1 tut in FindObjectsByType<Tutorial1>())
        {
            Destroy(tut.gameObject);
        }

        finalDoor.Close();
        finalDoor.enabled = false;
    }
}