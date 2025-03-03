using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WordPuzzle.Model;
using System.Collections;

namespace WordPuzzle.View
{
    public class WordPuzzleUIView : MonoBehaviour
    {
        [SerializeField] private GameObject wordButtonPrefab;
        [SerializeField] private RectTransform wordOptionsContainer;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Button submitButton;
        [SerializeField] private Image scenarioImage;

        public event System.Action<List<string>> OnSubmitAnswer;

        private List<GameObject> wordOptionButtons = new List<GameObject>();
        private List<string> selectedWords = new List<string>();
        private LevelData currentLevel;
        private bool isMultiWordPuzzle = false;

        public void Initialize()
        {
            Debug.Log("WordPuzzleUIView: Initialize called");
            ClearWordButtons();
            UpdateScoreDisplay(0);
            submitButton.onClick.AddListener(SubmitAnswer);
        }

        public void DisplayLevel(LevelData level, int levelNumber)
        {
            Debug.Log($"WordPuzzleUIView: Displaying level {levelNumber}: {level.levelId} with {level.wordOptions.Count} options");
            currentLevel = level;
            ClearWordButtons();
            selectedWords.Clear();

            if (level.problemImage != null)
            {
                scenarioImage.sprite = level.problemImage;
                scenarioImage.enabled = true;
                Debug.Log($"WordPuzzleUIView: Set problem image for {level.levelId}");
            }
            else
            {
                scenarioImage.enabled = false;
                Debug.LogWarning("WordPuzzleUIView: No problem image available");
            }

            levelText.text = $"Level {levelNumber}: {level.levelId}";
            isMultiWordPuzzle = level.correctWords.Count > 1;
            submitButton.gameObject.SetActive(isMultiWordPuzzle);

            CreateWordOptionButtons(level.wordOptions);
        }

        private void CreateWordOptionButtons(List<string> wordOptions)
        {
            Debug.Log($"WordPuzzleUIView: Creating {wordOptions.Count} word buttons");
            foreach (var word in wordOptions)
            {
                CreateWordOptionButton(word);
            }
        }

        private void CreateWordOptionButton(string word)
        {
            Debug.Log($"WordPuzzleUIView: Creating button for {word}");
            GameObject buttonObj = Instantiate(wordButtonPrefab, wordOptionsContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI textComponent = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            textComponent.text = word;
            button.onClick.AddListener(() => SelectWord(word, buttonObj));
            wordOptionButtons.Add(buttonObj);
        }

        private void SelectWord(string word, GameObject buttonObj)
        {
            Debug.Log($"WordPuzzleUIView: Selected word: {word}");
            selectedWords.Add(word);
            buttonObj.SetActive(false);

            if (!isMultiWordPuzzle)
            {
                SubmitAnswer();
            }
        }

        public void SubmitAnswer()
        {
            Debug.Log("WordPuzzleUIView: Submitting answer");
            OnSubmitAnswer?.Invoke(selectedWords);
        }

        public void ShowResult(bool isCorrect)
        {
            Debug.Log($"WordPuzzleUIView: Showing result - Correct: {isCorrect}");
            if (isCorrect)
            {
                StartCoroutine(ClearAfterDelay(2.0f));
            }
            else
            {
                RestoreWordOptions();
            }
        }

        private IEnumerator ClearAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ClearWordButtons();
        }

        private void RestoreWordOptions()
        {
            selectedWords.Clear();
            foreach (var button in wordOptionButtons)
            {
                button.SetActive(true);
            }
        }

        private void ClearWordButtons()
        {
            Debug.Log("WordPuzzleUIView: Clearing word buttons");
            foreach (var button in wordOptionButtons)
            {
                Destroy(button);
            }
            wordOptionButtons.Clear();
        }

        public void UpdateScoreDisplay(int score)
        {
            scoreText.text = $"Score: {score}";
        }

        public void ShowGameCompleted(int finalScore)
        {
            Debug.Log($"WordPuzzleUIView: Game completed with score: {finalScore}");
            levelText.text = $"Game Over! Final Score: {finalScore}";
        }

        private void OnDestroy()
        {
            submitButton.onClick.RemoveAllListeners();
        }
    }
}