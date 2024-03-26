using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class Civilian : Triangle
{
    public override void InitSetting()
    {
        enemyController = GetComponent<Triangle>().enemyController;
    }
    
    public override IEnumerator Behaviour(EnemyState enemyState)
    {
        if (hp <= 0) StateChange(EnemyState.Die);
        anim.SetBool("isRun", isMoving);
        anim.SetBool("isMeelee", isMeeleeRange);

        switch (enemyState)
        {
            case EnemyState.Idle:
                isMoving = false;
            break;

            case EnemyState.Detect:
                //방향 설정
                CalculateDir();

                //state가 바뀌지 않았다면 이동
                isMoving = true;

                //meelee range인지 아닌지 체크하고 Attack state로 전환해야한다
                if (TargetDistance(target) < meeleeRange)
                {
                    isMoving = false;
                    StateChange(EnemyState.Attack);
                }
                Vector2 newPos = (Vector2)transform.position + (runDir * moveSpd * Time.deltaTime);
                transform.position = newPos;
            break;

            case EnemyState.Attack:
                this.Log("dosth");
                StateChange(EnemyState.Idle);
            break;

            case EnemyState.Die:
                throw new System.NotImplementedException();
            break;
        }

        yield break;
    }
}
