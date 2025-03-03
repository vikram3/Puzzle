using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WordPuzzle.Model;
using System.Collections;

namespace WordPuzzle.View
{
    public class WordPuzzleUIView : MonoBehaviour, IGameView
    {
        [Header("UI References")]
        [SerializeField] private GameObject wordButtonPrefab;
        [SerializeField] private RectTransform wordOptionsContainer;
        [SerializeField] private RectTransform selectedWordsContainer;
        [SerializeField] private Image scenarioImage;
        [SerializeField] private Animator sceneAnimator;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private GameObject gameCompletedPanel;
        [SerializeField] private Button submitButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextLevelButton;

        [Header("UI Settings")]
        [SerializeField] private Color selectedWordBackgroundColor = new Color(0.3f, 0.7f, 0.9f);
        [SerializeField] private Color defaultWordBackgroundColor = new Color(0.2f, 0.2f, 0.2f);
        [SerializeField] private float wordButtonSpacing = 10f;
        [SerializeField] private float selectedWordButtonSpacing = 10f;

        public event System.Action<List<string>> OnSubmitAnswer;
        public event System.Action OnNextLevel;
        public event System.Action OnRestartGame;

        private List<GameObject> wordOptionButtons = new List<GameObject>();
        private List<GameObject> selectedWordButtons = new List<GameObject>();
        private List<string> selectedWords = new List<string>();
        private LevelData currentLevel;
        private bool isMultiWordPuzzle = false;
        private int currentLevelNumber = 1;

        private static readonly string ANIM_TRIGGER_PROBLEM = "Problem";
        private static readonly string ANIM_TRIGGER_CORRECT = "Correct";
        private static readonly string ANIM_TRIGGER_INCORRECT = "Incorrect";

        public void Initialize()
        {
            ClearWordButtons();
            UpdateScoreDisplay(0);
            gameCompletedPanel.SetActive(false);

            submitButton.onClick.AddListener(() => SubmitAnswer());
            restartButton.onClick.AddListener(() => OnRestartGame?.Invoke());
            nextLevelButton.onClick.AddListener(() => OnNextLevel?.Invoke());

            nextLevelButton.gameObject.SetActive(false);
        }

        public void DisplayLevel(LevelData level, int levelNumber)
        {
            currentLevel = level;
            currentLevelNumber = levelNumber;

            ClearWordButtons();
            selectedWords.Clear();

            if (level.problemImage != null)
            {
                scenarioImage.sprite = level.problemImage;
                scenarioImage.enabled = true;
            }
            else
            {
                scenarioImage.enabled = false;
            }

            levelText.text = $"Level {levelNumber}: {level.levelId}";

            if (level.problemAnimation != null)
            {
                PlayAnimation(ANIM_TRIGGER_PROBLEM, level.problemAnimation);
            }

            isMultiWordPuzzle = level.correctWords.Count > 1;
            submitButton.gameObject.SetActive(isMultiWordPuzzle);

            CreateWordOptionButtons(level.wordOptions);

            nextLevelButton.gameObject.SetActive(false);
        }

        private void CreateWordOptionButtons(List<string> wordOptions)
        {
            List<string> shuffledOptions = new List<string>(wordOptions);
            ShuffleList(shuffledOptions);

            for (int i = 0; i < shuffledOptions.Count; i++)
            {
                CreateWordOptionButton(shuffledOptions[i]);
            }

            LayoutWordOptionButtons();
        }

