using Carroted;
using System.Collections.Generic;
using UnityEngine;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Game Manager")]
    public class GameManager : Core.GameManager
    {
        private List<PlayerScore> playersScores = new();

        public override void StartGame()
        {
            Debug.Log("Starting the game (en fait non)");
        }
    }
}
