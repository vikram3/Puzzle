using UnityEngine;
using WordPuzzle.Controller;

namespace WordPuzzle
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameController gameController;

        private void Start() // Line 12
        {
            StartGame();
        }

        public void StartGame() // Line 17
        {
            gameController.StartGame();
        }

        public void RestartGame()
        {
            StartGame();
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}