using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class Soldier : Triangle
{
    protected override IEnumerator Idle()
    {
        isMoving = false;
        
        yield return null;
    }

    protected override IEnumerator Detect()
    {
        //방향 설정
        CalculateDir();

        //state가 바뀌지 않았다면 이동
        isMoving = true;

        //이동 로직
        if (TargetDistance(target) < meeleeRange)
        {
            isMoving = false;
            StateChange(EnemyState.Attack);
        }

        Vector2 newPos = (Vector2)transform.position - (runDir * moveSpd * Time.deltaTime);
        transform.position = newPos;

        yield return null;
    }

    protected override IEnumerator Attack()
    {
        // 거리에 들어오면 수행할 동작
        isMoving = false;
        
        yield return AttackCombo();
    }

    protected override IEnumerator Die()
    {
        EnemyManager.instance.RemoveEnemy(gameObject);
        
        yield return null;
    }

    protected override void TriangleAnim()
    {
        anim.SetBool("isChase", isMoving);
        anim.SetBool("isMeelee", isMeeleeRange);
    }
}