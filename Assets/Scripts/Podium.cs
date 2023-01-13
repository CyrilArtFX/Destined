using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public struct PodiumData
{
    public Sprite sprite;
    public float offsetY;
    public int playerClassement;
}

public class Podium : MonoBehaviour
{
    [SerializeField]
    private List<PodiumData> datas;

    [SerializeField]
    private TextMeshPro text;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public Vector3 SetPodium(int classement, int score)
    {
        Vector3 position = transform.position;

        foreach (PodiumData data in datas)
        {
            if(data.playerClassement == classement)
            {
                sr.sprite = data.sprite;
                position.y += data.offsetY;
                break;
            }
        }

        text.text = score.ToString();

        return position;
    }
}
