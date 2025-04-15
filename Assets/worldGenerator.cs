using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject chunkGeneratorPrefab, landPrefab;
    public Transform player;
    public TextMeshProUGUI statusText;
    public Sprite free0, free1, free2, free3, free4, free5, free6, free7;
    public Image freedomBar;
    private Vector2 chunkSize = new Vector2(20f, 10f);

    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int currentPlayerChunk;
    private int GameEndChunk = 7; // Adjust to make game longer or shorter

    void Start()
    {
        currentPlayerChunk = GetChunkCoord(player.position);
        GenerateChunksAround(currentPlayerChunk);
    }

    void Update()
    {
        Vector2Int playerChunk = GetChunkCoord(player.position);
        if (playerChunk != currentPlayerChunk)
        {
            currentPlayerChunk = playerChunk;
            GenerateChunksAround(currentPlayerChunk);
            RemoveDistantChunks(currentPlayerChunk);
        }
        UpdateStatusText();
        UpdateFreedomBar();
    }

    void UpdateFreedomBar(){
        //get x chunk coord of player
        int x = Mathf.FloorToInt((player.position.x + chunkSize.x/2f) / chunkSize.x);
        if(x < 0) x = 0;
        if(x > 7) x = 7;
        //set sprite based on x chunk coord
        switch (x)
        {
            case 0:
                freedomBar.sprite = free0;
                break;
            case 1:
                freedomBar.sprite = free1;
                break;
            case 2:
                freedomBar.sprite = free2;
                break;
            case 3:
                freedomBar.sprite = free3;
                break;
            case 4:
                freedomBar.sprite = free4;
                break;
            case 5:
                freedomBar.sprite = free5;
                break;
            case 6:
                freedomBar.sprite = free6;
                break;
            case 7:
                freedomBar.sprite = free7;
                break;
        }
    }

    Vector2Int GetChunkCoord(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize.x);
        int y = Mathf.FloorToInt(position.y / chunkSize.y);
        return new Vector2Int(x, y);
    }

    void GenerateChunksAround(Vector2Int centerChunk)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector2Int chunkCoord = new Vector2Int(centerChunk.x + dx, centerChunk.y + dy);
                if (!activeChunks.ContainsKey(chunkCoord))
                {
                    Vector3 spawnPosition = new Vector3(
                        chunkCoord.x * chunkSize.x,
                        chunkCoord.y * chunkSize.y,
                        0f
                    );
                    GameObject chunk;
                    if (chunkCoord.x != GameEndChunk) chunk = Instantiate(chunkGeneratorPrefab, spawnPosition, Quaternion.identity);
                    else chunk = Instantiate(landPrefab, spawnPosition, Quaternion.identity);
                    int density=0;
                    //Adjust density based on chunk's x value (0-2: 12, 3-5: 10, etc)
                    if (chunkCoord.x >= GameEndChunk) density = 0; 
                    else if (chunkCoord.x < 2)
                        density = 12;
                    else if (chunkCoord.x < 4)
                        density = 8;
                    else if (chunkCoord.x < 6)
                        density = 4;
                    else if (chunkCoord.x < 8)
                        density = 4; // Adjust as needed for further chunks
                    Debug.Log("Generating chunk at: " + chunkCoord + " with density: " + density);
                    chunk.GetComponent<ChunkGenerator>().generateChunk(density); // Adjust density as needed
                    activeChunks[chunkCoord] = chunk;
                }
            }
        }
    }

    void RemoveDistantChunks(Vector2Int centerChunk)
    {
        List<Vector2Int> chunksToRemove = new List<Vector2Int>();

        foreach (var chunkCoord in activeChunks.Keys)
        {
            // Only delete if it's more than 3 chunks away AND to the left of the player
            if (chunkCoord.x < centerChunk.x - 3)
            {
                chunksToRemove.Add(chunkCoord);
            }
        }

        foreach (var chunkCoord in chunksToRemove)
        {
            Debug.Log("Destroying chunk at: " + chunkCoord);
            Destroy(activeChunks[chunkCoord]);
            activeChunks.Remove(chunkCoord);
        }
    }
    void UpdateStatusText(){
        if (statusText != null && player != null)
        {
            statusText.text = $"At chunk ({currentPlayerChunk.x},{currentPlayerChunk.y})\nYou have traveled {player.position.x:F1} units";
        }
    }
}
