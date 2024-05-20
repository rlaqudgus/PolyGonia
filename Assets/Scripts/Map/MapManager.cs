using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class MapManager : Singleton<MapManager>
{
    private MapContainerData[] _rooms;
    private void Awake()
    {
        CreateSingleton(this);
        _rooms = GetComponentsInChildren<MapContainerData>(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void RevealRoom()
    {
        string newLoadScene = SceneManager.GetActiveScene().name;

        for (int i = 0; i < _rooms.Length; i++)
        {
            if (_rooms[i].sceneName == newLoadScene && !_rooms[i].IsRevealed)
            {
                _rooms[i].gameObject.SetActive(true);
                _rooms[i].IsRevealed = true;

                return;
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //this.Log("revealroom");
        RevealRoom();
    }
}
