﻿using UnityEngine;
using Core;
using Core.Players;

namespace HoldUp.CCTV
{
	public class SecurityCamera : MonoBehaviour
	{
		public Player Player { get; private set; }
		public VisionFOV Vision => vision;

		[SerializeField]
		private VisionFOV vision;

		void Awake()
		{
			UnControl();
		}

		void Start()
		{
			SecurityCameraNetwork.Cameras.Add( this );
		}

		void OnDestroy()
		{
			SecurityCameraNetwork.Cameras.Remove( this );
		}

		public void TakeControl( Player player )
		{
			Player = player;

			//vision.enabled = true;
			gameObject.layer = LayerMask.NameToLayer( VisionFOV.VISION_MASK_LAYER_NAME );
			vision.gameObject.layer = gameObject.layer;
		}

		public void UnControl()
		{
			Player = null;

			//vision.enabled = false;
			gameObject.layer = LayerMask.NameToLayer( VisionFOV.BEHIND_VISION_MASK_LAYER_NAME );
			vision.gameObject.layer = gameObject.layer;
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			if ( !collision.TryGetComponent( out Player player ) ) return;
			if ( Player != null ) return;

			TakeControl( player );
		}

		private void OnTriggerExit2D( Collider2D collision )
		{
			if ( !collision.TryGetComponent( out Player player ) ) return;
			if ( player != Player ) return;

			UnControl();
		}
	}
}