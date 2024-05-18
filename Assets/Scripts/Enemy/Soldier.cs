using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class Soldier : Triangle
{
    protected override IEnumerator Idle()
    {
        yield return base.Idle();

        _isMoving = false;
    }

    protected override IEnumerator Detect()
    {
        yield return base.Detect();

        //Attack state에서 Detect로 넘어갈 때 target이 null이면 idle로
        if (!_target) StateChange(EnemyState.Idle);
        
        //이동 로직 
        Move(chaseDir, _moveSpd);

        if (_isMeeleeRange)
        {
            StateChange(EnemyState.Attack);
        }
    }

    protected override IEnumerator Attack()
    {   
        yield return base.Attack();
        
        yield return AttackCombo();
    
        //공격패턴 끝나면 다시 감지
        _anim.SetTrigger("Detect");
        StateChange(EnemyState.Detect);
    }

    protected override IEnumerator Die()    
    {   
        yield return base.Die();    
    }   
}
