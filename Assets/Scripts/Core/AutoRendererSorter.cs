using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

namespace Core
{
    public class AutoRendererSorter : MonoBehaviour
    {
        private const float AXIS_FACTOR = 16.0f;

        [SerializeField, Tooltip( "Optional. The SortingGroup to sort order" )]
        private SortingGroup sortingGroup;
        [SerializeField, Tooltip( "Optional. The renderer to sort order" )]
        private new Renderer renderer;

        [SerializeField, Tooltip( "If checked, this component will only sort once on Start and won't Update each frame." )]
        private bool isStatic = false;

        void Start()
        {
            if ( isStatic )
            {
                Update();
                enabled = false;
            }
        }

        void Update()
        {
            int order = (int) -( transform.position.y * AXIS_FACTOR );

            if ( sortingGroup != null )
                sortingGroup.sortingOrder = order;
            if ( renderer != null )
                renderer.sortingOrder = order;
        }
    }
}