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

        if (!player._isShield) _collider.enabled = false;
    }

    public override void UseShield() => _collider.enabled = player._isShield;

    private void OnTriggerEnter2D(Collider2D col)
    {
        //패링 오브젝트가 검출되었다면(패링 성공)
        //패링 오브젝트의 부모 접근 - 부모의 IAttackable 컴포넌트 접근 - byParry 메서드 실행
        if (col.CompareTag("Parry") && _isParrying)
        {
            this.Log("parry");
            var parent = col.transform.parent;
            if (parent.TryGetComponent<IAttackable>(out IAttackable a))
            {
                var shield = this.GetComponentInParent<Shield>();

                //패링된놈의 ByParry 메서드
                a.ByParry(shield);
                //플레이어 효과 - 넉백
                player.PlayerKnockBack(0.5f);
                //플레이어 효과 - 카메라 흔들기

                CameraManager.Instance.Shake();
                //조이콘 효과 - 진동
                JoyConManager.Instance?.j[0].SetRumble(160, 320, 0.6f, 200);

            }
        }
    }
}
