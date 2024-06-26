using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectBox : MonoBehaviour
{
    [SerializeField] string detectTag;

    //tag나 레이어에 따라 감지한 결과를 담고 있는 객체
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag(detectTag))
        {
            transform.parent.TryGetComponent<IDetectable>(out IDetectable d);
            d.Detect(col.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag(detectTag))
        {
            transform.parent.TryGetComponent<IDetectable>(out IDetectable d);
            d.Detect(null);
        }
    }
}
