using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;

public class UIJoindreLobby : MonoBehaviour
{
    //Conteneur des boutons
    [SerializeField] private Transform _contentParent = default;
    [SerializeField] private LobbyListElement _lobbyListElementPrefab = default;
    [SerializeField] private float _tempsRafraichissement = 2f;

    private float _timer = 0;  // Variable de temps pour le rafraichissement de la liste

    // Afficher la liste des lobby quand on active le panneau JoindreLobby
    private void OnEnable()
    {
        UpdateLobbyList();
    }

    private void Update()
    {
        // Lance le update à chaque temps de rafraichissement
        if (_timer > _tempsRafraichissement)
        {
            UpdateLobbyList();
            _timer -= _tempsRafraichissement;
        }
        _timer += Time.deltaTime;
    }

    // Méthode qui met à jour la liste des lobbys disponibles 
    public async void UpdateLobbyList()
    {
        //On ajoute un ou des filtres pour rechercher les Lobbys disponibles
        QueryLobbiesOptions queryOptions = new QueryLobbiesOptions();
        
        queryOptions.Count = 10;  // Donne le nombre maximum de Lobby listé
        queryOptions.Order = new List<QueryOrder>(); // change l'ordre d'apparition
        QueryOrder plusRecentenPremier = new QueryOrder(false, QueryOrder.FieldOptions.Created);  //Du plus récent au plus vieux
        
        queryOptions.Order.Add(plusRecentenPremier); // Ajoute l'ordre aux options

        queryOptions.Filters = new List<QueryFilter>(); //Ajout de filtres supplémentaires
        // Filtre personnalisé nommé disponible qui vérifie que les AvailableSlots sont plus grand (GT) que 0
        QueryFilter disponible = new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT);
        // Filtre personnalisé pour vérifé les lobbys non verrouillés 0 = non verrouilé , 1 = verrouillé
        QueryFilter notLocked = new QueryFilter(QueryFilter.FieldOptions.IsLocked, "0", QueryFilter.OpOptions.EQ);

        //ajouter les filtres personnalisé à nos options de tri
        queryOptions.Filters.Add(disponible);
        queryOptions.Filters.Add(notLocked);

        // Interroge le service Lobby pour récupérer tous les lobbys
        QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(queryOptions);

        // On commence par effacer tous les boutons avant de remettre les actuels
        for (int i = 0; i < _contentParent.childCount; i++)
        {
            Destroy(_contentParent.GetChild(i).gameObject);
        }

        // Pour chaque Lobby récupérer dans response
        foreach (var lobby in response.Results)
        {
            // On instancie le prefab à l'intérieur du content et le place dans la variable
            LobbyListElement spawnElement = Instantiate(_lobbyListElementPrefab, _contentParent);
            spawnElement.Initialiser(lobby);
        }
    }
}

