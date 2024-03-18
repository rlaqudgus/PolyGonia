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
        RaycastHit2D[] boxHits = Physics2D.BoxCastAll(col.bounds.center, col.bounds.size, 0, Vector2.down, 0);
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
    public bool CheckWithRay(Vector2 dir, float distance)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, dir, distance);
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
