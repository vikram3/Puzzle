using System.Collections.Generic;
using UnityEngine;
using WordPuzzle.Model;
using WordPuzzle.Factory;

namespace WordPuzzle
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private TextAsset levelConfigFile;
        [SerializeField] private List<Sprite> availableImages = new List<Sprite>();
        [SerializeField] private List<AnimationClip> availableAnimations = new List<AnimationClip>();

        private LevelFactory levelFactory = new LevelFactory();

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

        public List<LevelData> LoadLevels()
        {
            List<LevelData> levels = new List<LevelData>();
            Debug.Log("LevelLoader: Loading levels");

            if (levelConfigFile != null)
            {
                try
                {
                    Debug.Log("LevelLoader: Parsing level config file");
                    var serializableLevels = JsonUtility.FromJson<SerializableLevelDataList>(levelConfigFile.text);
                    if (serializableLevels == null || serializableLevels.levels == null)
                    {
                        Debug.LogError("LevelLoader: Failed to parse JSON or no levels found");
                        return CreateDefaultLevels();
                    }

                    foreach (var serializableLevel in serializableLevels.levels)
                    {
                        // Extract resource paths
                        string imagePath = GetResourcePath(serializableLevel.problemImagePath);
                        string problemAnimPath = GetResourcePath(serializableLevel.problemAnimationPath);
                        string correctAnimPath = GetResourcePath(serializableLevel.correctAnimationPath);
                        string incorrectAnimPath = GetResourcePath(serializableLevel.incorrectAnimationPath);

                        // Load assets (or use available lists if assigned)
                        Sprite problemImage = Resources.Load<Sprite>(imagePath);
                        AnimationClip problemAnim = Resources.Load<AnimationClip>(problemAnimPath);
                        AnimationClip correctAnim = Resources.Load<AnimationClip>(correctAnimPath);
                        AnimationClip incorrectAnim = Resources.Load<AnimationClip>(incorrectAnimPath);

                        // Log warnings if assets are missing
                        if (problemImage == null) Debug.LogWarning($"LevelLoader: Sprite not found at {imagePath}");
                        if (problemAnim == null) Debug.LogWarning($"LevelLoader: Animation not found at {problemAnimPath}");

                        LevelData level = levelFactory.CreateLevel(
                            serializableLevel.levelId,
                            problemImage ?? (availableImages.Count > 0 ? availableImages[0] : null), // Fallback
                            problemAnim ?? (availableAnimations.Count > 0 ? availableAnimations[0] : null),
                            correctAnim ?? (availableAnimations.Count > 1 ? availableAnimations[1] : null),
                            incorrectAnim ?? (availableAnimations.Count > 2 ? availableAnimations[2] : null),
                            serializableLevel.wordOptions,
                            serializableLevel.correctWords
                        );
                        levels.Add(level);
                    }
                    Debug.Log($"LevelLoader: Loaded {levels.Count} levels from config");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"LevelLoader: Error loading levels from config: {ex.Message}");
                    levels = CreateDefaultLevels();
                }
            }
            else
            {
                Debug.Log("LevelLoader: No config file assigned, using default levels");
                levels = CreateDefaultLevels();
            }

            return levels;
        }

        private string GetResourcePath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath)) return string.Empty;
            int resourcesIndex = assetPath.IndexOf("Resources/");
            if (resourcesIndex >= 0)
            {
                string resourcePath = assetPath.Substring(resourcesIndex + 10);
                return System.IO.Path.ChangeExtension(resourcePath, null); // Remove extension
            }
            Debug.LogWarning($"LevelLoader: Path {assetPath} not in Resources, attempting raw path");
            return assetPath;
        }

        private List<LevelData> CreateDefaultLevels()
        {
            List<LevelData> levels = new List<LevelData>();
            Debug.Log("LevelLoader: Creating default levels");

            // Default Level 1 (Sick Mom)
            levels.Add(levelFactory.CreateLevel(
                "Sick Mom",
                availableImages.Count > 0 ? availableImages[0] : null,
                availableAnimations.Count > 0 ? availableAnimations[0] : null,
                availableAnimations.Count > 1 ? availableAnimations[1] : null,
                availableAnimations.Count > 2 ? availableAnimations[2] : null,
                new List<string> { "Medicine", "Candy", "Water" },
                new List<string> { "Medicine", "Water" }
            ));

            // Default Level 2 (ThirstyBoy)
            levels.Add(levelFactory.CreateLevel(
                "ThirstyBoy",
                availableImages.Count > 1 ? availableImages[1] : null,
                availableAnimations.Count > 3 ? availableAnimations[3] : null,
                availableAnimations.Count > 4 ? availableAnimations[4] : null,
                availableAnimations.Count > 5 ? availableAnimations[5] : null,
                new List<string> { "Water", "Milk" },
                new List<string> { "Water" }
            ));

            Debug.Log($"LevelLoader: Created {levels.Count} default levels");
            return levels;
        }
    }
}