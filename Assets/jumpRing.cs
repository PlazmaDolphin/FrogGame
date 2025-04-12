using UnityEngine;

public class RingExpander : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform directionIndicator, frogPos; // Assign a small circle sprite in the Inspector
    private float thickness = 0.05f;
    private int segments = 100;
    private float expandSpeed = 2.5f;
    private float maxRadius = 12f;

    private float radius = 0f;
    private bool expanding = false;
    private Vector3 startPos;
    void Start()
    {
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        //change color to harcoded transparent red
        lineRenderer.startColor = new Color(0.2f, 0.2f, 0.2f, 0.5f); // Transparent grey
        lineRenderer.endColor = new Color(0.2f, 0.2f, 0.2f, 0.5f); // Transparent grey
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
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(Camera.main.transform.position.z);
            startPos = transform.position;
            radius = 0f;
            expanding = true;
            lineRenderer.enabled = true;
            if (directionIndicator != null) directionIndicator.gameObject.SetActive(true);

            UpdateRing();
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
                    frogPos.position = new Vector3(startPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius*frogPos.localScale.x, startPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius*frogPos.localScale.x, frogPos.position.z);
                }
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
