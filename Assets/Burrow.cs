using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burrow : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private new SpriteRenderer renderer;

    void Start()
    {
        renderer.sprite = sprites[Random.Range( 0, sprites.Length )];
    }

    void Update()
    {
        
    }
}
