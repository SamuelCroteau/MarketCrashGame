using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
        Finder.AppController.LoadFirstLevel();
    }

    public void OnLevelSelectorButtonClicked()
    {
        Finder.AppController.LoadLevelSelector();
    }
    
    public void OnExitButtonClicked()
    {
        Finder.AppController.Exit();
    }
}