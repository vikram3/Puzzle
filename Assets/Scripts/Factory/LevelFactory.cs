using System.Collections.Generic;
using UnityEngine;
using WordPuzzle.Model;

namespace WordPuzzle.Factory
{
    public class LevelFactory
    {
        public LevelData CreateLevel(string levelId, Sprite problemImage, AnimationClip problemAnimation,
                                     AnimationClip correctAnimation, AnimationClip incorrectAnimation,
                                     List<string> wordOptions, List<string> correctWords)
        {
            return new LevelData
            {
                levelId = levelId,
                problemImage = problemImage,
                problemAnimation = problemAnimation,
                correctAnimation = correctAnimation,
                incorrectAnimation = incorrectAnimation,
                wordOptions = wordOptions,
                correctWords = correctWords
            };
        }
    }
}