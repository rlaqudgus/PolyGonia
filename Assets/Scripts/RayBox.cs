using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayBox : MonoBehaviour
{
    [SerializeField] string tagName;

    public bool CheckWithRay(Vector2 dir, float distance)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, dir, distance);
        Debug.DrawRay(transform.position, dir, Color.blue);
        foreach (var hit in hits) 
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag(tagName))
            {
                Debug.Log(hit.collider.name);
                return true;
            }
        }
        

        return false;
    }
}
