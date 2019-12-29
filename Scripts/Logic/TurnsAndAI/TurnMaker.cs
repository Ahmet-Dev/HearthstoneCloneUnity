using UnityEngine;
using System.Collections;

public abstract class TurnMaker : MonoBehaviour {

    protected Player p;

    void Awake()
    {
        p = GetComponent<Player>();
    }

    public virtual void OnTurnStart()
    {
        p.OnTurnStart();
    }

}
