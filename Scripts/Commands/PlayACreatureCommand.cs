using UnityEngine;
using System.Collections;

public class PlayACreatureCommand : Command
{
    private CardLogic cl;
    private int tablePos;
    private Player p;
    private int creatureID;

    public PlayACreatureCommand(CardLogic cl, Player p, int tablePos, int creatureID)
    {
        this.p = p;
        this.cl = cl;
        this.tablePos = tablePos;
        this.creatureID = creatureID;
    }

    public override void StartCommandExecution()
    {
        HandVisual PlayerHand = p.PArea.handVisual;
        GameObject card = IDHolder.GetGameObjectWithID(cl.UniqueCardID);
        PlayerHand.RemoveCard(card);
        GameObject.Destroy(card);
        HoverPreview.PreviewsAllowed = true;
        p.PArea.tableVisual.AddCreatureAtIndex(cl.ca, creatureID, tablePos);
    }
}
