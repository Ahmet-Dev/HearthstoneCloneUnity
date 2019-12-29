﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragHeroPowerActions : DraggingActions {

    public TargetingOptions Options;

    public override void OnStartDrag()
    {
       
    }

    public override void OnDraggingInUpdate()
    {

    }

    public override void OnEndDrag()
    {

        if (DragSuccessful())
        {
            
        }
        else
        {
            
        } 
    }

    protected override bool DragSuccessful()
    {
        bool TableNotFull = (TurnManager.Instance.whoseTurn.table.CreaturesOnTable.Count < 8);

        return TableVisual.CursorOverSomeTable&& TableNotFull;
    }
}
