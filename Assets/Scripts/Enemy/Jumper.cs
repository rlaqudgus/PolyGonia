using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities;

public class Jumper : Triangle
{
    protected override IEnumerator Idle()
    {
        yield return base.Idle();
        
        // Animation은 항상 돌아다니는 애니메이션 실행된다
        // Jumper의 경우 Idle animation은 단지 정지되어 있는 상태에서의 애니메이션를 표시하기 위한 것이며
        // IEnumerator Idle에서 돌아가는 animation은 Detect animation에 해당한다
        // 두 개가 혼동될 수 있다
        _isMoving = true;
        Move(_moveDir, _moveSpd);

        // [TG] [2024-04-06] [Refactor]
        // 1. Raybox의 CheckWithRay에 시작 위치 매개변수를 추가하였으므로 같이 수정 
        // 2. 기존 Raybox의 trasnform.position이 (-0.5, 0, 0) 이고 BoxCollider의 Offset이 (0, 0) 임을 반영하여 변경
        // 3. 기존 값을 확인하는 것은 비효율적이기 때문에 참조를 할 수 있게 수정이 필요해 보임

        // [SH] [2024-04-14] [Fix] [Refactor] 
        // Move 함수를 활용할 수 있도록 Refactor
        // Move 함수에서 raycast 활용하여 _reachCliff / _onWall 플래그 업데이트
        // raycast는 현재 위치가 아닌 예측된 위치를 기반으로 이루어짐 
        // Move 함수 내에서 _isMoving 이 업데이트되도록 수정

        if (_reachCliff || _onWall) 
        {
            yield return new WaitForSeconds(2f);
            TurnAround();
            yield break;
        }
    }

    protected override IEnumerator Detect()
    {
        yield return base.Detect();
        
        //이동 로직
        Move(chaseDir, _moveSpd);

        if (_isMeeleeRange)
        {
            // 현재 Attack에 해당하는 Animation이 없어서 애니메이션 전환을 보류한다
            // _anim.SetTrigger("Attack");
            StateChange(EnemyState.Attack);
        }
    }

    protected override IEnumerator Attack()
    {
        yield return base.Attack();
        
        yield return Jump();

        //공격패턴 끝나면 다시 감지
        // _anim.SetTrigger("Detect");
        StateChange(EnemyState.Detect);
    }

    protected override IEnumerator Die()
    {
        yield return base.Die();
    }

}
