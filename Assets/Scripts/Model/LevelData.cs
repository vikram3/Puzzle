using System.Collections.Generic;
using UnityEngine;

namespace WordPuzzle.Model
{
    [System.Serializable]
    public class LevelData
    {
        public string levelId;
        public Sprite problemImage; // Optional, can be null
        public AnimationClip problemAnimation; // Optional, can be null
        public AnimationClip correctAnimation; // Optional, can be null
        public AnimationClip incorrectAnimation; // Optional, can be null
        public List<string> wordOptions = new List<string>();
        public List<string> correctWords = new List<string>();
    }
}