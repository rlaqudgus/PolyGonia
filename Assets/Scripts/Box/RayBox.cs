using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class RayBox : MonoBehaviour
{
    //태그로 검사할지 Layer로 검사할지? 고민해야한다
    [SerializeField] string tagName;

    BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }
    //콜라이더 박스 자체가 범위가 되어 확인 ray 쏘는 것 x
    public bool CheckWithBox()
    {
        RaycastHit2D[] boxHits = Physics2D.BoxCastAll(col.bounds.center, col.bounds.size, 0, Vector2.down, 0.1f);
        foreach (var hit in boxHits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag(tagName))
            {
                this.Log(hit.collider.name);
                return true;
            }
        }

        return false;
    }
    // [TG] [2024-04-06] [Refactor]
    // 1. 시작 위치 매개변수(Vector2 startPos)를 추가
    // 2. Player가 ground의 끝부분에 있는 지 판단하기 위해 Player의 왼쪽, 오른쪽 부분에서 ray를 발사하여 ground탐지
    // [광] [2024-05-02]
    // 1. "Default" 레이어에만 raycast 하도록 layermask 변수 설정
    public bool CheckWithRay(Vector2 startPos, Vector2 dir, float distance)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Default");
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, dir, distance, layerMask);
        Debug.DrawRay(transform.position, dir, Color.blue);

        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag(tagName))
            {
                //Debug.Log(hit.collider.name);
                return true;
            }
        }
        return false;
                
    }
}
