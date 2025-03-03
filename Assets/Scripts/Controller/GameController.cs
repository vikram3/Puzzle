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
        private RuntimeLevelLoader levelLoader;

        private void Awake()
        {
            Debug.Log("GameController: Awake called");
            InitializeComponents();
            if (levelLoader != null) Debug.Log("GameController: levelLoader assigned in Awake");
            SubscribeToEvents();
        }

        private void InitializeComponents()
        {
            if (gameView == null)
            {
                Debug.LogError("GameController: GameView is not assigned in the Inspector!");
                return;
            }
            gameView.Initialize();

            levelLoader = GetComponent<RuntimeLevelLoader>();
            if (levelLoader == null)
            {
                Debug.LogWarning("GameController: RuntimeLevelLoader not found, adding it automatically");
                levelLoader = gameObject.AddComponent<RuntimeLevelLoader>();
            }
            Debug.Log("GameController: Components initialized");
        }

        private void SubscribeToEvents()
        {
            Debug.Log("GameController: Subscribing to events");
            gameView.OnSubmitAnswer += HandleSubmitAnswer;
            gameModel.OnLevelLoaded += HandleLevelLoaded;
            gameModel.OnScoreChanged += HandleScoreChanged;
            gameModel.OnAnswerSubmitted += HandleAnswerResult;
            gameModel.OnGameCompleted += HandleGameCompleted;
        }

        public void StartGame() // Around Line 50-56
        {
            Debug.Log("GameController: StartGame called");
            if (levelLoader == null)
            {
                Debug.LogError("GameController: LevelLoader is null in StartGame!"); // Line 55 or 56
                return;
            }
            List<LevelData> levels = levelLoader.LoadLevels(); // Line 56 or 57
            Debug.Log($"GameController: Loaded {levels.Count} levels");
            gameModel.LoadLevels(levels);
            LoadCurrentLevel();
        }

        private void LoadCurrentLevel()
        {
            Debug.Log("GameController: Loading current level");
            gameModel.GetCurrentLevel();
        }

        private void HandleLevelLoaded(LevelData level)
        {
            Debug.Log($"GameController: HandleLevelLoaded called with level ID: {level.levelId}");
            gameView.DisplayLevel(level, gameModel.GetCurrentLevelIndex() + 1);
        }

        private void HandleSubmitAnswer(List<string> selectedWords)
        {
            Debug.Log("GameController: HandleSubmitAnswer called");
            gameModel.SubmitAnswer(selectedWords);
        }

        private void HandleScoreChanged(int score)
        {
            Debug.Log($"GameController: Score changed to {score}");
            gameView.UpdateScoreDisplay(score);
        }

        private void HandleAnswerResult(bool isCorrect, List<string> correctWords)
        {
            Debug.Log($"GameController: Answer result - Correct: {isCorrect}");
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
            Debug.Log("GameController: Game completed");
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