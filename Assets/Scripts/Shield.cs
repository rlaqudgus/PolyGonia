using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� ���� �޼��峪 ������Ʈ �������ִ� Ŭ����
//���� ������Ʈ�� ���� �ݶ��̴��� �и� ��Ʈ�ڽ� �� ��Ƶ־� �ҵ�
public class Shield : MonoBehaviour
{
    public float shieldForce;
    bool isColliding;

    [SerializeField] GameObject shieldBox;
    [SerializeField] GameObject shieldParry;
    [SerializeField] GameObject shieldEffect;
    [SerializeField] GameObject parryEffect;
    [SerializeField] Transform effectPos;

    //�̰� �� tlqkf
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
        Debug.Log("shieldEffect");
        Instantiate(shieldEffect, effectPos.position, Quaternion.identity);
    }

    public void ParryEffect()
    {
        Debug.Log("parryEffect");
        Instantiate(parryEffect,effectPos.position,Quaternion.identity);
        //parryEffect.SetActive(true);
        
    }
    public void SpearActivate()
    {

    }

    //�̺κ� ������ ���� �ڽ��� �ڴʰ� setactive �Ǵ� ��찡 ����
    IEnumerator ParryReset()
    {
        shieldParry.SetActive(true);
        shieldBox.SetActive(false);
        yield return new WaitForSeconds(.3f);
        shieldParry.SetActive(false);

        if (!player.isShield)
        yield break;
        shieldBox.SetActive(true);
    }

    //���д� �ݸ������� 
    //���п� Enemy�� collide �Ǹ� �˹�

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<IAttackable>(out IAttackable a))
        {
            a.ByShield(this);
        }
    }
}
