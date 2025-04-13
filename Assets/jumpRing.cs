using UnityEngine;

public class RingExpander : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform directionIndicator, frogPos; // Assign a small circle sprite in the Inspector
    public arcJumper jumper;
    public WaterMash waterMash; // Assign the water mash script in the Inspector
    public AudioSource chargeSFX, jumpSFX;
    private float thickness = 0.05f;
    private int segments = 100;
    private float expandSpeed = 1.6f;
    private float maxRadius = 2.5f;

    private float radius = 0f;
    private bool expanding = false;
    private Vector3 startPos;
    void Start()
    {
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.positionCount = segments + 1;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;
        lineRenderer.startWidth = thickness;
        lineRenderer.endWidth = thickness;
        lineRenderer.enabled = false;
        if (directionIndicator != null) directionIndicator.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !jumper.IsJumping() && !waterMash.submerged){
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            startPos = transform.position;
            radius = 0f;
            expanding = true;
            lineRenderer.enabled = true;
            if (directionIndicator != null) directionIndicator.gameObject.SetActive(true);
            lineRenderer.startColor = new Color(0.2f, 0.2f, 0.2f, 0.5f); // Transparent grey
            lineRenderer.endColor = new Color(0.2f, 0.2f, 0.2f, 0.5f); // Transparent grey
            UpdateRing();
            if (chargeSFX != null) chargeSFX.Play();
        }

        if (expanding)
        {
            radius += expandSpeed * Time.deltaTime;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Atan2(mouseWorld.y - startPos.y, mouseWorld.x - startPos.x) * Mathf.Rad2Deg;
            if (radius >= maxRadius || Input.GetMouseButtonUp(0)) //go ahead and jump (jump!)
            {
                expanding = false;
                lineRenderer.enabled = false;
                if (directionIndicator != null) directionIndicator.gameObject.SetActive(false);
                if (Input.GetMouseButtonUp(0))
                {
                    // Jump logic here
                    // Example: frogPos.position = new Vector3(startPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius, startPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius, frogPos.position.z);
                    Vector3 target = new Vector3(startPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius*frogPos.localScale.x, startPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius*frogPos.localScale.x -0.4f, frogPos.position.z);
                    jumper.StartJump(frogPos, target);
                    if (jumpSFX != null) jumpSFX.Play();
                    if (chargeSFX != null) chargeSFX.Stop();
                }
            }
            else if (radius >= maxRadius * 0.9f){
                lineRenderer.startColor = new Color(1f, 0.2f, 0.2f, 0.5f); // Transparent red
                lineRenderer.endColor = new Color(1f, 0.2f, 0.2f, 0.5f); // Transparent red
            }
            else if (radius >= maxRadius * 0.65f){
                lineRenderer.startColor = new Color(1f, 1f, 0.2f, 0.5f); // Transparent yellow
                lineRenderer.endColor = new Color(1f, 1f, 0.2f, 0.5f); // Transparent yellow
            }

            UpdateRing();

            if (directionIndicator != null)
            {
                directionIndicator.position = startPos + Quaternion.Euler(0, 0, angle) * Vector3.right * radius * frogPos.localScale.x; // Adjust the scale factor as needed
                //Add a little Z offset to the direction indicator
                directionIndicator.position -= new Vector3(0, 0, 0.1f);
            }
        }
    }

    void UpdateRing()
    {
        for (int i = 0; i <= segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}
