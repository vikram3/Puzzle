using System.Collections.Generic;
using UnityEngine;

namespace WordPuzzle.Model
{
    [System.Serializable]
    public class LevelData
    {
        public string levelId;
        public Sprite problemImage;
        public AnimationClip problemAnimation;
        public AnimationClip correctAnimation;
        public AnimationClip incorrectAnimation;
        public List<string> wordOptions = new List<string>();
        public List<string> correctWords = new List<string>();
    }
}