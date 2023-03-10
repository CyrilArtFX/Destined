using UnityEngine;

namespace HoldUp
{
    public class ItemSpawner : MonoBehaviour
    {
        [SerializeField]
        private Item itemToSpawn;

        [SerializeField]
        private float spawnDelay;

        [SerializeField]
        private Transform spawnPosition;

        private float timer;
        private Item spawnedItem;

        void Start()
        {
            SpawnNewItem();
        }

        void Update()
        {
            if (!spawnedItem)
            {
                timer -= Time.deltaTime;

                if(timer <= 0)
                {
                    SpawnNewItem();
                }
            }
        }

        private void SpawnNewItem()
        {
            spawnedItem = GameObject.Instantiate(itemToSpawn, GameManager.instance.transform);
            spawnedItem.transform.position = spawnPosition.position;
            timer = spawnDelay;
        }
    }
}
