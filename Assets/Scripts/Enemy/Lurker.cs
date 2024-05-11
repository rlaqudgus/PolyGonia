using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lurker : Triangle
{
    private float _width;
    private float _height;
    private Color _color;

    protected override void Start()
    {
        base.Start();

        _width = transform.localScale.x;
        _height = transform.localScale.y;

        _meeleeRange = 2f / 3f * _height + 1f;

        _color = _spriteRenderer.color;
        _color.a = 0f;
        _spriteRenderer.color = _color;
    }

    protected override void Update()
    {
        base.Update();

        if (_isMeeleeRange)
        {
            float opacity = TargetDistance(_target) / _meeleeRange;
            float gamma = 0.5f;
            float alpha = 1f - Mathf.Pow(opacity, 1f/gamma);
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            
            _color.a = alpha;
            _spriteRenderer.color = _color;
        }
    }

    protected override IEnumerator Idle()
    {
        yield return base.Idle();

        _isMoving = false;
    }

    protected override IEnumerator Detect()
    {
        yield return base.Detect();
        
        _isMoving = false;

        if (_isMeeleeRange)
        {
            // _anim.SetTrigger("Attack");
            StateChange(EnemyState.Attack);
        }
    }

    protected override IEnumerator Attack()
    {   
        yield return base.Attack();
        
        Quaternion rotation = transform.rotation;
        Vector3 defaultDir = Vector3.up;
        if (_spriteRenderer.flipY) defaultDir = Vector3.down;

        Vector3 dir = rotation * defaultDir;

        yield return Prick(dir * _height, _moveSpd);
        yield return Prick(-dir * _height, _moveSpd);

        while (_isMeeleeRange)
        {
            yield return null;
        }

        _color = _spriteRenderer.color;
        _color.a = 0f;
        _spriteRenderer.color = _color;

        //공격패턴 끝나면 다시 감지
        // _anim.SetTrigger("Detect");
        StateChange(EnemyState.Detect);
    }

    protected override IEnumerator Die()    
    {   
        yield return base.Die();    
    }

    private IEnumerator Prick(Vector3 dir, float speed)
    {
        speed = Mathf.Max(speed, 0.1f);
    
        float dist = dir.magnitude;
        float remainingDistance = dist;
        int MAX_ITER = 1000;
        int iter = 0;

        while (remainingDistance > 0)
        {
            if (iter > MAX_ITER) break;

            Vector3 delta = dir * speed * Time.deltaTime;

            transform.position += delta;
            // transform.Translate(delta);
            remainingDistance -= delta.magnitude;
            iter++;

            yield return null;
        }        
    }
}
