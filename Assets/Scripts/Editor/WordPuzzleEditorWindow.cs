#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using WordPuzzle.Model;
using WordPuzzle.Factory;
using System.IO;
using System.Linq;

namespace WordPuzzle.Editor
{
    public class WordPuzzleEditorWindow : EditorWindow
    {
        private List<LevelData> levels = new List<LevelData>();
        private LevelFactory levelFactory = new LevelFactory();
        private Vector2 scrollPosition;
        private int selectedLevelIndex = -1;
        private string exportPath = "Assets/Resources/LevelData.json";

        private string newLevelId = "level_";
        private Sprite newProblemImage;
        private AnimationClip newProblemAnimation;
        private AnimationClip newCorrectAnimation;
        private AnimationClip newIncorrectAnimation;
        private string newWordOption = "";
        private List<string> newWordOptions = new List<string>();
        private List<string> newCorrectWords = new List<string>();
        // Removed: private int selectedWordIndex = -1;
        // Removed: private int selectedCorrectWordIndex = -1;

        [MenuItem("Tools/Word Puzzle Editor")]
        public static void ShowWindow()
        {
            GetWindow<WordPuzzleEditorWindow>("Word Puzzle Editor");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            DrawLevelList();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            if (selectedLevelIndex >= 0 && selectedLevelIndex < levels.Count)
            {
                DrawLevelDetails(levels[selectedLevelIndex]);
            }
            else
            {
                DrawNewLevelForm();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            DrawBottomControls();
        }

        private void DrawLevelList()
        {
            EditorGUILayout.LabelField("Levels", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(200), GUILayout.Height(300));
            for (int i = 0; i < levels.Count; i++)
            {
                if (GUILayout.Toggle(selectedLevelIndex == i, $"Level {i + 1}: {levels[i].levelId}", "Button"))
                {
                    selectedLevelIndex = i;
                }
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("+ Add New Level"))
            {
                selectedLevelIndex = -1;
                ClearNewLevelForm();
            }

            if (selectedLevelIndex >= 0 && GUILayout.Button("Delete Selected Level"))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete", "Are you sure you want to delete this level?", "Yes", "No"))
                {
                    levels.RemoveAt(selectedLevelIndex);
                    selectedLevelIndex = -1;
                }
            }
        }

