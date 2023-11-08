using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyListElement : MonoBehaviour
{
    [SerializeField] private Button _joinButton = default;
    [SerializeField] private TMP_Text _txtLobbyName = default;
    [SerializeField] private TMP_Text _txtJoueurs = default;
    private string _lobbyID;

    private void Start()
    {
        _joinButton.onClick.AddListener(() => LobbyManager.Instance.RejoindreLobby(_lobbyID));
    }

    public void Initialiser(Lobby lobby)
    {
        _txtLobbyName.text = lobby.Name;
        _txtJoueurs.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        _lobbyID = lobby.Id;
    }
}


