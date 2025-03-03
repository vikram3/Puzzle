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
                    Debug.Log("Loading levels from config file");
                    var serializableLevels = JsonUtility.FromJson<SerializableLevelDataList>(levelConfigFile.text);
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
                        levels.Add(level);
                    }
                    Debug.Log($"Loaded {levels.Count} levels from config");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error loading levels from config: {ex.Message}");
                    levels = CreateSampleLevels();
                }
            }
            else
            {
                Debug.Log("No config file assigned, using sample levels");
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
                return Path.ChangeExtension(resourcePath, null);
            }
            return assetPath;
        }

        private List<LevelData> CreateSampleLevels()
        {
            List<LevelData> levels = new List<LevelData>();
            Debug.Log("Creating sample levels");

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

            Debug.Log($"Created {levels.Count} sample levels");
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