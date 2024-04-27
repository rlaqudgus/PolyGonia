using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public SoundManager soundManager;
    public UIManager uiManager;
    public PlayerController playerController;
    public CameraController cameraController;

    public enum GameState { Init, Adventure, Inventory, Pause, Cinematic, Die, CutScene }

    private void Start()
    {
        
    }

    private void Awake()
    {
        //instanceê°€ ì´ˆê¸°?”ë˜ì§€ ?Šì•˜?¤ë©´
        if (instance == null)
        {
            //?ì‹ ?¼ë¡œ ì´ˆê¸°??
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init()
    {
        //dosth
        //GameStart UI Animation
        //GameStart Sound
        //Player Spawn
    }

    public void Adventure()
    {
        //dosth
        //½ÇÁ¦ °ÔÀÓ ½ÇÇàÇÏ°í ÀÖÀ» ¶§
        //°ÔÀÓ »óÈ²¿¡ µû¸¥ BGM, sound µîµî ´Ù¸£°Ô
        //
    }

    public void Inventory()
    {
        //dosth
        //ÇÒ·Î¿ì³ªÀÌÆ®ÀÇ °æ¿ì ÀÎº¥Åä¸®³ª ¸ÊÀ» º¸°í ÀÖÀ» ¶§ °ÔÀÓ »óÈ²ÀÌ ´Ş¶óÁü
        //ex ¼Óµµ°¡ ´À·ÁÁö°í Áöµµ¸¦ º¸´Â ÇÃ·¹ÀÌ¾î ¾Ö´Ï¸ŞÀÌ¼Ç ½ÇÇà
        //Inventory¸¦ º¸°í ÀÖÀ» ¶§ player°¡ ¿òÁ÷ÀÏ ¼ö ÀÖ°Ô ÇÒ °ÍÀÎ°¡?
        //¾Æ¿¹ °ÔÀÓ pause¸¦ ÇÒ °ÍÀÎ°¡?
        //sound on / off?
    }

    public void Pause() 
    {
        //dosth
        //Pause sound
        //sound on / off
        //Pause UI
    }

    public void Cinematic()
    {
        //dosth
        //sound
        //UI
        //Camera
        //Timeline?
    }

    public void CutScene()
    {
        //dosth
        //sound
        //UI
        //Camera
        //TimeLine?
    }

    public void Die()
    {
        //doth
        //sound
        //UI
        //Camera
    }
}
