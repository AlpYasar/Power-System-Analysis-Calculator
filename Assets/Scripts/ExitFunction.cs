using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitFunction : MonoBehaviour
{
    public void doExitGame() {
        Application.Quit();
    }
    
    public void loadlevel()
    {
        SceneManager.LoadScene("Pink");
 
    }
}
