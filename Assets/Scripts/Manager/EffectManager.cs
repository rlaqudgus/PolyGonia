using System;
using System.Collections;
using System.Collections.Generic;
using Bases;
using UnityEngine;
using UnityEngine.Diagnostics;
using Utilities;

// Player -> Player_AF
// PlayerController -> PlayerController_AF로 바꿔줘야함..
public enum ParticleColor { WHITE, YELLOW, RED, MIX};
public class EffectManager : Singleton<EffectManager>
{
    //public enum particleEffect { white, yellow, red, mix };
    PlayerController _playercontroller;
 
    [SerializeField] GameObject[] _particles;
    // Start is called before the first frame update
    private void Awake()
    {
        CreateSingleton(this);
        _playercontroller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void InvincibleEffect() => StartCoroutine(IEInvincible());


    public void KnockBackEffect(Transform obj, Vector2 dir, float knockBackDist)
    {
        obj.transform.position = (Vector2)obj.transform.position + new Vector2(dir.x * knockBackDist, 0);
    }

    public void InstantiateEffect(ParticleColor color ,Vector3 pos) => Instantiate(_particles[(int)color], pos, Quaternion.identity);

    public void TimeStop(float time) => StartCoroutine(IETimeStop(time));


    IEnumerator IETimeStop(float time)
    {
        Time.timeScale = 0;
        this.Log("timestop");
        yield return new WaitForSecondsRealtime(time);
        this.Log("timerestart");
        Time.timeScale = 1;
    }

    

    IEnumerator IEInvincible()
    {
        GameObject go = _playercontroller.gameObject;
        //�ֻ��� �ݶ��̴� invincible�� ����
        go.layer = LayerMask.NameToLayer("Invincible");
        //���� ��� ������Ʈ invincible�� ����
        foreach (Transform child in go.transform)
        {
            child.gameObject.layer = child.gameObject.name == "Detector" ? LayerMask.NameToLayer("Player1") : LayerMask.NameToLayer("Invincible");
        }

        //�������� ����Ʈ
        while (_playercontroller.isInvincible)
        {
            yield return new WaitForSeconds(.2f);
            go.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(.2f);
            go.GetComponent<SpriteRenderer>().enabled = true;
        }
        //�ֻ��� �ݶ��̴� �ٽ� �÷��̾�� ����
        go.layer = LayerMask.NameToLayer("Player1");

        //���� ��� ������Ʈ �ٽ� �÷��̾�� ���� 
        foreach (Transform child in go.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Player1");
        }
    }


    
}
