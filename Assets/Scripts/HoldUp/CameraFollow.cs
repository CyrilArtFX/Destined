using System.Collections.Generic;
using UnityEngine;
using Core.Players;

namespace HoldUp
{
    [AddComponentMenu("Scripts/HoldUp Camera Follow")]
    public class CameraFollow : MonoBehaviour
    {
        [Header("Lag")]
        [SerializeField]
        private bool isLagEnabled = false;
        [SerializeField]
        private float lagSpeed = 2.0f;

        [Header("Extras")]
        [SerializeField]
        private bool autoUpdatePlayers = false;
        [SerializeField]
        private bool followActivated = false;

        private List<Transform> playersTransforms = new();

        void FixedUpdate()
        {
            //  auto update transforms on player count changes
            if (autoUpdatePlayers && playersTransforms.Count != PlayersManager.instance.GetNumberOfPlayers())
            {
                SetTargets(PlayersManager.instance.GetPlayers());
            }

            if(followActivated && playersTransforms.Count > 0)
            {
                Vector3 centered_position = Vector3.zero;
                foreach (Transform player_transform in playersTransforms)
                {
                    centered_position += player_transform.position;
                }
                centered_position /= playersTransforms.Count;
                centered_position.z = -10.0f;

                //  apply position
                if (isLagEnabled)
                {
                    transform.position = Vector3.Lerp(transform.position, centered_position, Time.deltaTime * lagSpeed);
                }
                else
                { 
                    transform.position = centered_position;
                }
            }
            else
            {
                transform.position = new Vector3(0.0f, 0.0f, -10.0f);
            }
        }

        public void EnableFollowMode(List<Player> players)
        {
            SetTargets(players);
            followActivated = true;
        }

        public void SetTargets(List<Player> players)
        {
            playersTransforms.Clear();
            foreach (Player player in players)
            {
                playersTransforms.Add(player.transform);
            }
        }

        public void DisableFollowMode()
        {
            followActivated = false;
        }
    }
}
