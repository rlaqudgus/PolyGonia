using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class ParryBox : MonoBehaviour
{
    [SerializeField] PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D col)
    {
        //�и� ������Ʈ�� ����Ǿ��ٸ�(�и� ����)
        //�и� ������Ʈ�� �θ� ���� - �θ��� IAttackable ������Ʈ ���� - byParry �޼��� ����
        if (col.CompareTag("Parry"))
        {
            this.Log("parry");
            var parent = col.transform.parent;
            if (parent.TryGetComponent<IAttackable>(out IAttackable a))
            {
                var shield = this.GetComponentInParent<Shield>();

                //�и��ȳ��� ByParry �޼���
                a.ByParry(shield);
                //�÷��̾� ȿ�� - �˹�
                playerController.PlayerKnockBack();
                //�÷��̾� ȿ�� - ī�޶� ����
                StartCoroutine(playerController.cam.Shake());
                
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
