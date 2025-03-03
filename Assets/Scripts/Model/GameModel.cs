using System;
using System.Collections.Generic;

namespace WordPuzzle.Model
{
    public class GameModel
    {
        private List<LevelData> allLevels = new List<LevelData>();
        private int currentLevelIndex = 0;
        private int playerScore = 0;

        public event Action<LevelData> OnLevelLoaded;
        public event Action<int> OnScoreChanged;
        public event Action<bool, List<string>> OnAnswerSubmitted;
        public event Action OnGameCompleted;

        public void LoadLevels(List<LevelData> levels)
        {
            allLevels = levels;
            currentLevelIndex = 0;
            playerScore = 0;
            UnityEngine.Debug.Log($"GameModel: Loaded {allLevels.Count} levels");
        }

        public LevelData GetCurrentLevel()
        {
            if (currentLevelIndex < allLevels.Count)
            {
                var level = allLevels[currentLevelIndex];
                UnityEngine.Debug.Log($"GameModel: Loading level {currentLevelIndex + 1}: {level.levelId}");
                OnLevelLoaded?.Invoke(level);
                return level;
            }
            UnityEngine.Debug.LogWarning("GameModel: No more levels to load");
            return null;
        }

        public void SubmitAnswer(List<string> selectedWords)
        {
            if (currentLevelIndex >= allLevels.Count) return;

            var currentLevel = allLevels[currentLevelIndex];
            bool isCorrect = true;

            if (selectedWords.Count != currentLevel.correctWords.Count)
            {
                isCorrect = false;
            }
            else
            {
                foreach (var word in selectedWords)
                {
                    if (!currentLevel.correctWords.Contains(word))
                    {
                        isCorrect = false;
                        break;
                    }
                }
            }

            UnityEngine.Debug.Log($"GameModel: Answer submitted - Correct: {isCorrect}");
            if (isCorrect)
            {
                playerScore += 10;
                OnScoreChanged?.Invoke(playerScore);
                currentLevelIndex++;

                if (currentLevelIndex >= allLevels.Count)
                {
                    UnityEngine.Debug.Log("GameModel: All levels completed");
                    OnGameCompleted?.Invoke();
                }
            }

            OnAnswerSubmitted?.Invoke(isCorrect, currentLevel.correctWords);
        }

        public int GetCurrentScore() => playerScore;
        public int GetCurrentLevelIndex() => currentLevelIndex;
    }
}