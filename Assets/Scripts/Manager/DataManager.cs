using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// Unity의 저장 및 로드 시스템
// https://www.youtube.com/watch?v=XOjd_qU2Ido

public static class DataManager
{   

#region Player

    // [SH] - 어디까지 저장할 것인가 ...
    public static void SavePlayer(PlayerController player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerInfo data = new PlayerInfo(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerInfo LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            
            PlayerInfo data = formatter.Deserialize(stream) as PlayerInfo;
            stream.Close();
            
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void SetPlayer(PlayerController player, PlayerInfo playerInfo)
    {
        PlayerStatus playerStatus = player.playerStatus;
        if (playerStatus == null) Debug.LogError("Player has no PlayerStatus component");

        float[] pos = playerInfo.position;
        Vector3 position = new Vector3(pos[0], pos[1], pos[2]);
        player.transform.position = position;

        playerStatus.currentHealth = playerInfo.health;
    }

#endregion Player

#region Quest

    public static void SaveQuest()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/quests.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        try
        {
            List<QuestData> questDataList = QuestManager.Instance.GetQuestDataList();
            formatter.Serialize(stream, questDataList);
        }
        catch (System.Exception e)
        {   
            Debug.LogError("Failed to save quest " + e);
        }
        finally
        {
            stream.Close();
        }
    }

    public static List<QuestData> LoadQuest()
    {
        string path = Application.persistentDataPath + "/quests.fun";
        
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            try
            {
                List<QuestData> questDataList = formatter.Deserialize(stream) as List<QuestData>;

                return questDataList;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load quests: " + e);
                return null;
            }
            finally
            {
                stream.Close();
            }
        }
        else
        {
            Debug.LogError("Saved quests file not found in " + path);
            return null;
        }
    }

    public static void SetQuest(List<QuestData> questDataList)
    {
        QuestManager.Instance.SetQuestDataList(questDataList);
    }

#endregion
}
