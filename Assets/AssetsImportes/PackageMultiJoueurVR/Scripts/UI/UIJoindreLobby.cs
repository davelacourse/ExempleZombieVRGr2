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
        // Lance le update � chaque temps de rafraichissement
        if (_timer > _tempsRafraichissement)
        {
            UpdateLobbyList();
            _timer -= _tempsRafraichissement;
        }
        _timer += Time.deltaTime;
    }

    // M�thode qui met � jour la liste des lobbys disponibles 
    public async void UpdateLobbyList()
    {
        //On ajoute un ou des filtres pour rechercher les Lobbys disponibles
        QueryLobbiesOptions queryOptions = new QueryLobbiesOptions();
        
        queryOptions.Count = 10;  // Donne le nombre maximum de Lobby list�
        queryOptions.Order = new List<QueryOrder>(); // change l'ordre d'apparition
        QueryOrder plusRecentenPremier = new QueryOrder(false, QueryOrder.FieldOptions.Created);  //Du plus r�cent au plus vieux
        
        queryOptions.Order.Add(plusRecentenPremier); // Ajoute l'ordre aux options

        queryOptions.Filters = new List<QueryFilter>(); //Ajout de filtres suppl�mentaires
        // Filtre personnalis� nomm� disponible qui v�rifie que les AvailableSlots sont plus grand (GT) que 0
        QueryFilter disponible = new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT);
        // Filtre personnalis� pour v�rif� les lobbys non verrouill�s 0 = non verrouil� , 1 = verrouill�
        QueryFilter notLocked = new QueryFilter(QueryFilter.FieldOptions.IsLocked, "0", QueryFilter.OpOptions.EQ);

        //ajouter les filtres personnalis� � nos options de tri
        queryOptions.Filters.Add(disponible);
        queryOptions.Filters.Add(notLocked);

        // Interroge le service Lobby pour r�cup�rer tous les lobbys
        QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(queryOptions);

        // On commence par effacer tous les boutons avant de remettre les actuels
        for (int i = 0; i < _contentParent.childCount; i++)
        {
            Destroy(_contentParent.GetChild(i).gameObject);
        }

        // Pour chaque Lobby r�cup�rer dans response
        foreach (var lobby in response.Results)
        {
            // On instancie le prefab � l'int�rieur du content et le place dans la variable
            LobbyListElement spawnElement = Instantiate(_lobbyListElementPrefab, _contentParent);
            spawnElement.Initialiser(lobby);
        }
    }
}

