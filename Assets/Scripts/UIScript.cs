using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Netcode;

public class UIScript : MonoBehaviour
{
    public Text player1Status;
    public Text player2Status;
    private NetworkObject _player;

    void Start()
    {
        InvokeRepeating(nameof(UpdateStatus), 0, 1f);
    }
    void UpdateStatus()
    {
        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            player1Status.text = "Disconnected";
        }
        if(_player == null)
        {
            _player = FindLocalPlayerObject();
        }
        if(_player != null && _player.IsSpawned)
        {
            if (_player.GetComponent<ShipControl>().dead)
            {
                player1Status.text = "Dead";
            }
            else
            {
                player1Status.text = "Alive";
            }
        }
        else
        {
            player1Status.text = "Disconnected";
        }
    }
    private NetworkObject FindLocalPlayerObject()
    {
        foreach (var obj in FindObjectsOfType<NetworkObject>())
        {
            if (obj.IsOwner) return obj;
        }
        return null;
    }
}
