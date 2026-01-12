using UnityEngine;

public class CreditsController : MonoBehaviour
{
    public void OnReturnButtonClicked()
    {
        Finder.AppController.LoadMainMenu();
    }
}