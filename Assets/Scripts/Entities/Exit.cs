using UnityEngine;

public class Exit : MonoBehaviour, IBreakable
{
    private bool _isBroken = false;

    public bool IsBroken
    { 
        get { return _isBroken; } 
        set { _isBroken = value; } 
    }

    public void Break()
    {
        Finder.LevelManager.EndGame();
    }
}
