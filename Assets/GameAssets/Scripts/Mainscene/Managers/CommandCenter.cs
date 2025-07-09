using UnityEngine;
[System.Serializable]
public enum GameMode
{
    Demo,
    Live,
}
public class CommandCenter : MonoBehaviour
{
    public static CommandCenter Instance { get; private set; }
    public GameMode gameMode; // Live or Demo
    public PoolManager poolManager_;
    public SettingsManager settingsManager_;
    public SoundManager soundManager_;
    public TextManager textManager_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
