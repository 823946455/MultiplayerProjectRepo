using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ShipControl : NetworkBehaviour
{
    [SerializeField] private float shipSpeed = 1f;
    [SerializeField] private AudioSource _thrusters = null;
    [SerializeField] protected GameObject bulletPrefab = null;
    [SerializeField] private AudioSource _bulletSound = null;
    [SerializeField] private AudioClip _blastSound = null;
    [SerializeField] private AudioSource _explosionSound = null;
    
    protected Renderer[] renderers;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private NetworkTransform networkTransform;
    public bool dead = false;
    public NetworkVariable<int> positionOffset = new NetworkVariable<int>();

    

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            AssignPositionOffsetServerRpc();
        }
    }
    private void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
        networkTransform = gameObject.GetComponent<NetworkTransform>();
    }
    private void Update()
    {
        if (!IsOwner) return;

        if (!dead)
        {
            
            HandleMovement();
            HandleShooting();

        }
    }
    private void HandleMovement()
    {
        if (!IsOwner) return;
        float moveDirection = 0f;

        // Player 1 Controls (Arrow Keys)
        if (OwnerClientId == 0)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) moveDirection = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) moveDirection = 1f;
            if (moveDirection > 0 || moveDirection < 0)
            {
                if (!_thrusters.isPlaying)
                    _thrusters.Play();
            }
            else
            {
                _thrusters.Stop();
            }
        }
        // Player 2 Controls (A, D)
        else if (OwnerClientId == 1)
        {
            if (Input.GetKey(KeyCode.A)) moveDirection = -1f;
            if (Input.GetKey(KeyCode.D)) moveDirection = 1f;
            if(moveDirection > 0 || moveDirection < 0)
            {
                if(!_thrusters.isPlaying) _thrusters.Play();
            }
            else
            {
                _thrusters.Stop();
            }
        }

        if (moveDirection != 0)
        {
            Vector3 move = new Vector3(moveDirection * shipSpeed * Time.deltaTime, 0, 0);
            controller.Move(move);
        }
    }
    
    private void HandleShooting()
    {
        // Player 1 (Up Arrow)
        if (OwnerClientId == 0 && Input.GetKeyDown(KeyCode.UpArrow))
        {
            ShootRpc();
        }
        // Player 2 (W Key)
        else if (OwnerClientId == 1 && Input.GetKeyDown(KeyCode.W))
        {
            ShootRpc();
        }
    }
    [Rpc(SendTo.Server)]
    protected void ShootRpc()
    {
        _bulletSound.PlayOneShot(_blastSound);
        Vector3 bulletPos = new Vector3(transform.position.x,transform.position.y,transform.position.z+positionOffset.Value);
        GameObject bullet = Instantiate(bulletPrefab, bulletPos, transform.rotation);
        NetworkObject bulletNetworkObject = bullet.GetComponent<NetworkObject>();
        bulletNetworkObject.Spawn();
        bullet.GetComponent<BulletScript>().SetDirection(transform.forward);
    }
    protected void OnCollisionEnter(Collision collision)
    {
        if (!dead)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
            {
                dead = true;
                _explosionSound.PlayOneShot(_explosionSound.clip);
                Debug.Log($"Ship {gameObject.name} hit! Calling TriggerExplosionRpc()");
                TriggerExplosionRpc(transform.position);
                
            }
        }
    }
    [Rpc(SendTo.Server)]
    private void AssignPositionOffsetServerRpc()
    {
        // Assign different positionOffsets for each client
        if (OwnerClientId == 0)
        {
            positionOffset.Value = 10; // offset for player 1
            
        }
        else if (OwnerClientId == 1)
        {
            positionOffset.Value = -10; // offset for player 2
            
        }
        
    }
    [Rpc(SendTo.Everyone)]
    private void TriggerExplosionRpc(Vector3 explosionPosition)
    {
        GameObject explosion = GameObject.Find("SmallExplosionEffect2");
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }
        explosion.transform.position = explosionPosition;
        explosion.GetComponent<ParticleSystem>().Play();
        
    }
    
}
