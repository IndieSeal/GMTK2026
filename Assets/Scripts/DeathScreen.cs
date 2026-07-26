using System;
using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public static event Action OnNewScene;
    
    [SerializeField] private Transform deathPosition;
    [SerializeField] private CanvasGroup canvas;

    [SerializeField] private float alphaDuration = 2;

    protected KidInput Input => KidInput.Instance;
    
    void OnEnable()
    {
        PlayerMovement.OnPlayerDeath += PlayerDied;
    }

    void OnDisable()
    {
        PlayerMovement.OnPlayerDeath -= PlayerDied;
    }

    private void PlayerDied()
    {
        PlayerMovement.instance.transform.position = deathPosition.position;
        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        yield return null;
        
        Input.SubscribeToInputAction(Input.InteractAction, ChangeScenes, null, null);

        float t = 0;
        
        while(canvas.alpha < 0.98)
        {            
            t += Time.deltaTime;
            canvas.alpha = Mathf.Lerp(0, 1, t/alphaDuration);
            
            yield return null;
        }

        canvas.alpha = 1;
    }

    private void ChangeScenes()
    {
        RuntimeManager.StudioSystem.setParameterByName("Death", 0);
        Input.UnsubscribeToInputAction(Input.InteractAction, ChangeScenes, null, null);
        OnNewScene?.Invoke();

        StartCoroutine(WaitAFrame());
    }

    private IEnumerator WaitAFrame()
    {
        yield return null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}