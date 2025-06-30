using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeatSaber.Runtime.Game
{
    public class GameManager
    {
        public static GameSessionSO gameSession { get; private set; }

        [RuntimeInitializeOnLoadMethod]
        public static void Bootstrap()
        {
            SceneManager.LoadScene("MetaGame");
            gameSession = ScriptableObject.CreateInstance<GameSessionSO>();
        }
    }

}
