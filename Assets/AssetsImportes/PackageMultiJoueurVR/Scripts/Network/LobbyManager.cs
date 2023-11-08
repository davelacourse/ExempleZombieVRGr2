using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Netcode;
using UnityEngine.Events;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance; // Cr�ation du Singleton

    private float _timer = 0f; // Timer pour envoy� le signal au Lobby 
    private float _updateLobbyTimer = 0;  // Timer pour la mise � jour du Lobby

    private Lobby _currentLobby; // Lobby courant
    public Lobby CurrentLobby => _currentLobby;

    public UnityEvent OnStartJoindreLobby;  //�v�nement qui se d�clenche quand je rejoins un serveur
    public UnityEvent OnFailedJoindreLobby;  //�v�nement qui se d�clenche quand la connexion �choue
    
    // Singleton
    private void Awake()
    {
        Instance = this;
    }

    private async void Update()
    {
        if (_timer > 15f)
        {
            _timer -= 15f;

            // Condition pour que seulement le host envoi le ping et non pas tous les joueurs du lobby
            if (_currentLobby != null && _currentLobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                // Envoi un ping au lobby � toutes les 15 secondes
                await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
            }
        }
        _timer += Time.deltaTime;

        // J'update mon lobby � toutes les 1.5s car max rate limits est � 1s
        if (_updateLobbyTimer > 1.5f)
        {
            _updateLobbyTimer -= 1.5f;
            if (_currentLobby != null)
            {
                //V�rifie si on a des donn�es de joueur � mettre � jour
                if(hasPlayerDateUpdate) 
                {
                    // Si on a de nouvelle donn�es on update les infos sur le joueur et remet
                    // le bool�en � vrai
                    UpdatePlayer(newPLayerData);
                    hasPlayerDateUpdate = false;
                }
                else
                {
                    // Mets � jour le Lobby
                    _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
                }
                
            }
        }
        _updateLobbyTimer += Time.deltaTime;
    }

    // Structure complexe qui contient l'information sur un lobby
    public struct LobbyData
    {
        public string lobbyName;
        public int maxPlayer;
    }

    // M�thode publique qui cr�e un lobby sur Unity services
    public async void CreateLobby(LobbyData lobbyData)
    {
        // Je d�clenche l'�v�nement de connection au serveur
        OnStartJoindreLobby.Invoke();

        // Teste si la connexion s'�tablit correctement
        try
        {
            // Il est possible d'ajouter des options au lobby
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();
            lobbyOptions.IsPrivate = false;  // EN placant true un code de connexion serait requis
                                             // Dans les options on peut aussi transmettre des Datas au Lobby
            lobbyOptions.Data = new Dictionary<string, DataObject>();  //ici je cr�e une data de type dictionnaire

            // Je peux me servir de ces data envoy� au lobby pour lui transmettre mon joinCode de mon Relay !
            string joinCode = await RelayManager.Instance.CreateRelayGame(lobbyData.maxPlayer);

            // Je cr�er un DataObject public qui contient le joincode et l'ajoute au dictionnaire
            DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, joinCode);
            lobbyOptions.Data.Add("JoinCodeKey", dataObject);

            // Je cr�er le Lobby sur le Relay
            _currentLobby = await Lobbies.Instance.CreateLobbyAsync(lobbyData.lobbyName, lobbyData.maxPlayer, lobbyOptions);
        }
        // Si une erreur survient je d�clence l'�v�nement OnFailJoindreLobby
        catch(System.Exception e)
        {
            Debug.Log(e.ToString());
            OnFailedJoindreLobby.Invoke();
        }

    }

    // M�thode publique pour la partie rapide qui rejoint le premier Lobby
    public async void PartieRapideLobby()
    {
        // Je d�clenche l'�v�nement de connection au serveur
        OnStartJoindreLobby.Invoke();
        
        // Teste si la connexion s'�tablit correctement
        try
        {
            // R�cup�re le premier lobby dans la variable lobby
            _currentLobby = await Lobbies.Instance.QuickJoinLobbyAsync();
            // Va chercher le Joincode du lobby r�cup�r�
            string relayJoinCode = _currentLobby.Data["JoinCodeKey"].Value;
            
            // Rejoint le lobby avec le joinCode r�cup�r�
            RelayManager.Instance.JoinRelayGame(relayJoinCode);
        }
        // Si une erreur survient je d�clence l'�v�nement OnFailJoindreLobby
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            OnFailedJoindreLobby.Invoke();
        }
    }

    // M�thode publique pour rejoindre un Lobby pr�cis
    public async void RejoindreLobby(string lobbyID)
    {
        // Je d�clenche l'�v�nement de connection au serveur
        OnStartJoindreLobby.Invoke();

        try
        {
            // R�cup�re le lobby avec le ID re�u en param�tre
            _currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyID);
            // Va chercher le Joincode du lobby r�cup�r�
            string relayJoinCode = _currentLobby.Data["JoinCodeKey"].Value;
            
            // Rejoint le lobby avec le joinCode r�cup�r�
            RelayManager.Instance.JoinRelayGame(relayJoinCode);
        }
        // Si une erreur survient je d�clence l'�v�nement OnFailJoindreLobby
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            OnFailedJoindreLobby.Invoke();
        }
    }

    // M�thode utiliser pour mettre � jour l'information sur un joueur dans le Lobby
    // Elle re�oit un dictionnaire avec le nom et la data � modifier
    public async void UpdatePlayer(Dictionary<string, PlayerDataObject> data)
    {
        // Cr�er une variable pour r�cup�rer le dictionnaire transmis
        UpdatePlayerOptions updateOptions = new UpdatePlayerOptions();
        // R�cup�re data et le place dans la variable
        updateOptions.Data = data;
        // Mets � jour les data du joueur dans le Lobby en cours
        _currentLobby = await LobbyService.Instance.UpdatePlayerAsync(_currentLobby.Id, AuthenticationService.Instance.PlayerId, updateOptions);
    }

    private bool hasPlayerDateUpdate = false;
    private Dictionary<string, PlayerDataObject> newPLayerData; 
    
    // M�thode qui va recuellir les infos � update sans faire l'update directement
    public void UpdatePlayerData(Dictionary<string, PlayerDataObject> data)
    {
        newPLayerData = data;
        hasPlayerDateUpdate = true;
    }

    public async void LockLobby()
    {
        _currentLobby = await Lobbies.Instance.UpdateLobbyAsync(_currentLobby.Id, new UpdateLobbyOptions { IsLocked = true });
    }

    
    public async void LeaveLobbyAsync()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.Shutdown(); // Permet de nous d�connecteur du r�seau
        }

        // V�rifie si le lobby est toujours actif
        if (_currentLobby != null)
        {
            string id = _currentLobby.Id; //si oui on note son id
            _currentLobby = null; // on place sa valeur � null se quitte termine l'envoi de signal
            // Enl�ve le joueur du Lobby
            await Lobbies.Instance.RemovePlayerAsync(id, AuthenticationService.Instance.PlayerId);
        }
    }
}