        private void CreateWordOptionButton(string word)
        {
            GameObject buttonObj = Instantiate(wordButtonPrefab, wordOptionsContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI textComponent = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            Image backgroundImage = buttonObj.GetComponent<Image>();

            textComponent.text = word;
            backgroundImage.color = defaultWordBackgroundColor;

            button.onClick.AddListener(() => SelectWord(word, buttonObj));

            wordOptionButtons.Add(buttonObj);
        }

        private void SelectWord(string word, GameObject buttonObj)
        {
            selectedWords.Add(word);
            buttonObj.SetActive(false);

            GameObject selectedButton = Instantiate(wordButtonPrefab, selectedWordsContainer);
            Button button = selectedButton.GetComponent<Button>();
            TextMeshProUGUI textComponent = selectedButton.GetComponentInChildren<TextMeshProUGUI>();
            Image backgroundImage = selectedButton.GetComponent<Image>();

            textComponent.text = word;
            backgroundImage.color = selectedWordBackgroundColor;

            button.onClick.AddListener(() => UnselectWord(word, buttonObj, selectedButton));

            selectedWordButtons.Add(selectedButton);

            if (!isMultiWordPuzzle)
            {
                SubmitAnswer();
            }
            else
            {
                LayoutSelectedWordButtons();
            }
        }

        private void UnselectWord(string word, GameObject originalButton, GameObject selectedButton)
        {
            selectedWords.Remove(word);
            originalButton.SetActive(true);
            selectedWordButtons.Remove(selectedButton);
            Destroy(selectedButton);
            LayoutSelectedWordButtons();
        }

        private void LayoutWordOptionButtons()
        {
            float x = 0;
            float y = 0;
            float maxWidth = wordOptionsContainer.rect.width;
            float rowHeight = 0;

            for (int i = 0; i < wordOptionButtons.Count; i++)
            {
                RectTransform rectTransform = wordOptionButtons[i].GetComponent<RectTransform>();
                float buttonWidth = rectTransform.rect.width;
                float buttonHeight = rectTransform.rect.height;

                if (x + buttonWidth > maxWidth && x > 0)
                {
                    x = 0;
                    y -= rowHeight + wordButtonSpacing;
                    rowHeight = 0;
                }

                rectTransform.anchoredPosition = new Vector2(x, y);

                x += buttonWidth + wordButtonSpacing;
                rowHeight = Mathf.Max(rowHeight, buttonHeight);
            }
        }

        private void LayoutSelectedWordButtons()
        {
            float x = 0;

            for (int i = 0; i < selectedWordButtons.Count; i++)
            {
                RectTransform rectTransform = selectedWordButtons[i].GetComponent<RectTransform>();
                float buttonWidth = rectTransform.rect.width;

                rectTransform.anchoredPosition = new Vector2(x, 0);

                x += buttonWidth + selectedWordButtonSpacing;
            }
        }

        public void SubmitAnswer()
        {
            OnSubmitAnswer?.Invoke(selectedWords);
        }

        public void ShowResult(bool isCorrect)
        {
            if (isCorrect)
            {
                if (currentLevel.correctAnimation != null)
                {
                    PlayAnimation(ANIM_TRIGGER_CORRECT, currentLevel.correctAnimation);
                }
                StartCoroutine(ShowNextLevelButton(1.5f));
            }
            else
            {
                if (currentLevel.incorrectAnimation != null)
                {
                    PlayAnimation(ANIM_TRIGGER_INCORRECT, currentLevel.incorrectAnimation);
                }
                RestoreWordOptions();
            }

            if (isMultiWordPuzzle)
            {
                submitButton.interactable = selectedWords.Count > 0;
            }
        }

        private IEnumerator ShowNextLevelButton(float delay)
        {
            yield return new WaitForSeconds(delay);
            nextLevelButton.gameObject.SetActive(true);
        }

        private void RestoreWordOptions()
        {
            foreach (var selectedButton in selectedWordButtons)
            {
                Destroy(selectedButton);
            }
            selectedWordButtons.Clear();
            selectedWords.Clear();

            foreach (var button in wordOptionButtons)
            {
                button.SetActive(true);
            }
        }

        private void PlayAnimation(string triggerName, AnimationClip clip)
        {
            if (clip != null && sceneAnimator != null)
            {
                var overrideController = new AnimatorOverrideController(sceneAnimator.runtimeAnimatorController);
                overrideController[triggerName] = clip;
                sceneAnimator.runtimeAnimatorController = overrideController;
                sceneAnimator.SetTrigger(triggerName);
            }
        }

        private void ClearWordButtons()
        {
            foreach (var button in wordOptionButtons)
            {
                Destroy(button);
            }
            wordOptionButtons.Clear();

            foreach (var button in selectedWordButtons)
            {
                Destroy(button);
            }
            selectedWordButtons.Clear();
        }

        public void UpdateScoreDisplay(int score)
        {
            scoreText.text = $"Score: {score}";
        }

        public void ShowGameCompleted(int finalScore)
        {
            gameCompletedPanel.SetActive(true);
            gameCompletedPanel.GetComponentInChildren<TextMeshProUGUI>().text =
                $"Congratulations!\nYou've completed all levels.\nFinal Score: {finalScore}";
        }

        private void ShuffleList<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void OnDestroy()
        {
            submitButton.onClick.RemoveAllListeners();
            restartButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.RemoveAllListeners();
        }
    }

    public interface IGameView
    {
        event System.Action<List<string>> OnSubmitAnswer;
        event System.Action OnNextLevel;
        event System.Action OnRestartGame;

        void Initialize();
        void DisplayLevel(LevelData level, int levelNumber);
        void ShowResult(bool isCorrect);
        void UpdateScoreDisplay(int score);
        void ShowGameCompleted(int finalScore);
    }
}