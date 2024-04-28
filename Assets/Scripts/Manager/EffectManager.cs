using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using Utilities;

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
        //최상위 콜라이더 invincible로 변경
        go.layer = LayerMask.NameToLayer("Invincible");
        //하위 모든 오브젝트 invincible로 변경
        foreach (Transform child in go.transform)
        {
            child.gameObject.layer = child.gameObject.name == "Detector" ? LayerMask.NameToLayer("Player1") : LayerMask.NameToLayer("Invincible");
        }

        //깜빡깜빡 이펙트
        while (_playercontroller._isInvincible)
        {
            yield return new WaitForSeconds(.2f);
            go.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(.2f);
            go.GetComponent<SpriteRenderer>().enabled = true;
        }
        //최상위 콜라이더 다시 플레이어로 변경
        go.layer = LayerMask.NameToLayer("Player1");

        //하위 모든 오브젝트 다시 플레이어로 변경 
        foreach (Transform child in go.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Player1");
        }
    }


    
}
