using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

//방패 관련 메서드나 오브젝트 관리해주는 클래스
//하위 오브젝트에 방패 콜라이더나 패링 히트박스 등 담아둬야 할듯
public class Shield : MonoBehaviour
{
    public float shieldForce;
    bool isColliding;

    [SerializeField] GameObject shieldBox;
    [SerializeField] GameObject shieldParry;
    [SerializeField] GameObject shieldEffect;
    [SerializeField] GameObject parryEffect;
    [SerializeField] Transform effectPos;

    //이건 좀 tlqkf
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShieldActivate(bool isActive)
    {
        shieldBox.SetActive(isActive);
    }

    public void ShieldParry()
    {
        StartCoroutine(ParryReset());
    }


    public void ShieldEffect()
    {
        this.Log("shieldEffect");
        EffectManager.Instance.InstantiateEffect(ParticleColor.WHITE, effectPos.position);
        //Instantiate(shieldEffect, effectPos.position, Quaternion.identity);
    }

    public void ParryEffect()
    {
        this.Log("parryEffect");
        EffectManager.Instance.InstantiateEffect(ParticleColor.YELLOW, effectPos.position);
        CameraManager.Zoom(6f, 0);
        CameraManager.ResetCamera(.15f);
        //Instantiate(parryEffect,effectPos.position,Quaternion.identity);
        //parryEffect.SetActive(true);
        
    }

    //이부분 때문에 방패 박스가 뒤늦게 setactive 되는 경우가 생김
    IEnumerator ParryReset()
    {
        shieldParry.SetActive(true);
        shieldBox.SetActive(false);
        yield return new WaitForSeconds(.3f);
        shieldParry.SetActive(false);

        if (!player._isShield)
        yield break;
        shieldBox.SetActive(true);
    }

    //방패는 콜리전으로 
    //방패에 Enemy가 collide 되면 넉백

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<IAttackable>(out IAttackable a))
        {
            a.ByShield(this);
        }
    }
}
