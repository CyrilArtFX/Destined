using UnityEngine;

namespace HoldUp
{
    public abstract class Item : MonoBehaviour
    {
        public Vector2 Direction => playerController.AimDirection != Vector2.zero ? playerController.AimDirection : playerController.LastPerformedDirection;

        public bool IsDefaultItem;
        public ItemOnGround ItemOnGround;

        protected PlayerController playerController;

        public virtual void Initialize(PlayerController controller)
        {
            playerController = controller;
        }

        public virtual void OnUsePressed()
        {
        }

        public virtual void OnUseReleased()
        {
        }

        protected void Rotate(Vector2 rotationDirection)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Atan2(rotationDirection.y, rotationDirection.x) * Mathf.Rad2Deg));
        }

        public virtual void Drop()
        {
            GameObject droppedItem = GameObject.Instantiate(ItemOnGround.gameObject, GameManager.instance.transform);
            droppedItem.transform.position = playerController.transform.position;
        }
    }
}
