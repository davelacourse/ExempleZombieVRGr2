using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using UnityEngine.UI;
using TMPro;

public class UISalleAttente : MonoBehaviour
{
    // Variable qui contient la case à cocher
    [SerializeField] private Toggle _isReadyToggle = default;
    // Texte qui affiche le nombre de joueur prêt
    [SerializeField] private TMP_Text _textSalleAttente = default;

    private void Start()
    {
        // Souscrit à l'évènement quand la case change de valeur on appelle la méthode SetReady
        _isReadyToggle.onValueChanged.AddListener(SetReady);
    }

    private void Update()
    {
        Lobby currentLobby = LobbyManager.Instance.CurrentLobby;
        if (currentLobby == null)
        {
            //Si le lobby n'existe pas
            _textSalleAttente.text = "0/0";
            return;
        }
 
        int nbJoueursPret = 0;
        //Pour chaque joueur dans le Lobby on vérifie s'il est prêt
        //Si oui on augmente le compteur
        foreach(var items in currentLobby.Players)
        {
            if(items.Data != null && items.Data.ContainsKey("isReady"))
            {
                if (items.Data["isReady"].Value == "yes")
                {
                    nbJoueursPret++;
                }
            }
        }

        // On affiche le text du nombre de joueurs prêt / le nombre de joueurs dans le lobby
        _textSalleAttente.text = nbJoueursPret + "/" + currentLobby.Players.Count;

        // Comme un veux seulement que ce soit le host du lobby qui call le 
        // début de la partie on s'assure que seulement celui-ci call le changement
        if(currentLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            // Veérifie que les joueurs sont tous prêts
            if (nbJoueursPret == currentLobby.Players.Count)
            {
                //On verrouille le Lobby pour les autres joueurs
                LobbyManager.Instance.LockLobby();
                // Si oui on call la méthode pour changer de scène
                NetworkSceneTransition.Instance.ChargerScenePourTous("Zombie");
            }
        }

    }

    // Méthode publique qui va updater les information du lobby quand le joueur
    // coche la case.
    public void SetReady(bool isReady)
    {
        Lobby currentLobby = LobbyManager.Instance.CurrentLobby;

        // Vérifier s'il existe un Lobby courant
        if(currentLobby != null)
        {
            // Si oui on récupère le PlayerId du joueur dans ce lobby
            string playerId = AuthenticationService.Instance.PlayerId;
            //À l'aide de la fonction Find ici on cherche dans les joueurs qui sont dans le lobby
            //Une valeur x où le Id de x est identique au playerId on place le joueur trouvé dans la variable myPLayer
            Player myPlayer = currentLobby.Players.Find(x => x.Id == playerId);

            // Si on a trouver un joueur avec ce Id
            if(myPlayer != null)
            {
                // Si le joueur n'a pas de Data on crée un nouveau dictionnaire
                // contenant l'information sur le joueur
                if(myPlayer.Data == null)
                {
                    myPlayer.Data = new Dictionary<string, PlayerDataObject>();
                }

                // On crée un DataObject qui va indiquer que le joueur est prêt
                // ici on doit l'indiquer par un string dans le dictionnaire donc on retourne yes si
                // isReady est vrai ou no s'il est faux
                PlayerDataObject isReadyData = new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,
                    isReady ? "yes" : "no");

                // On vérifie si les Data du joueur on bien une key dans le dictionnaire
                // qui se nomme isReady
                if(myPlayer.Data.ContainsKey("isReady"))
                {
                    // Si c'est le cas on assigne la data crée plus haut dans le dictionnaire
                    myPlayer.Data["isReady"] = isReadyData;
                }
                else
                {
                    // Si la clé n'existe pas on en crée une et assigne la valeur
                    myPlayer.Data.Add("isReady", isReadyData);
                }

                // Finalement on update l'information sur le joueur dans le lobby
                // LobbyManager.Instance.UpdatePlayer(myPlayer.Data);

                //Pour éviter le bug on ne change pas durectement les données du joueur
                // On appelle plutot la méthode avec un délai
                LobbyManager.Instance.UpdatePlayerData(myPlayer.Data);

            }
        }
    }
}

