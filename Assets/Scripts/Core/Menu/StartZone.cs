using Core.Players;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Core.Menu
{
    public class StartZone : MonoBehaviour
    {
        [SerializeField]
        GameObject progressBar;

        [SerializeField]
        AnimationCurve progressBarCurve;

        [SerializeField]
        LayerMask playerMask;


        [SerializeField]
        private float timeForStart;
        private float timer;

        List<Player> playersInStartZone = new();


        void Start()
        {
            ResetProgressBar();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (!LayerMaskUtils.HasLayer(playerMask, collision.gameObject.layer)) return;
            PlayerEnterStartZone(collision.gameObject.GetComponent<Player>());
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (!LayerMaskUtils.HasLayer(playerMask, collision.gameObject.layer)) return;
            PlayerLeaveStartZone(collision.gameObject.GetComponent<Player>());
        }

        public void PlayerEnterStartZone(Player player)
        {
            playersInStartZone.Add(player);
        }

        public void PlayerLeaveStartZone(Player player)
        {
            if (playersInStartZone.Contains(player))
            {
                playersInStartZone.Remove(player);
            }

            ResetProgressBar();
        }

        public void ChangeProgressBar(float value)
        {
            value = progressBarCurve.Evaluate(value);
            progressBar.transform.localScale = new Vector3(value * 7.5f, 0.4f, 1.0f);
        }
        private void ResetProgressBar()
        {
            timer = 0.0f;
            ChangeProgressBar(0.0f);
        }

        void Update()
        {
            if (playersInStartZone.Count > 1 && playersInStartZone.Count == PlayersManager.instance.GetNumberOfPlayers())
            {
                timer += Time.deltaTime;
                ChangeProgressBar(timer / timeForStart);
            }

            if (timer > 0.0f && playersInStartZone.Count < PlayersManager.instance.GetNumberOfPlayers())
            {
                ResetProgressBar();
            }

            if (timer > timeForStart)
            {
                GameManager.instance.StartGame();
            }
        }
    }
}
