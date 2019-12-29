using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragSpellNoTarget: DraggingActions{

    private int savedHandSlot;
    private WhereIsTheCardOrCreature whereIsCard;

    void Awake()
    {
        whereIsCard = GetComponent<WhereIsTheCardOrCreature>();
    }

    public override void OnStartDrag()
    {
        savedHandSlot = whereIsCard.Slot;

        whereIsCard.VisualState = VisualStates.Dragging;
        whereIsCard.BringToFront();

    }

    public override void OnDraggingInUpdate()
    {
        
    }

    public override void OnEndDrag()
    {
        if (DragSuccessful())
        {
            playerOwner.PlayASpellFromHand(GetComponent<IDHolder>().UniqueID, -1);
        }
        else
        {
            whereIsCard.Slot = savedHandSlot;
            whereIsCard.VisualState = VisualStates.LowHand;
            HandVisual PlayerHand = TurnManager.Instance.whoseTurn.PArea.handVisual;
            Vector3 oldCardPos = PlayerHand.slots.Children[savedHandSlot].transform.localPosition;
            transform.DOLocalMove(oldCardPos, 1f);
        } 
    }

    protected override bool DragSuccessful()
    {
        return TableVisual.CursorOverSomeTable; 
    }


}
