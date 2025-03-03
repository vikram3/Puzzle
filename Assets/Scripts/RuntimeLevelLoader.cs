using System.Collections.Generic;
using UnityEngine;
using WordPuzzle.Model;
using WordPuzzle.Factory;
using System.IO;

namespace WordPuzzle
{
    public class RuntimeLevelLoader : MonoBehaviour
    {
        [SerializeField] private TextAsset levelConfigFile;
        private LevelFactory levelFactory = new LevelFactory();

        private void Awake()
        {
            // Ensure levelConfigFile is loaded even if not assigned in Inspector
            if (levelConfigFile == null)
            {
                levelConfigFile = Resources.Load<TextAsset>("LevelData");
                if (levelConfigFile == null)
                {
                    Debug.LogError("RuntimeLevelLoader: Failed to load LevelData.json from Resources!");
                }
                else
                {
                    Debug.Log("RuntimeLevelLoader: Loaded LevelData.json manually from Resources");
                }
            }
        }

        public List<LevelData> LoadLevels()
        {
            List<LevelData> levels = new List<LevelData>();
            Debug.Log($"RuntimeLevelLoader: Attempting to load levels on {Application.platform}");

            if (levelConfigFile != null)
            {
                try
                {
                    Debug.Log($"RuntimeLevelLoader: Loading from config file: {levelConfigFile.name}");
                    var serializableLevels = JsonUtility.FromJson<SerializableLevelDataList>(levelConfigFile.text);
                    if (serializableLevels == null || serializableLevels.levels == null)
                    {
                        Debug.LogError("RuntimeLevelLoader: Failed to parse JSON or no levels found");
                        return new List<LevelData>(); // Return empty list instead of sample levels
                    }

                    foreach (var serializableLevel in serializableLevels.levels)
                    {
                        string imagePath = GetResourcePath(serializableLevel.problemImagePath);
                        string problemAnimPath = GetResourcePath(serializableLevel.problemAnimationPath);
                        string correctAnimPath = GetResourcePath(serializableLevel.correctAnimationPath);
                        string incorrectAnimPath = GetResourcePath(serializableLevel.incorrectAnimationPath);

                        LevelData level = new LevelData
                        {
                            levelId = serializableLevel.levelId,
                            problemImage = Resources.Load<Sprite>(imagePath),
                            problemAnimation = Resources.Load<AnimationClip>(problemAnimPath),
                            correctAnimation = Resources.Load<AnimationClip>(correctAnimPath),
                            incorrectAnimation = Resources.Load<AnimationClip>(incorrectAnimPath),
                            wordOptions = serializableLevel.wordOptions,
                            correctWords = serializableLevel.correctWords
                        };

                        if (level.problemImage == null) Debug.LogWarning($"RuntimeLevelLoader: Sprite not found at {imagePath}");
                        if (level.problemAnimation == null) Debug.LogWarning($"RuntimeLevelLoader: Animation not found at {problemAnimPath}");
                        Debug.Log($"RuntimeLevelLoader: Loaded level: {level.levelId}");

                        levels.Add(level);
                    }
                    Debug.Log($"RuntimeLevelLoader: Loaded {levels.Count} levels from config");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"RuntimeLevelLoader: Error loading levels from config: {ex.Message}");
                    levels = new List<LevelData>(); // Return empty list on failure
                }
            }
            else
            {
                Debug.LogError("RuntimeLevelLoader: No config file assigned or found in Resources!");
                levels = new List<LevelData>(); // Return empty list instead of sample levels
            }

            Debug.Log($"RuntimeLevelLoader: Returning {levels.Count} levels");
            return levels;
        }

        private string GetResourcePath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath)) return string.Empty;
            int resourcesIndex = assetPath.IndexOf("Resources/");
            if (resourcesIndex >= 0)
            {
                string resourcePath = assetPath.Substring(resourcesIndex + 10);
                Debug.Log($"RuntimeLevelLoader: Resource path: {resourcePath}");
                return Path.ChangeExtension(resourcePath, null);
            }
            Debug.LogWarning($"RuntimeLevelLoader: Path {assetPath} not in Resources, using raw path");
            return assetPath;
        }

        private List<LevelData> CreateSampleLevels() // Keep for reference but not used
        {
            List<LevelData> levels = new List<LevelData>();
            Debug.Log("RuntimeLevelLoader: Creating sample levels");
            // Sample levels removed to prevent accidental use
            Debug.Log($"RuntimeLevelLoader: Created {levels.Count} sample levels");
            return levels;
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