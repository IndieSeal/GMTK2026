using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;

public class ChaseSequence : MonoBehaviour
{
    public static event Action OnChaseSequenceStart;
    public static event Action OnChaseSequenceTP;
    public static event Action OnChaseSequenceChase;
    
    [SerializeField] private Door previousClosedDoor;
    [SerializeField] private Door generalClosedDoor;

    [SerializeField] private List<Room> rooms = new List<Room>();
    private int roomsCompleted;
    private bool shouldBeDone;

    [SerializeField] private EventReference EarthquakeSound;

    void OnEnable()
    {
        rooms.ForEach(x => x.OnRoomClearedEvent += OnRoomCompleted);
    }

    void OnDisable()
    {
        rooms.ForEach(x => x.OnRoomClearedEvent -= OnRoomCompleted);
    }

    //Remove this, it's not required, OdinInspector plugin required, just a quick debug
    [Button]
    private void OnRoomCompleted()
    {
        roomsCompleted++;
        if(!shouldBeDone && roomsCompleted >= rooms.Count)
        {
            shouldBeDone = true;
            StartChase();
        }
    }

    private void StartChase()
    {
        RuntimeManager.PlayOneShot(EarthquakeSound, transform.position);

        previousClosedDoor.enabled = true;
        generalClosedDoor.enabled = true;

        foreach(Door door in FindObjectsByType<Door>())
        {
            door.Close(playAudio: false);
        }

        OnChaseSequenceStart?.Invoke();
        StartCoroutine(Chase());
    }

    private IEnumerator Chase()
    {
        yield return new WaitForSeconds(12);

        OnChaseSequenceTP?.Invoke();

        yield return new WaitForSeconds(9f);

        OnChaseSequenceChase?.Invoke();
    }
}