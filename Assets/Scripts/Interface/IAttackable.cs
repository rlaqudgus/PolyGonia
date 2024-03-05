using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    
    public void ByShield(Shield shield);

    public void ByParry(Shield shield);

    public void BySpear();

}
