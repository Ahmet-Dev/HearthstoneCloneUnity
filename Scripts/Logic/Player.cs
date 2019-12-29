using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, ICharacter
{
    public int PlayerID;
    public CharacterAsset charAsset;
    public PlayerArea PArea;
    public SpellEffect HeroPowerEffect;

    public Deck deck;
    public Hand hand;
    public Table table;

    private int bonusManaThisTurn = 0;
    public bool usedHeroPowerThisTurn = false;

    public int ID
    {
        get{ return PlayerID; }
    }

    private int manaThisTurn;
    public int ManaThisTurn
    {
        get{ return manaThisTurn;}
        set
        {
            manaThisTurn = value;
            PArea.ManaBar.TotalCrystals = manaThisTurn;
            new UpdateManaCrystalsCommand(this, manaThisTurn, manaLeft).AddToQueue();
        }
    }

    private int manaLeft;
    public int ManaLeft
    {
        get
        { return manaLeft;}
        set
        {
            manaLeft = value;
            PArea.ManaBar.AvailableCrystals = manaLeft;
            new UpdateManaCrystalsCommand(this, ManaThisTurn, manaLeft).AddToQueue();
            Debug.Log(ManaLeft);
            if (TurnManager.Instance.whoseTurn == this)
                HighlightPlayableCards();
        }
    }

    public Player otherPlayer
    {
        get
        {
            if (Players[0] == this)
                return Players[1];
            else
                return Players[0];
        }
    }

    private int health;
    public int Health
    {
        get { return health;}
        set
        {
            health = value;
            if (value <= 0)
                Die(); 
        }
    }

    public delegate void VoidWithNoArguments();
    public event VoidWithNoArguments EndTurnEvent;

    public static Player[] Players;

    void Awake()
    {
        Players = GameObject.FindObjectsOfType<Player>();
        PlayerID = IDFactory.GetUniqueID();
    }

    public virtual void OnTurnStart()
    {
        Debug.Log("In ONTURNSTART for "+ gameObject.name);
        usedHeroPowerThisTurn = false;
        ManaThisTurn++;
        ManaLeft = ManaThisTurn;
        foreach (CreatureLogic cl in table.CreaturesOnTable)
            cl.OnTurnStart();
        PArea.HeroPower.WasUsedThisTurn = false;

    }

    public void GetBonusMana(int amount)
    {
        bonusManaThisTurn += amount;
        ManaThisTurn += amount;
        ManaLeft += amount;
    }   

    public void OnTurnEnd()
    {
        if(EndTurnEvent != null)
            EndTurnEvent.Invoke();
        ManaThisTurn -= bonusManaThisTurn;
        bonusManaThisTurn = 0;
        GetComponent<TurnMaker>().StopAllCoroutines();
    }

    public void DrawACard(bool fast = false)
    {
        if (deck.cards.Count > 0)
        {
            if (hand.CardsInHand.Count < PArea.handVisual.slots.Children.Length)
            {
                int indexToPlaceACard = hand.CardsInHand.Count;
                CardLogic newCard = new CardLogic(deck.cards[0]);
                newCard.owner = this;
                hand.CardsInHand.Add(newCard);
                deck.cards.RemoveAt(0);
                new DrawACardCommand(hand.CardsInHand[indexToPlaceACard], this, indexToPlaceACard, fast, fromDeck: true).AddToQueue(); 
            }
        }
        else
        {
            // fatigue damage.
        }
       
    }

    public void DrawACoin()
    {
        if (hand.CardsInHand.Count < PArea.handVisual.slots.Children.Length)
        {
            CardLogic newCard = new CardLogic(GlobalSettings.Instance.CoinCard);
            newCard.owner = this;
            hand.CardsInHand.Add(newCard);
            new DrawACardCommand(hand.CardsInHand[hand.CardsInHand.Count - 1], this, hand.CardsInHand.Count - 1, fast: true, fromDeck: false).AddToQueue(); 
        }
    }

    public void PlayASpellFromHand(int SpellCardUniqueID, int TargetUniqueID)
    {
        if (TargetUniqueID < 0)
            PlayASpellFromHand(CardLogic.CardsCreatedThisGame[SpellCardUniqueID], null);
        else if (TargetUniqueID == ID)
        {
            PlayASpellFromHand(CardLogic.CardsCreatedThisGame[SpellCardUniqueID], this);
        }
        else if (TargetUniqueID == otherPlayer.ID)
        {
            PlayASpellFromHand(CardLogic.CardsCreatedThisGame[SpellCardUniqueID], this.otherPlayer);
        }
        else
        {
            PlayASpellFromHand(CardLogic.CardsCreatedThisGame[SpellCardUniqueID], CreatureLogic.CreaturesCreatedThisGame[TargetUniqueID]);
        }
          
    }

    public void PlayASpellFromHand(CardLogic playedCard, ICharacter target)
    {
        ManaLeft -= playedCard.CurrentManaCost;
        if (playedCard.effect != null)
            playedCard.effect.ActivateEffect(playedCard.ca.specialSpellAmount, target);
        else
        {
            Debug.LogWarning("No effect found on card " + playedCard.ca.name);
        }
        new PlayASpellCardCommand(this, playedCard).AddToQueue();
        hand.CardsInHand.Remove(playedCard);
    }

    public void PlayACreatureFromHand(int UniqueID, int tablePos)
    {
        PlayACreatureFromHand(CardLogic.CardsCreatedThisGame[UniqueID], tablePos);
    }

    public void PlayACreatureFromHand(CardLogic playedCard, int tablePos)
    {
        Debug.Log(ManaLeft);
        Debug.Log(playedCard.CurrentManaCost);
        ManaLeft -= playedCard.CurrentManaCost;
        Debug.Log("Mana Left after played a creature: " + ManaLeft);
        CreatureLogic newCreature = new CreatureLogic(this, playedCard.ca);
        table.CreaturesOnTable.Insert(tablePos, newCreature);
        new PlayACreatureCommand(playedCard, this, tablePos, newCreature.UniqueCreatureID).AddToQueue();
        hand.CardsInHand.Remove(playedCard);
        HighlightPlayableCards();
    }

    public void Die()
    {

        PArea.ControlsON = false;
        otherPlayer.PArea.ControlsON = false;
        TurnManager.Instance.StopTheTimer();
        new GameOverCommand(this).AddToQueue();
    }
    public void HighlightPlayableCards(bool removeAllHighlights = false)
    {
        foreach (CardLogic cl in hand.CardsInHand)
        {
            GameObject g = IDHolder.GetGameObjectWithID(cl.UniqueCardID);
            if (g!=null)
                g.GetComponent<OneCardManager>().CanBePlayedNow = (cl.CurrentManaCost <= ManaLeft) && !removeAllHighlights;
        }

        foreach (CreatureLogic crl in table.CreaturesOnTable)
        {
            GameObject g = IDHolder.GetGameObjectWithID(crl.UniqueCreatureID);
            if(g!= null)
                g.GetComponent<OneCreatureManager>().CanAttackNow = (crl.AttacksLeftThisTurn > 0) && !removeAllHighlights;
        }
        PArea.HeroPower.Highlighted = (!usedHeroPowerThisTurn) && (ManaLeft > 1) && !removeAllHighlights;
    }
    public void LoadCharacterInfoFromAsset()
    {
        Health = charAsset.MaxHealth;
        PArea.Portrait.charAsset = charAsset;
        PArea.Portrait.ApplyLookFromAsset();
        if (charAsset.HeroPowerName != null && charAsset.HeroPowerName != "")
        {
            HeroPowerEffect = System.Activator.CreateInstance(System.Type.GetType(charAsset.HeroPowerName)) as SpellEffect;
        }
        else
        {
            Debug.LogWarning("Check hero powr name for character " + charAsset.ClassName);
        }
    }

    public void TransmitInfoAboutPlayerToVisual()
    {
        PArea.Portrait.GetComponent<IDHolder>().UniqueID = PlayerID;
        if (GetComponent<TurnMaker>() is AITurnMaker)
        {
            PArea.AllowedToControlThisPlayer = false;
        }
        else
        {
            PArea.AllowedToControlThisPlayer = true;
        }
    }

    public void UseHeroPower()
    {
        ManaLeft -= 2;
        usedHeroPowerThisTurn = true;
        HeroPowerEffect.ActivateEffect();
    }
}
