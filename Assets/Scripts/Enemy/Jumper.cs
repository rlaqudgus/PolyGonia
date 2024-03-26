using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class Jumper : Triangle
{
    public override void InitSetting()
    {
        enemyController = GetComponent<Triangle>().enemyController;
    }
    
    public override IEnumerator Behaviour(EnemyState enemyState)
    {
        if (hp <= 0) StateChange(EnemyState.Die);
        anim.SetBool("isChase", isMoving);
        anim.SetBool("isMeelee", isMeeleeRange);

        switch (enemyState)
        {
            case EnemyState.Idle:
                isMoving = true;
                Move();
                if (!ray.CheckWithRay(Vector2.down, 5) || ray.CheckWithRay(moveDir, .5f))
                {
                    isMoving = false;
                    yield return new WaitForSeconds(2f);
                    TurnAround();
                    isMoving = true;
                }
            break;

            case EnemyState.Detect:
                //방향 설정
                CalculateDir();

                //state가 바뀌지 않았다면 이동
                isMoving = true;

                //meelee range인지 아닌지 체크하고 Attack state로 전환해야한다
                if (TargetDistance(target) < meeleeRange)
                {
                    StateChange(EnemyState.Attack);
                }
                Vector2 runPos = (Vector2)transform.position - (runDir * (moveSpd * 1.5f) * Time.deltaTime);
                transform.position = runPos;
            break;

            case EnemyState.Attack:
                isMoving = true;
                yield return Jump();
            break;

            case EnemyState.Die:
                throw new System.NotImplementedException();
            break;
        }

        yield break;
    }
}
