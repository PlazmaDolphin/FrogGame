using UnityEngine;

public class WaterMash : MonoBehaviour
{
    private float maxSpeed = 2f;
    private float decayRate = 3f;
    private float resetSpeedDuration = 0.1f;
    private string lilypadTag = "Lilypad";
    public Collider2D col;
    public Transform frogPos;

    private float currentSpeed = 0f;
    private float initSmoothSpeed = 0f;
    public bool submerged = false, stroking = false;
    private float resetTimer = 0f;

    void Start()
    {
    }

    void Update()
    {
        // Begin movement on LMB
        if (Input.GetMouseButtonDown(0) && !stroking && submerged)
        {
            resetTimer = 0f;
            stroking = true;
            initSmoothSpeed = currentSpeed;
        }

        // Smooth speed reset
        if (stroking && currentSpeed < maxSpeed)
        {
            resetTimer = Mathf.Clamp01(resetTimer + Time.deltaTime / resetSpeedDuration);
            currentSpeed = Mathf.Lerp(initSmoothSpeed, maxSpeed, resetTimer);
            if(currentSpeed >= maxSpeed)
            {
                stroking = false;
            }
            Debug.Log("speeding up: " + currentSpeed);
        }

        if (submerged)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            Vector3 direction = (mousePos - frogPos.position).normalized;
            frogPos.position += direction * currentSpeed * Time.deltaTime;

            currentSpeed *= Mathf.Exp(-decayRate * Time.deltaTime);
            Debug.Log("slowing down: " + currentSpeed);

            if (CheckLilyCollision())
            {
                Debug.Log("Unsubmerging");
                submerged = false;
                currentSpeed = 0f;
            }
        }
    }
    public bool landCheck(){
        bool onLily = CheckLilyCollision();
        submerged = !onLily;
        if (onLily)
        {
            currentSpeed = 0f;
            resetTimer = 0f;
        }
        Debug.Log("onLily: " + onLily);
        return onLily;
    }
    bool CheckLilyCollision()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        Collider2D[] hits = new Collider2D[10];
        int count = col.Overlap(filter, hits);

        for (int i = 0; i < count; i++)
        {
            if (hits[i] != null && hits[i].CompareTag(lilypadTag))
            {
                return true;
            }
        }
        return false;
    }
}
