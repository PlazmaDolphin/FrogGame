using TMPro;
using UnityEngine;

public class WitchMove : MonoBehaviour
{
    public Transform player;
    private float xSpeed = 1.5f;
    public float yFollowSpeed = 10f;
    private float cameraSwitchOnDistance = 9f;
    private float cameraFollowDistance = 7.6f;
    private float cameraSwitchOffDistance = 10f;
    private float maxDistance = 25f;
    public AudioSource BG1;
    public AudioSource BG2;
    public AudioSource BG3;
    public Camera mainCamera;
    public TextMeshProUGUI distanceTxt, gameOverTxt;
    public Collider2D col;

    //private Vector3 cameraOffset;
    private bool cameraLocked = false;
    private float initialCameraZ;

    void Start()
    {
        initialCameraZ = mainCamera.transform.position.z;
    }

    void Update()
    {
        // Move right constantly
        transform.position += Vector3.right * xSpeed * Time.deltaTime;
        // Clamp X position to max distance from player
        float xDist = Mathf.Abs(transform.position.x - player.position.x);
        Vector3 pos = transform.position;
        if (xDist > maxDistance)
        {
            pos.x = player.position.x - maxDistance;
            transform.position = pos;
        }

        // Lerp to player's Y position quickly
        pos.y = Mathf.Lerp(pos.y, player.position.y, yFollowSpeed * Time.deltaTime);
        transform.position = pos;

        // Audio logic based on horizontal distance

        BG1.enabled = xDist >= 18f;
        BG2.enabled = xDist < 18f && !cameraLocked;
        BG3.enabled = cameraLocked;
        distanceTxt.text = "Witch Distance: " + xDist.ToString("F2") + "m";

        // Camera behavior
        float screenXDistance = player.position.x - transform.position.x;
        if (screenXDistance < cameraSwitchOnDistance)
        {
            cameraLocked = true;
        }
        else if (screenXDistance > cameraSwitchOffDistance)
        {
            cameraLocked = false;
        }

        if (cameraLocked)
        {
            Vector3 camPos = mainCamera.transform.position;
            camPos.x = transform.position.x + cameraFollowDistance; // Lock player ahead of witch
            camPos.y = Mathf.Lerp(camPos.y, player.position.y, Time.deltaTime * 5f); // Smooth Y
            camPos.z = initialCameraZ;
            mainCamera.transform.position = camPos;
        }
        else
        {
            Vector3 camPos = player.position;
            camPos.z = initialCameraZ;
            mainCamera.transform.position = camPos;
        }
        if (CheckPlayerCollision())
        {
            gameOverTxt.gameObject.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }
    bool CheckPlayerCollision()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        Collider2D[] hits = new Collider2D[10];
        int count = col.Overlap(filter, hits);

        for (int i = 0; i < count; i++)
        {
            if (hits[i] != null && hits[i].CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
        
}
