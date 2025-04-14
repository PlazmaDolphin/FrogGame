using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterMash2 : MonoBehaviour
{
    private float maxSpeed = 1f;
    private float decayRate = 3f;
    private float resetSpeedDuration = 0.2f;
    private float swimCost = 0.02f, maxCroakSpeed = 0.1f;
    private string lilypadTag = "Lilypad";
    public Collider2D col;
    public Transform frogPos;
    public AudioSource splashSFX, swimSFX;
    public theEnergyBar energy;

    private float currentSpeed = 0f;
    private float initSmoothSpeed = 0f;
    public bool submerged = false, stroking = false, recovering = false;
    private float resetTimer = 5f;
    private float recoverDuration = 0.3f; // How long to jump on lily after submerged
    private float recoverTimer = 0f;
    private float recoverDepth = 0.6f; // How far on lily to jump
    private Vector3 initPos = Vector3.zero;
    private Vector3 recoverTarget = Vector3.zero;

    void Start()
    {
    }

    void Update()
    {
        // Begin movement on LMB
        if (Input.GetMouseButtonDown(0) && !stroking && submerged && resetTimer >= 0.8f && energy.energy > 0f)
        {
            energy.useEnergy(swimCost);
            energy.isActive = true;
            resetTimer = 0f;
            stroking = true;
            initSmoothSpeed = currentSpeed;
            swimSFX.Play();
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
        }

        if (submerged)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;

            Vector3 direction = (mousePos - frogPos.position).normalized;
            frogPos.position += direction * currentSpeed * Time.deltaTime;

            currentSpeed *= Mathf.Exp(-decayRate * Time.deltaTime);
            if (currentSpeed < maxCroakSpeed && !stroking) energy.isActive = false;
            if (CheckLilyCollision())
            {
                energy.isActive = true; // No croaks when recovering
                Debug.Log("Unsubmerging");
                submerged = false;
                currentSpeed = 0f;
                // BEGIN RECOVERY LERP
                recoverTimer = 0f;
                recovering = true;
                initPos = frogPos.position;
                // recover position = frog position + direction * recoverDepth
                float angle = Mathf.Atan2(mousePos.y - initPos.y, mousePos.x - initPos.x) * Mathf.Rad2Deg;
                recoverTarget = new Vector3(initPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * recoverDepth, initPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * recoverDepth, frogPos.position.z);
            }
        }
        else if (recovering)
        {
            recoverTimer += Time.deltaTime / recoverDuration;
            frogPos.position = Vector3.Lerp(initPos, recoverTarget, recoverTimer);
            if (recoverTimer >= 1f)
            {
                recovering = false;
                currentSpeed = 0f;
                recoverTimer = 0f;
                energy.isActive = false; // Reset energy bar
            }
        }

    }
    public bool landCheck(){
        bool onLily = CheckLilyCollision();
        submerged = !onLily;
        if (submerged) splashSFX.Play();
        if (onLily)
        {
            currentSpeed = 0f;
            resetTimer = 2f;
        }
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
            if (hits[i] != null && hits[i].CompareTag("winLand"))
            {
                SceneManager.LoadScene("victory");
            }
        }
        return false;
    }
}
