using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnCreditsButton()
    {
        Application.OpenURL("https://konradmampe.itch.io/brick-and-roll");
    }

    public void OnExitButton()
    {
        Application.Quit();
    }
}
