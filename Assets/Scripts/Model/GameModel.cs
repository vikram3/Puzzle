using System;
using System.Collections.Generic;

namespace WordPuzzle.Model
{
    public class GameModel
    {
        private List<LevelData> allLevels = new List<LevelData>();
        private int currentLevelIndex = 0;
        private int playerScore = 0;

        // Events
        public event Action<LevelData> OnLevelLoaded;
        public event Action<int> OnScoreChanged;
        public event Action<bool, List<string>> OnAnswerSubmitted;
        public event Action OnGameCompleted;

        public void LoadLevels(List<LevelData> levels)
        {
            allLevels = levels;
            currentLevelIndex = 0;
            playerScore = 0;
        }

        public LevelData GetCurrentLevel()
        {
            if (currentLevelIndex < allLevels.Count)
            {
                var level = allLevels[currentLevelIndex];
                OnLevelLoaded?.Invoke(level);
                return level;
            }
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

            if (isCorrect)
            {
                playerScore += 10;
                OnScoreChanged?.Invoke(playerScore);
                currentLevelIndex++;

                if (currentLevelIndex >= allLevels.Count)
                {
                    OnGameCompleted?.Invoke();
                }
            }

            OnAnswerSubmitted?.Invoke(isCorrect, currentLevel.correctWords);
        }

        // Add this method to expose playerScore
        public int GetCurrentScore()
        {
            return playerScore;
        }

        // Optional: If you added this previously for level number
        public int GetCurrentLevelIndex() => currentLevelIndex;
    }
}