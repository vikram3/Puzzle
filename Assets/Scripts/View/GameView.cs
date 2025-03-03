using System.Collections.Generic;
using UnityEngine;
using WordPuzzle.Model;

namespace WordPuzzle.View
{
    public class GameView : MonoBehaviour
    {
        [SerializeField] private GameObject wordButtonPrefab;
        [SerializeField] private Transform wordContainer;
        [SerializeField] private Transform selectedWordsContainer;
        [SerializeField] private Animator sceneAnimator;
        [SerializeField] private SpriteRenderer problemImageRenderer;
        [SerializeField] private TMPro.TextMeshProUGUI scoreText;
        [SerializeField] private GameObject gameCompletedPanel;

        private List<GameObject> wordButtons = new List<GameObject>();
        private List<string> selectedWords = new List<string>();

        public event System.Action<List<string>> OnSubmitAnswer;

        public void Initialize()
        {
            ClearWordOptions();
            UpdateScoreDisplay(0);
            gameCompletedPanel.SetActive(false);
        }

        public void DisplayLevel(LevelData level)
        {
            ClearWordOptions();
            selectedWords.Clear();

            problemImageRenderer.sprite = level.problemImage;

            if (level.problemAnimation != null)
            {
                PlayAnimation("Problem", level.problemAnimation);
            }

            foreach (var word in level.wordOptions)
            {
                CreateWordButton(word);
            }
        }

        private void CreateWordButton(string word)
        {
            GameObject button = Instantiate(wordButtonPrefab, wordContainer);
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = word;
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SelectWord(word, button));
            wordButtons.Add(button);
        }

        private void SelectWord(string word, GameObject button)
        {
            selectedWords.Add(word);
            button.SetActive(false);
            UpdateSelectedWordsDisplay();

            if (selectedWords.Count == 1)
            {
                SubmitAnswer();
            }
        }

        private void UpdateSelectedWordsDisplay()
        {
            foreach (Transform child in selectedWordsContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var word in selectedWords)
            {
                GameObject label = Instantiate(wordButtonPrefab, selectedWordsContainer);
                label.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = word;
                label.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => UnselectWord(word, label));
            }
        }

        private void UnselectWord(string word, GameObject label)
        {
            selectedWords.Remove(word);
            Destroy(label);

            foreach (var button in wordButtons)
            {
                if (button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text == word)
                {
                    button.SetActive(true);
                    break;
                }
            }

            UpdateSelectedWordsDisplay();
        }

        public void SubmitAnswer()
        {
            OnSubmitAnswer?.Invoke(selectedWords);
        }

        public void ShowResult(bool isCorrect, AnimationClip correctAnim, AnimationClip incorrectAnim)
        {
            if (isCorrect)
            {
                PlayAnimation("Correct", correctAnim);
            }
            else
            {
                PlayAnimation("Incorrect", incorrectAnim);
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

        private void ClearWordOptions()
        {
            foreach (var button in wordButtons)
            {
                Destroy(button);
            }
            wordButtons.Clear();

            foreach (Transform child in selectedWordsContainer)
            {
                Destroy(child.gameObject);
            }
        }

        public void UpdateScoreDisplay(int score)
        {
            scoreText.text = $"Score: {score}";
        }

        public void ShowGameCompleted()
        {
            gameCompletedPanel.SetActive(true);
        }
    }
}