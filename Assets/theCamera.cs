using UnityEngine;

public class TheCamera : MonoBehaviour
{
    public Transform frogPos;

    void LateUpdate()
    {
        if (frogPos != null)
        {
            transform.position = new Vector3(frogPos.position.x, frogPos.position.y, transform.position.z);
        }
    }
}
