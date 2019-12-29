using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CreatureLogic: ICharacter 
{
    public Player owner;
    public CardAsset ca;
    public CreatureEffect effect;
    public int UniqueCreatureID;
    public int ID
    {
        get{ return UniqueCreatureID; }
    }
    public bool Frozen = false;
    private int baseHealth;
    public int MaxHealth
    {
        get{ return baseHealth;}
    }
        
    private int health;

    public int Health
    {
        get{ return health; }

        set
        {
            if (value > MaxHealth)
                health = baseHealth;
            else if (value <= 0)
                Die();
            else
                health = value;
        }
    }

    public bool CanAttack
    {
        get
        {
            bool ownersTurn = (TurnManager.Instance.whoseTurn == owner);
            return (ownersTurn && (AttacksLeftThisTurn > 0) && !Frozen);
        }
    }

    private int baseAttack;
    public int Attack
    {
        get{ return baseAttack; }

    }
        
    private int attacksForOneTurn = 1;
    public int AttacksLeftThisTurn
    {
        get;
        set;
    }
    public CreatureLogic(Player owner, CardAsset ca)
    {
        this.ca = ca;
        baseHealth = ca.MaxHealth;
        Health = ca.MaxHealth;
        baseAttack = ca.Attack;
        attacksForOneTurn = ca.AttacksForOneTurn;
        if (ca.Charge)
            AttacksLeftThisTurn = attacksForOneTurn;
        this.owner = owner;
        UniqueCreatureID = IDFactory.GetUniqueID();
        if (ca.CreatureScriptName!= null && ca.CreatureScriptName!= "")
        {
            effect = System.Activator.CreateInstance(System.Type.GetType(ca.CreatureScriptName), new System.Object[]{owner, this, ca.specialCreatureAmount}) as CreatureEffect;
            effect.RegisterEffect();
        }
        CreaturesCreatedThisGame.Add(UniqueCreatureID, this);
    }

    public void OnTurnStart()
    {
        AttacksLeftThisTurn = attacksForOneTurn;
    }

    public void Die()
    {   
        owner.table.CreaturesOnTable.Remove(this);

        new CreatureDieCommand(UniqueCreatureID, owner).AddToQueue();
    }

    public void GoFace()
    {
        AttacksLeftThisTurn--;
        int targetHealthAfter = owner.otherPlayer.Health - Attack;
        new CreatureAttackCommand(owner.otherPlayer.PlayerID, UniqueCreatureID, 0, Attack, Health, targetHealthAfter).AddToQueue();
        owner.otherPlayer.Health -= Attack;
    }

    public void AttackCreature (CreatureLogic target)
    {
        AttacksLeftThisTurn--;
        int targetHealthAfter = target.Health - Attack;
        int attackerHealthAfter = Health - target.Attack;
        new CreatureAttackCommand(target.UniqueCreatureID, UniqueCreatureID, target.Attack, Attack, attackerHealthAfter, targetHealthAfter).AddToQueue();

        target.Health -= Attack;
        Health -= target.Attack;
    }

    public void AttackCreatureWithID(int uniqueCreatureID)
    {
        CreatureLogic target = CreatureLogic.CreaturesCreatedThisGame[uniqueCreatureID];
        AttackCreature(target);
    }
    public static Dictionary<int, CreatureLogic> CreaturesCreatedThisGame = new Dictionary<int, CreatureLogic>();

}
