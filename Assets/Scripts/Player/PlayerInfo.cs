using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Save and Load
[System.Serializable]
public class PlayerInfo
{
    public float[] position;
    public int health;

    public PlayerInfo(PlayerController player)
    {
        PlayerStatus playerStatus = player.playerStatus;
        if (playerStatus == null) Debug.LogError("Player has no PlayerStatus component");

        position = new float[3];
        Vector3 pos = player.transform.position;
        position[0] = pos.x; position[1] = pos.y; position[2] = pos.z;

        health = playerStatus.currentHealth;
    }
}
