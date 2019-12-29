using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class TurnManager : MonoBehaviour {

    private RopeTimer timer;
    public static TurnManager Instance;
    private Player _whoseTurn;
    public Player whoseTurn
    {
        get
        {
            return _whoseTurn;
        }

        set
        {
            _whoseTurn = value;
            timer.StartTimer();
            GlobalSettings.Instance.EnableEndTurnButtonOnStart(_whoseTurn);
            TurnMaker tm = whoseTurn.GetComponent<TurnMaker>();
            tm.OnTurnStart();
            if (tm is PlayerTurnMaker)
            {
                whoseTurn.HighlightPlayableCards();
            }
            whoseTurn.otherPlayer.HighlightPlayableCards(true);               
        }
    }

    void Awake()
    {
        Instance = this;
        timer = GetComponent<RopeTimer>();
    }

    void Start()
    {
        OnGameStart();
    }

    public void OnGameStart()
    {
        CardLogic.CardsCreatedThisGame.Clear();
        CreatureLogic.CreaturesCreatedThisGame.Clear();
        foreach (Player p in Player.Players)
        {
            p.ManaThisTurn = 0;
            p.ManaLeft = 0;
            p.LoadCharacterInfoFromAsset();
            p.TransmitInfoAboutPlayerToVisual();
            p.PArea.PDeck.CardsInDeck = p.deck.cards.Count;
            p.PArea.Portrait.transform.position = p.PArea.handVisual.OtherCardDrawSourceTransform.position;
        }

        Sequence s = DOTween.Sequence();
        s.Append(Player.Players[0].PArea.Portrait.transform.DOMove(Player.Players[0].PArea.PortraitPosition.position, 1f).SetEase(Ease.InQuad));
        s.Insert(0f, Player.Players[1].PArea.Portrait.transform.DOMove(Player.Players[1].PArea.PortraitPosition.position, 1f).SetEase(Ease.InQuad));
        s.PrependInterval(3f);
        s.OnComplete(() =>
            {
                int rnd = Random.Range(0,2); 
                Debug.Log(Player.Players.Length);
                Player whoGoesFirst = Player.Players[rnd];
                Debug.Log(whoGoesFirst);
                Player whoGoesSecond = whoGoesFirst.otherPlayer;
                Debug.Log(whoGoesSecond);
                int initDraw = 4;
                for (int i = 0; i < initDraw; i++)
                {            
                    whoGoesSecond.DrawACard(true);
                    whoGoesFirst.DrawACard(true);
                }
                whoGoesSecond.DrawACard(true);
                whoGoesSecond.DrawACoin();
                new StartATurnCommand(whoGoesFirst).AddToQueue();
            });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            EndTurn();
    }

    public void EndTurn()
    {
        timer.StopTimer();
        whoseTurn.OnTurnEnd();

        new StartATurnCommand(whoseTurn.otherPlayer).AddToQueue();
    }

    public void StopTheTimer()
    {
        timer.StopTimer();
    }

}

