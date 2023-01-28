using System.Collections.Generic;
using UnityEngine;
using Core.Players;
using System.Linq;

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

        private List<Transform> targetsTransforms = new();

        void FixedUpdate()
        {
            //  auto update transforms on player count changes
            if (autoUpdatePlayers && targetsTransforms.Count != PlayersManager.instance.GetNumberOfPlayers())
            {
                SetTargets(PlayersManager.instance.GetPlayers());
            }

            if(followActivated && targetsTransforms.Count > 0)
			{
				//  apply position
				Vector3 centered_position = GetTargetsCenter();
				if ( isLagEnabled )
				{
					transform.position = Vector3.Lerp( transform.position, centered_position, Time.deltaTime * lagSpeed );
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

		private Vector3 GetTargetsCenter()
		{
			Vector3 centered_position = Vector3.zero;
			foreach ( Transform player_transform in targetsTransforms )
			{
				centered_position += player_transform.position;
			}
			centered_position /= targetsTransforms.Count;
			centered_position.z = -10.0f;
			return centered_position;
		}

		public void EnableFollowMode(IEnumerable<Player> players)
        {
            SetTargets(players);
            followActivated = true;
        }

        public void SetTargets(IEnumerable<Transform> targets, bool instant_warp_to = false)
        {
            targetsTransforms.Clear();
            foreach (Transform target in targets)
            {
                targetsTransforms.Add(target);
            }

            if (instant_warp_to && targetsTransforms.Count > 0)
            {
                transform.position = GetTargetsCenter();
            }
        }
        public void SetTargets(IEnumerable<Player> players) => SetTargets(players.Select(player => player.transform));

        public void DisableFollowMode()
        {
            followActivated = false;
        }
    }
}
