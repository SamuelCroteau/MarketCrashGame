using UnityEngine;

public class LevelSelectorController : MonoBehaviour
{
    public void LoadLevel1()
    {
        Finder.AppController.LoadLevel("Level01Scene");
    }
    
    public void LoadLevel2()
    {
        Finder.AppController.LoadLevel("Level02Scene");
    }
    
    public void LoadLevel3()
    {
        Finder.AppController.LoadLevel("Level03Scene");
    }

    public void ReturnMainMenu()
    {
        Finder.AppController.LoadMainMenu();
    }
}