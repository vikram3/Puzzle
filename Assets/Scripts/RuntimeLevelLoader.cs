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

        public List<LevelData> LoadLevels()
        {
            List<LevelData> levels = new List<LevelData>();
            Debug.Log("RuntimeLevelLoader: Attempting to load levels");

            if (levelConfigFile != null)
            {
                try
                {
                    Debug.Log("RuntimeLevelLoader: Loading from config file");
                    var serializableLevels = JsonUtility.FromJson<SerializableLevelDataList>(levelConfigFile.text);
                    if (serializableLevels == null || serializableLevels.levels == null)
                    {
                        Debug.LogError("RuntimeLevelLoader: Failed to parse JSON or no levels found");
                        return CreateSampleLevels();
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

                        // Log if assets are missing (expected for now)
                        if (level.problemImage == null) Debug.LogWarning($"RuntimeLevelLoader: Sprite not found at {imagePath}");
                        if (level.problemAnimation == null) Debug.LogWarning($"RuntimeLevelLoader: Animation not found at {problemAnimPath}");

                        levels.Add(level);
                    }
                    Debug.Log($"RuntimeLevelLoader: Loaded {levels.Count} levels from config");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"RuntimeLevelLoader: Error loading levels from config: {ex.Message}");
                    levels = CreateSampleLevels();
                }
            }
            else
            {
                Debug.Log("RuntimeLevelLoader: No config file assigned, using sample levels");
                levels = CreateSampleLevels();
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
                return Path.ChangeExtension(resourcePath, null); // Remove extension
            }
            // For Packages/ paths, try loading directly (won't work with Resources.Load, but log for clarity)
            Debug.LogWarning($"RuntimeLevelLoader: Path {assetPath} not in Resources, attempting raw path");
            return assetPath;
        }

        private List<LevelData> CreateSampleLevels()
        {
            List<LevelData> levels = new List<LevelData>();
            Debug.Log("RuntimeLevelLoader: Creating sample levels");

            levels.Add(levelFactory.CreateLevel(
                "level_1",
                null,
                null,
                null,
                null,
                new List<string> { "Medicine", "Water", "Candy", "Toy" },
                new List<string> { "Medicine" }
            ));

            levels.Add(levelFactory.CreateLevel(
                "level_2",
                null,
                null,
                null,
                null,
                new List<string> { "The", "Girl", "Needs", "Sleep", "Food" },
                new List<string> { "The", "Girl", "Needs", "Sleep" }
            ));

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