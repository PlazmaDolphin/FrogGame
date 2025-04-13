using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WitchMove : MonoBehaviour
{
    public Transform player;
    private float xSpeed = 1.5f;
    public float yFollowSpeed = 10f;
    private float cameraSwitchOnDistance = 9f;
    private float cameraFollowDistance = 7.6f;
    private float cameraSwitchOffDistance = 10f;
    private float farOn = 18f, farOff = 15f;
    private bool far = false;
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
        Time.timeScale = 1f; // Ensure time scale is normal at start
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
        far = xDist >= farOn ? true : far;
        far = xDist < farOff ? false : far;

        // Audio logic based on horizontal distance

        BG1.enabled = far;
        BG2.enabled = !far && !cameraLocked;
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
            SceneManager.LoadScene("gameOver"); // Load the game over scene
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
