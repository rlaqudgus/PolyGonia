using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class Civilian : Triangle
{
    protected override IEnumerator Idle()
    {
        yield return base.Idle();
        
        _isMoving = false;
    }

    protected override IEnumerator Detect()
    {
        yield return base.Detect();

        //이동 로직
        Move(runDir, _moveSpd);
        
        // [SH] [2024-04-16] [Refactor]
        // 애니메이션 간소화
        // 현재 Civilian의 Attack Animation이 없어서 IEnumerator Attack을 거치게 되면
        // Animation 전환 없이 State만 전환되어 혼란스러울 수 있음
        // 나중에 필요하면 Attack Animation과 함께 Attack State를 추가하는 것이 나을 것 같다

        if (_isMeeleeRange)
        {   
            _isMoving = false;  
            this.Log("dosth");  
            StateChange(EnemyState.Idle);   
        }   
    }

    protected override IEnumerator Attack()
    {
        yield return base.Attack();
    }

    protected override IEnumerator Die()
    {   
        yield return base.Die();
    }   
}