        private void DrawLevelDetails(LevelData level)
        {
            EditorGUILayout.LabelField($"Editing Level: {level.levelId}", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            level.levelId = EditorGUILayout.TextField("Level ID:", level.levelId);
            level.problemImage = (Sprite)EditorGUILayout.ObjectField("Problem Image:", level.problemImage, typeof(Sprite), false);
            level.problemAnimation = (AnimationClip)EditorGUILayout.ObjectField("Problem Animation:", level.problemAnimation, typeof(AnimationClip), false);
            level.correctAnimation = (AnimationClip)EditorGUILayout.ObjectField("Correct Animation:", level.correctAnimation, typeof(AnimationClip), false);
            level.incorrectAnimation = (AnimationClip)EditorGUILayout.ObjectField("Incorrect Animation:", level.incorrectAnimation, typeof(AnimationClip), false);
            EditorGUI.EndChangeCheck();

            DrawWordOptionsEditor(level);
            DrawCorrectWordsEditor(level);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preview (Not available in editor window)", EditorStyles.miniLabel);
        }

        private void DrawWordOptionsEditor(LevelData level)
        {
            EditorGUILayout.LabelField("Word Options:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            newWordOption = EditorGUILayout.TextField("New Word:", newWordOption);
            if (GUILayout.Button("Add", GUILayout.Width(80)) && !string.IsNullOrWhiteSpace(newWordOption))
            {
                level.wordOptions.Add(newWordOption);
                newWordOption = "";
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            for (int i = 0; i < level.wordOptions.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                level.wordOptions[i] = EditorGUILayout.TextField($"Word {i + 1}:", level.wordOptions[i]);
                if (GUILayout.Button("×", GUILayout.Width(30)))
                {
                    level.wordOptions.RemoveAt(i);
                    level.correctWords.Remove(level.wordOptions[i]);
                    break;
                }
                if (GUILayout.Button("→", GUILayout.Width(30)) && !level.correctWords.Contains(level.wordOptions[i]))
                {
                    level.correctWords.Add(level.wordOptions[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawCorrectWordsEditor(LevelData level)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Correct Words (Solution):", EditorStyles.boldLabel);
            for (int i = 0; i < level.correctWords.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Word {i + 1}:", level.correctWords[i]);
                if (GUILayout.Button("×", GUILayout.Width(30)))
                {
                    level.correctWords.RemoveAt(i);
                    break;
                }
                if (i > 0 && GUILayout.Button("↑", GUILayout.Width(30)))
                {
                    string temp = level.correctWords[i];
                    level.correctWords[i] = level.correctWords[i - 1];
                    level.correctWords[i - 1] = temp;
                }
                if (i < level.correctWords.Count - 1 && GUILayout.Button("↓", GUILayout.Width(30)))
                {
                    string temp = level.correctWords[i];
                    level.correctWords[i] = level.correctWords[i + 1];
                    level.correctWords[i + 1] = temp;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawNewLevelForm()
        {
            EditorGUILayout.LabelField("Create New Level", EditorStyles.boldLabel);
            newLevelId = EditorGUILayout.TextField("Level ID:", newLevelId);
            newProblemImage = (Sprite)EditorGUILayout.ObjectField("Problem Image:", newProblemImage, typeof(Sprite), false);
            newProblemAnimation = (AnimationClip)EditorGUILayout.ObjectField("Problem Animation:", newProblemAnimation, typeof(AnimationClip), false);
            newCorrectAnimation = (AnimationClip)EditorGUILayout.ObjectField("Correct Animation:", newCorrectAnimation, typeof(AnimationClip), false);
            newIncorrectAnimation = (AnimationClip)EditorGUILayout.ObjectField("Incorrect Animation:", newIncorrectAnimation, typeof(AnimationClip), false);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Word Options:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            newWordOption = EditorGUILayout.TextField("New Word:", newWordOption);
            if (GUILayout.Button("Add", GUILayout.Width(80)) && !string.IsNullOrWhiteSpace(newWordOption))
            {
                newWordOptions.Add(newWordOption);
                newWordOption = "";
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            for (int i = 0; i < newWordOptions.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                newWordOptions[i] = EditorGUILayout.TextField($"Word {i + 1}:", newWordOptions[i]);
                if (GUILayout.Button("×", GUILayout.Width(30)))
                {
                    newWordOptions.RemoveAt(i);
                    newCorrectWords.Remove(newWordOptions[i]);
                    break;
                }
                if (GUILayout.Button("→", GUILayout.Width(30)) && !newCorrectWords.Contains(newWordOptions[i]))
                {
                    newCorrectWords.Add(newWordOptions[i]);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Correct Words (Solution):", EditorStyles.boldLabel);
            for (int i = 0; i < newCorrectWords.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Word {i + 1}:", newCorrectWords[i]);
                if (GUILayout.Button("×", GUILayout.Width(30)))
                {
                    newCorrectWords.RemoveAt(i);
                    break;
                }
                if (i > 0 && GUILayout.Button("↑", GUILayout.Width(30)))
                {
                    string temp = newCorrectWords[i];
                    newCorrectWords[i] = newCorrectWords[i - 1];
                    newCorrectWords[i - 1] = temp;
                }
                if (i < newCorrectWords.Count - 1 && GUILayout.Button("↓", GUILayout.Width(30)))
                {
                    string temp = newCorrectWords[i];
                    newCorrectWords[i] = newCorrectWords[i + 1];
                    newCorrectWords[i + 1] = temp;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            bool canCreateLevel = !string.IsNullOrWhiteSpace(newLevelId) &&
                                 newProblemImage != null &&
                                 newProblemAnimation != null &&
                                 newCorrectAnimation != null &&
                                 newIncorrectAnimation != null &&
                                 newWordOptions.Count > 0 &&
                                 newCorrectWords.Count > 0;
            EditorGUI.BeginDisabledGroup(!canCreateLevel);
            if (GUILayout.Button("Create Level"))
            {
                LevelData newLevel = levelFactory.CreateLevel(
                    newLevelId,
                    newProblemImage,
                    newProblemAnimation,
                    newCorrectAnimation,
                    newIncorrectAnimation,
                    newWordOptions.ToList(), // This line should now work with System.Linq
                    newCorrectWords.ToList() // This line should now work with System.Linq
                );
                levels.Add(newLevel);
                selectedLevelIndex = levels.Count - 1;
                ClearNewLevelForm();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawBottomControls()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            exportPath = EditorGUILayout.TextField("Export Path:", exportPath);
            if (GUILayout.Button("Save Levels", GUILayout.Width(100)))
            {
                SaveLevels();
            }
            if (GUILayout.Button("Load Levels", GUILayout.Width(100)))
            {
                LoadLevels();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SaveLevels()
        {
            try
            {
                var serializableLevels = new List<SerializableLevelData>();
                foreach (var level in levels)
                {
                    serializableLevels.Add(new SerializableLevelData
                    {
                        levelId = level.levelId,
                        problemImagePath = AssetDatabase.GetAssetPath(level.problemImage),
                        problemAnimationPath = AssetDatabase.GetAssetPath(level.problemAnimation),
                        correctAnimationPath = AssetDatabase.GetAssetPath(level.correctAnimation),
                        incorrectAnimationPath = AssetDatabase.GetAssetPath(level.incorrectAnimation),
                        wordOptions = level.wordOptions,
                        correctWords = level.correctWords
                    });
                }
                string json = JsonUtility.ToJson(new SerializableLevelDataList { levels = serializableLevels }, true);
                string directory = Path.GetDirectoryName(exportPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(exportPath, json);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Save Successful", $"Saved {levels.Count} levels to {exportPath}", "OK");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Save Failed", $"Error saving levels: {ex.Message}", "OK");
                Debug.LogError($"Error saving levels: {ex}");
            }
        }

        private void LoadLevels()
        {
            try
            {
                if (File.Exists(exportPath))
                {
                    string json = File.ReadAllText(exportPath);
                    var serializableLevels = JsonUtility.FromJson<SerializableLevelDataList>(json);
                    levels.Clear();
                    foreach (var serializableLevel in serializableLevels.levels)
                    {
                        LevelData level = new LevelData
                        {
                            levelId = serializableLevel.levelId,
                            problemImage = AssetDatabase.LoadAssetAtPath<Sprite>(serializableLevel.problemImagePath),
                            problemAnimation = AssetDatabase.LoadAssetAtPath<AnimationClip>(serializableLevel.problemAnimationPath),
                            correctAnimation = AssetDatabase.LoadAssetAtPath<AnimationClip>(serializableLevel.correctAnimationPath),
                            incorrectAnimation = AssetDatabase.LoadAssetAtPath<AnimationClip>(serializableLevel.incorrectAnimationPath),
                            wordOptions = serializableLevel.wordOptions,
                            correctWords = serializableLevel.correctWords
                        };
                        levels.Add(level);
                    }
                    selectedLevelIndex = levels.Count > 0 ? 0 : -1;
                    EditorUtility.DisplayDialog("Load Successful", $"Loaded {levels.Count} levels from {exportPath}", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("File Not Found", $"Could not find file at {exportPath}", "OK");
                }
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("Load Failed", $"Error loading levels: {ex.Message}", "OK");
                Debug.LogError($"Error loading levels: {ex}");
            }
        }

        private void ClearNewLevelForm()
        {
            newLevelId = "level_" + (levels.Count + 1);
            newProblemImage = null;
            newProblemAnimation = null;
            newCorrectAnimation = null;
            newIncorrectAnimation = null;
            newWordOption = "";
            newWordOptions.Clear();
            newCorrectWords.Clear();
        }
    }

    [System.Serializable]
    public class SerializableLevelData
    {
        public string levelId;
        public string problemImagePath;
        public string problemAnimationPath;
        public string correctAnimationPath;
        public string incorrectAnimationPath;
        public List<string> wordOptions = new List<string>();
        public List<string> correctWords = new List<string>();
    }

    [System.Serializable]
    public class SerializableLevelDataList
    {
        public List<SerializableLevelData> levels = new List<SerializableLevelData>();
    }
}
#endif