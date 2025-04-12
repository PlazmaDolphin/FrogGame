using UnityEngine;

public class arcJumper : MonoBehaviour
{
    public WaterMash waterMash;
    private float baseJumpDuration = 0.5f;
    private float baseArcHeight = 0.2f;
    private float heightMultiplier = 0.25f;
    private float timeMultiplier = 0.065f;

    private bool jumping = false;
    private Vector3 start;
    private Vector3 end;
    private float elapsed = 0f;
    private float jumpDuration;
    private float arcHeight;
    public Transform control; // Assign the frog or object to jump in the Inspector
    private Transform jumpTarget;

    public void StartJump(Transform target, Vector3 destination)
    {
        if (jumping) return;

        jumpTarget = target;
        start = target.position;
        end = destination;
        elapsed = 0f;

        float distance = Vector3.Distance(start, end);
        jumpDuration = baseJumpDuration + (distance * timeMultiplier);
        arcHeight = baseArcHeight + (distance * heightMultiplier);

        jumping = true;
    }

    public bool IsJumping()
    {
        return jumping;
    }

    void Update()
    {
        // Debug: Right Mouse Button initiates jump to mouse
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            StartJump(control, worldPos);
        }

        if (!jumping || jumpTarget == null)
            return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / jumpDuration);

        float easedT = Mathf.SmoothStep(0f, 1f, t);
        Vector3 flatPosition = Vector3.Lerp(start, end, easedT);
        float heightOffset = Mathf.Sin(easedT * Mathf.PI) * arcHeight;
        flatPosition.y += heightOffset;

        jumpTarget.position = flatPosition;

        if (t >= 1f)
        {
            jumping = false;
            jumpTarget.position = end;
            waterMash.landCheck(); // Check if the frog is on a lily pad after the jump
        }
    }
}
