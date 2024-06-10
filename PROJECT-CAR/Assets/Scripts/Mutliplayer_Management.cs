using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mutliplayer_Management : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>();
    [SerializeField]
    private List<Transform> startingPoints;
    [SerializeField]
    private List<LayerMask> playerLayers;
    private PlayerInputManager PlayerInputManager;

    private void Awake()
    {
        PlayerInputManager = FindAnyObjectByType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        PlayerInputManager.onPlayerJoined += AddPlayer; 
    }

    private void OnDisable()
    {
        PlayerInputManager.onPlayerJoined -= AddPlayer; 
    }

    public void AddPlayer(PlayerInput player)
    {
        players.Add(player);

        Transform playerParent = player.transform.parent;
        playerParent.position = startingPoints[players.Count - 1].position; 

        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count - 1].value, 2);

    }
}
