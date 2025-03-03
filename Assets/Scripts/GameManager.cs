using UnityEngine;
using WordPuzzle.Controller;

namespace WordPuzzle
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameController gameController;

        private void Start()
        {
            Debug.Log("GameManager: Start called");
            StartGame();
        }

        public void StartGame()
        {
            Debug.Log("GameManager: StartGame called");
            if (gameController == null)
            {
                Debug.LogError("GameManager: GameController is null!");
                return;
            }
            gameController.StartGame();
        }
    }
}