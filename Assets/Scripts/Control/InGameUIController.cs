using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    public void OnReturnButtonClicked()
    {
        Finder.AppController.LoadMainMenu();
    }
}