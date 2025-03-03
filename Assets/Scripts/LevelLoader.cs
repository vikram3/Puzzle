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

        public List<LevelData> LoadLevels()
        {
            List<LevelData> levels = new List<LevelData>();

            levels.Add(CreateSampleLevel1());
            levels.Add(CreateSampleLevel2());

            return levels;
        }

        private LevelData CreateSampleLevel1()
        {
            return levelFactory.CreateLevel(
                "level_1",
                availableImages[0],
                availableAnimations[0],
                availableAnimations[1],
                availableAnimations[2],
                new List<string> { "Medicine", "Water", "Candy", "Toy" },
                new List<string> { "Medicine" }
            );
        }

        private LevelData CreateSampleLevel2()
        {
            return levelFactory.CreateLevel(
                "level_2",
                availableImages[1],
                availableAnimations[3],
                availableAnimations[4],
                availableAnimations[5],
                new List<string> { "The", "Girl", "Needs", "Sleep", "Food" },
                new List<string> { "The", "Girl", "Needs", "Sleep" }
            );
        }
    }
}