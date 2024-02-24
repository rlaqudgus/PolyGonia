using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryBox : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D col)
    {
        //패링 오브젝트가 검출되었다면(패링 성공)
        //패링 오브젝트의 부모 접근 - 부모의 IAttackable 컴포넌트 접근 - byParry 메서드 실행
        if (col.CompareTag("Parry"))
        {
            var parent = col.transform.parent;
            if (parent.TryGetComponent<IAttackable>(out IAttackable a))
            {
                var shield = this.GetComponentInParent<Shield>();

                //패링된놈의 ByParry 메서드
                a.ByParry(shield);
                //플레이어 효과 - 넉백
                playerController.PlayerKnockBack();
                //플레이어 효과 - 카메라 흔들기
                StartCoroutine(playerController.cam.Shake());
                Debug.Log("parry");
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
