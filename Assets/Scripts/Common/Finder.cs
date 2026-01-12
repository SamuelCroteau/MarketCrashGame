using UnityEngine;

public static class Finder
{
    private static AppController appController;
    public static AppController AppController
    {
        get
        {
            if (appController == null)
                appController = GetWithTag<AppController>("AppController");
            return appController;
        }
    }
    
    private static LevelManager levelManager;
    public static LevelManager LevelManager
    {
        get
        {
            if (levelManager == null)
                levelManager = GetWithTag<LevelManager>("LevelManager");
            return levelManager;
        }
    }

    private static Clock clock;
    public static Clock Clock
    {
        get
        {
            if (clock == null)
                clock = GetWithTag<Clock>("LevelManager");
            return clock;
        }
    }

    private static PlayerCharacter player;
    public static PlayerCharacter Player
    {
        get
        {
            if (player == null)
                player = GetWithTag<PlayerCharacter>("Player");
            return player;
        }
    }

    private static SongManager songManager;
    public static SongManager SongManager
    {
        get
        {
            if (songManager == null)
                songManager = GetWithTag<SongManager>("SongManager");
            return songManager;
        }
    }

    // Probablement dans les autres exercices, mais fait avec l'IA pour là... pas le temps #lol
    private static T GetWithTag<T>(string tag) where T : Component
    {
        var go = GameObject.FindWithTag(tag);
        if (go == null)
        {
            Debug.LogWarning($"Finder: no GameObject found with tag '{tag}'.");
            return null;
        }

        var comp = go.GetComponent<T>();
        if (comp == null)
            Debug.LogWarning($"Finder: GameObject with tag '{tag}' does not have component '{typeof(T).Name}'.");

        return comp;
    }
}