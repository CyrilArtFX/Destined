using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burrow : MonoBehaviour
{
    /*[SerializeField]
	private Sprite[] sprites;

	[SerializeField]
	private new SpriteRenderer renderer;

	void Start()
	{
		renderer.sprite = sprites[Random.Range( 0, sprites.Length )];
	}*/

    [SerializeField]
    GameObject carrotPrefab;
    [SerializeField]
    int maxAliveCarrots = 20;

    public BurrowData Data;

    float[] currentZonesCooldowns;
    List<Transform> aliveCarrots = new();

    float currentSpawnRatio;
    float gameTimer;
    float maxGameTimer;


    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        //  init cooldowns
        currentZonesCooldowns = new float[Data.ZonesCooldowns.Length];
        for (int id = 0; id < currentZonesCooldowns.Length; id++)
            currentZonesCooldowns[id] = Data.ZonesCooldowns[id];

        if(Data.GameTimer && GameSceneManager.instance)
        {
            gameTimer = maxGameTimer = GameSceneManager.instance.GameTimer;
        }
        currentSpawnRatio = Data.SpawnProgressionCurve.Evaluate(0.0f);
    }

    void Update()
    {
        //  remove null references
        for (int i = 0; i < aliveCarrots.Count; i++)
        {
            if (aliveCarrots[i] == null)
            {
                aliveCarrots.RemoveAt(i);
                break;
            }
        }

        //  handle cooldowns
        for (int id = 0; id < currentZonesCooldowns.Length; id++)
        {
            if (aliveCarrots.Count + 1 > maxAliveCarrots + (int)(maxAliveCarrots * currentSpawnRatio)) continue;
            if ((currentZonesCooldowns[id] -= Time.deltaTime) > 0.0f) continue;

            float rdm = Random.Range(0.0f, currentSpawnRatio) * Data.carrotSpawnProbabilityMultiplier;
            int numberToSpawn = Mathf.Clamp(Mathf.FloorToInt(rdm), 1, Data.maxCarrotSpawnAtOnce);
            for(int i = 0; i < numberToSpawn; i++)
            {
                SpawnCarrot(GetZoneRange(id));
            }

            currentZonesCooldowns[id] = Data.ZonesCooldowns[id];
        }

        if (gameTimer > 0)
        {
            gameTimer-= Time.deltaTime;
            currentSpawnRatio = Data.SpawnProgressionCurve.Evaluate((maxGameTimer - gameTimer) / maxGameTimer);
        }
    }

    void SpawnCarrot(Vector2 radius_range)
    {
        //  instantiate
        GameObject obj = Instantiate(carrotPrefab, GameManager.instance.transform);

        //  set pos
        float ang = Random.Range(0, 360);
        float radius = Random.Range(radius_range.x, radius_range.y);
        obj.transform.position = transform.position + new Vector3(Mathf.Cos(ang) * radius, Mathf.Sin(ang) * radius);

        //  register
        aliveCarrots.Add(obj.transform);
    }

    Vector2 GetZoneRange(int id)
    {
        Vector2 range = new(0, Data.ZonesRadiuses[id]);

        if (id - 1 >= 0)
            range.x = Data.ZonesRadiuses[id - 1];

        return range;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        for (int id = 0; id < Data.ZonesRadiuses.Length; id++)
            Gizmos.DrawWireSphere(transform.position, Data.ZonesRadiuses[id]);
    }
}
