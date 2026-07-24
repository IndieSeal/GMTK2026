using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] private List<Door> doors = new List<Door>();

    // Got to make it so it's waves instead of all enemies at once without waves
    [SerializeField] private List<EnemyBase> enemies = new List<EnemyBase>();
    public bool HasRoomBeenCleared { get; private set; }= false;
    private bool hasRoomBeenEntered = false;
    private int enemiesKilled;

    void OnEnable()
    {
        //Subscribe to doors to check if they were passed, and subscribe to enemies to see if they were killed
        foreach(EnemyBase enemy in enemies) enemy.OnKilled += OnEnemyKilled;
        foreach(Door door in doors) door.OnCrossed += DoorPassed;
    }

    void OnDisable()
    {
        foreach(EnemyBase enemy in enemies) enemy.OnKilled -= OnEnemyKilled;
        foreach(Door door in doors) door.OnCrossed -= DoorPassed;
    }

    private void OnEnemyKilled(EnemyBase enemy)
    {
        enemiesKilled++;
        if(enemiesKilled >= enemies.Count) OnRoomCleared();
    }

    private void DoorPassed()
    {
        if (hasRoomBeenEntered) return;
        hasRoomBeenEntered = true;

        foreach(Door door in doors) door.Close(true);
        foreach(EnemyBase enemy in enemies) enemy.OnStartBehaviour();
    }

    private void OnRoomCleared()
    {
        HasRoomBeenCleared = true;
        foreach(Door door in doors) door.RemoveForcedDoor();
    }
}