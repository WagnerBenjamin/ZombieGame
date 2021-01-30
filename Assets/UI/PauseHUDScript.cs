using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHUDScript : MonoBehaviour
{
    GameControllerScript gcs;

    // Start is called before the first frame update
    void Start()
    {
        gcs = (GameControllerScript)FindObjectOfType(typeof(GameControllerScript));
    }

    public void UnPause()
    {
        gcs.PauseControl();
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
