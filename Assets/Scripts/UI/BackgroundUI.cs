using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BackgroundUI : MonoBehaviour
{
    public Sprite heartImage;
    private GameObject _heartGroup;
    private Stack<Image> _heartImages = new Stack<Image>();

    public PlayerController player;
    
    private void Awake()
    {
        _heartGroup = transform.GetChild(0).gameObject;

        if (player == null) player = FindObjectOfType<PlayerController>();
        player.OnDamaged += OnHeartUISub;
        player.OnHealed += OnHeartUIAdd;
    }

    public void OnHeartUIAdd(int amount)
    {
        Debug.Log($"HP : {amount}");
        for (int i = 0; i < amount; ++i)
        {
            if (_heartImages.Count >= player._maxHP) return;
         
            Debug.Log($"HP add : {i}th");
            
            var obj = new GameObject("Heart");
            var img = obj.AddComponent<Image>();
            img.sprite = heartImage;
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(80, 80);

            _heartImages.Push(img);
            obj.transform.SetParent(_heartGroup.transform);
        }
    }

    private void OnHeartUISub(int amount)
    {
        for (int i = 0; i < amount; ++i)
        {
            if (_heartImages.Count == 0) return;
            
            var img = _heartImages.Pop();
            Destroy(img.gameObject);
        }
    }
}
