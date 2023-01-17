using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Items;

namespace Carroted
{
    public class Carrot : Collectible
    { 
        public enum ItemType
        {
            CARROT,
            GOLD_CARROT,
        }

        public ItemType Type => type;
        public int Score => score;
        public bool CanBeThrown => canBeThrown;

        [SerializeField]
        private ItemType type;
        [SerializeField]
        private bool canBeThrown = true;
        [SerializeField]
        private int score = 1;


        public override void OnCollect(Inventory inventory)
        {
            GameEvents.OnCollect.Invoke(inventory.Owner, this);
        }
    }
}
