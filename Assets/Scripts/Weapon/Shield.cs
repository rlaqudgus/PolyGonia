using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Shield : Weapon
{
    bool _isParrying;
    BoxCollider2D _collider;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();    
        _collider.enabled = false;
        _isParrying = false;
    }

    public override void UseWeapon(int idx) => StartCoroutine(ParryReset());

    IEnumerator ParryReset()
    {
        _isParrying = true;
        yield return new WaitForSeconds(.3f);
        _isParrying = false;

        if (!player.isShield) _collider.enabled = false;
    }

    public override void UseShield() => _collider.enabled = player.isShield;

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    EffectManager.Instance.InstantiateEffect(ParticleColor.WHITE, transform.position);
    //}

    private void OnTriggerEnter2D(Collider2D col)
    {
        //�и� ������Ʈ�� ����Ǿ��ٸ�(�и� ����)
        //�и� ������Ʈ�� �θ� ���� - �θ��� IAttackable ������Ʈ ���� - byParry �޼��� ����
        if (col.CompareTag("Parry") && _isParrying)
        {
            this.Log("parry");
            var parent = col.transform.parent;
            if (parent.TryGetComponent<IAttackable>(out IAttackable a))
            {
                var shield = this.GetComponentInParent<Shield>();

                //�и��ȳ��� ByParry �޼���
                //a.ByParry(shield);

                EffectManager.Instance.InstantiateEffect(ParticleColor.YELLOW, (transform.position + col.transform.position) * .5f);
                //�÷��̾� ȿ�� - �˹�
                player.PlayerKnockBack(0.5f);
                //�÷��̾� ȿ�� - ī�޶� ����

                CameraManager.Instance.Shake();

                //ī�޶� zoom �Դ� ���� �ϴ� ����Ʈ ���餷�ּ�
                CameraManager.Instance.Zoom(6, 0);

                CameraManager.Instance.ResetCamera(.15f);
                //������ ȿ�� - ����
                JoyConManager.Instance?.j[0].SetRumble(160, 320, 0.6f, 200);

            }
        }

        else if(col.CompareTag("Parry") && !_isParrying)
        {
            EffectManager.Instance.InstantiateEffect(ParticleColor.WHITE, transform.position);
        }
    }
}
