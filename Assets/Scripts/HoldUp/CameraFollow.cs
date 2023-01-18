using System.Collections.Generic;
using UnityEngine;
using Core.Players;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Camera Follow")]
    public class CameraFollow : MonoBehaviour
    {
        private bool followActivated = false;
        private List<Transform> playersTransforms = new();

        void Update()
        {
            if(followActivated)
            {
                Vector3 centered_position = Vector3.zero;
                foreach (Transform player_transform in playersTransforms)
                {
                    centered_position += player_transform.position;
                }
                centered_position /= playersTransforms.Count;
                centered_position.z = -10.0f;
                transform.position = centered_position;
            }
            else
            {
                transform.position = new Vector3(0.0f, 0.0f, -10.0f);
            }
        }

        public void EnableFollowMode(List<Player> players)
        {
            foreach (Player player in players)
            {
                playersTransforms.Add(player.transform);
            }
            followActivated = true;
        }

        public void DisableFollowMode()
        {
            followActivated = false;
        }
    }
}
