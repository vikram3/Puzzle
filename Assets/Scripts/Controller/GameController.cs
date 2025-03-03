using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WordPuzzle.Model;
using WordPuzzle.View;

namespace WordPuzzle.Controller
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private WordPuzzleUIView gameView;
        private GameModel gameModel = new GameModel();
        private RuntimeLevelLoader levelLoader; // Changed from LevelLoader to RuntimeLevelLoader

        private void Awake()
        {
            InitializeComponents();
            SubscribeToEvents();
        }

        private void InitializeComponents()
        {
            if (gameView == null)
            {
                Debug.LogError("GameView is not assigned in the Inspector!");
                return;
            }
            gameView.Initialize();
            levelLoader = GetComponent<RuntimeLevelLoader>(); // Updated to RuntimeLevelLoader
            if (levelLoader == null)
            {
                Debug.LogError("RuntimeLevelLoader component not found on this GameObject!");
                return;
            }
        }

        private void SubscribeToEvents()
        {
            gameView.OnSubmitAnswer += HandleSubmitAnswer;
            gameModel.OnLevelLoaded += HandleLevelLoaded;
            gameModel.OnScoreChanged += HandleScoreChanged;
            gameModel.OnAnswerSubmitted += HandleAnswerResult;
            gameModel.OnGameCompleted += HandleGameCompleted;
        }

        public void StartGame()
        {
            if (levelLoader == null)
            {
                Debug.LogError("LevelLoader is null! Please ensure it’s attached to the GameController GameObject.");
                return;
            }
            List<LevelData> levels = levelLoader.LoadLevels();
            gameModel.LoadLevels(levels);
            LoadCurrentLevel();
        }

        private void LoadCurrentLevel()
        {
            gameModel.GetCurrentLevel();
        }

        private void HandleLevelLoaded(LevelData level)
        {
            gameView.DisplayLevel(level, gameModel.GetCurrentLevelIndex() + 1);
        }

        private void HandleSubmitAnswer(List<string> selectedWords)
        {
            gameModel.SubmitAnswer(selectedWords);
        }

        private void HandleScoreChanged(int score)
        {
            gameView.UpdateScoreDisplay(score);
        }

        private void HandleAnswerResult(bool isCorrect, List<string> correctWords)
        {
            var currentLevel = gameModel.GetCurrentLevel();
            gameView.ShowResult(isCorrect);

            if (isCorrect)
            {
                StartCoroutine(LoadNextLevelAfterDelay(2.0f));
            }
        }

        private IEnumerator LoadNextLevelAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            LoadCurrentLevel();
        }

        private void HandleGameCompleted()
        {
            gameView.ShowGameCompleted(gameModel.GetCurrentScore());
        }

        private void OnDestroy()
        {
            gameView.OnSubmitAnswer -= HandleSubmitAnswer;
            gameModel.OnLevelLoaded -= HandleLevelLoaded;
            gameModel.OnScoreChanged -= HandleScoreChanged;
            gameModel.OnAnswerSubmitted -= HandleAnswerResult;
            gameModel.OnGameCompleted -= HandleGameCompleted;
        }
    }
}