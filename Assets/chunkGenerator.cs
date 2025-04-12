using System;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public GameObject theLilypad;
    public Vector2 areaSize = new Vector2(20f, 10f);
    public static float density = 10; // Number of lilypads to spawn
    public int maxAttemptsPerLilypad = 20;
    public float meanScale = 1f;
    public float scaleStdDev = 0.2f;

    void Start()
    {
        for (int i = 0; i < Mathf.RoundToInt(density); i++)
        {
            bool placed = false;
            for (int attempt = 0; attempt < maxAttemptsPerLilypad && !placed; attempt++)
            {
                float scale = Mathf.Clamp(NormalRandom(meanScale, scaleStdDev), 0.1f, 5f);
                float checkRadius = scale * 0.5f; // Assuming original lilypad collider radius is 0.5 at scale 1

                Vector2 randomPos = (Vector2)transform.position + new Vector2(
                    UnityEngine.Random.Range(-areaSize.x / 2f, areaSize.x / 2f),
                    UnityEngine.Random.Range(-areaSize.y / 2f, areaSize.y / 2f)
                );

                Collider2D[] hits = Physics2D.OverlapCircleAll(randomPos, checkRadius);
                bool collision = false;

                foreach (Collider2D hit in hits)
                {
                    if (hit.CompareTag("Lilypad"))
                    {
                        collision = true;
                        break;
                    }
                }

                if (!collision)
                {
                    GameObject lily = Instantiate(theLilypad, randomPos, Quaternion.identity);
                    lily.transform.localScale = new Vector3(scale, scale, 1f);
                    placed = true;
                }
            }
        }
    }

    float NormalRandom(float mean, float stdDev)
    {
        // Box-Muller transform
        float u1 = 1.0f - UnityEngine.Random.value;
        float u2 = 1.0f - UnityEngine.Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + stdDev * randStdNormal;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0));
    }
}
