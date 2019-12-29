using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Command
{
    public static Queue<Command> CommandQueue = new Queue<Command>();
    public static bool playingQueue = false;

    public virtual void AddToQueue()
    {
        CommandQueue.Enqueue(this);
        if (!playingQueue)
            PlayFirstCommandFromQueue();
    }

    public virtual void StartCommandExecution()
    {

    }

    public static void CommandExecutionComplete()
    {
        if (CommandQueue.Count > 0)
            PlayFirstCommandFromQueue();
        else
            playingQueue = false;
        if (TurnManager.Instance.whoseTurn != null)
            TurnManager.Instance.whoseTurn.HighlightPlayableCards();
    }

    public static void PlayFirstCommandFromQueue()
    {
        playingQueue = true;
        CommandQueue.Dequeue().StartCommandExecution();
    }

    public static bool CardDrawPending()
    {
        foreach (Command c in CommandQueue)
        {
            if (c is DrawACardCommand)
                return true;
        }
        return false;
    }
}
