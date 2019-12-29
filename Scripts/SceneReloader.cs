using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneReloader: MonoBehaviour {

    public void ReloadScene()
    {
        Debug.Log("Scene reloaded");
        IDFactory.ResetIDs();
        IDHolder.ClearIDHoldersList();
        Command.CommandQueue.Clear();
        Command.CommandExecutionComplete();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
