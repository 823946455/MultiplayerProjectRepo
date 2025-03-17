using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
[RequireComponent(typeof(NetworkManager))]
public class PlayerPositionSpawner : MonoBehaviour
{
    NetworkManager _networkManager;

    [SerializeField]
    List<Vector3> _spawnPositions = new List<Vector3>() { Vector3.zero};
    [SerializeField]
    List<Quaternion> _spawnRotations = new List<Quaternion>() { Quaternion.identity };

    private int _spawnCount = 0;
    
    private void Awake()
    {
        _networkManager = GetComponent<NetworkManager>();
        _networkManager.ConnectionApprovalCallback += ConnectionApprovalWithSpawnPos;
    }
    void ConnectionApprovalWithSpawnPos(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.CreatePlayerObject = true;
        response.Position = _spawnPositions[Mathf.Min(_spawnCount, _spawnPositions.Count - 1)];
        response.Rotation = _spawnRotations[Mathf.Min(_spawnCount, _spawnRotations.Count-1)];
        response.Approved = true;
        response.Pending = false;
        _spawnCount++;
    }
}
