using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class BulletScript : NetworkBehaviour
{
    [SerializeField] protected float speed = 500f; //speed of bullet
    [SerializeField] protected Rigidbody _rigidbody = null; //hook up via inspector for efficiency
    [SerializeField] protected int lifeDuration = 8; //how long should a bullet live before being destroyed

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(DestroyAfterTime(lifeDuration));
        }
    }

    public void Kill(bool byPlayer)
    {
        //Destroy this game object from the scene
        if (IsServer) // Only the server can despawn objects
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Kill(false);
    }
    public void SetDirection(Vector3 direction)
    {
        if (IsServer)
        {
            _rigidbody.linearVelocity = direction * speed;
        }
        else
        {
            GetComponent<BulletInterpolation>().UpdatePosition(transform.position + direction * speed * Time.deltaTime);
        }
        UpdatePositionClientRpc(direction);
        
    }
    [Rpc(SendTo.Everyone)]
    private void UpdatePositionClientRpc(Vector3 direction)
    {
        if (!IsServer) // Clients update position prediction
        {
            _rigidbody.linearVelocity = direction * speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer) // Only the server can despawn objects
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
