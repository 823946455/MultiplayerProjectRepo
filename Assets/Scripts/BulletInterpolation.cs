using Unity.Netcode;
using UnityEngine;

public class BulletInterpolation : NetworkBehaviour
{
    private Vector3 lastPosition;

    void Update()
    {
        if (!IsServer)
        {
            // Smoothly interpolate movement
            transform.position = Vector3.Lerp(transform.position, lastPosition, Time.deltaTime * 10f);
        }
    }

    public void UpdatePosition(Vector3 newPos)
    {
        lastPosition = newPos;
    }
}
