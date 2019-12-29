using UnityEngine;
using System.Collections;

[System.Serializable]
public class BuffInfo
{
    public string BuffClassName;
    public int buffHealth;
    public int buffAttack;
    public int buffManaCost;
    public int buffSpecialAmount;
}

[System.Serializable]
public class Buff
{
    public int buffHealth;
    public int buffAttack;
    public int buffManaCost;
    public int buffSpecialAmount;

    public void SetValuesFromBuffInfo(BuffInfo bi)
    {
        buffAttack = bi.buffAttack;
        buffHealth = bi.buffHealth;
        buffManaCost = bi.buffManaCost;
        buffSpecialAmount = bi.buffSpecialAmount;
    }

    public virtual void BuffAction()
    {
        
    }

    public virtual void AttachBuffToEvent()
    {
        
    }

}
