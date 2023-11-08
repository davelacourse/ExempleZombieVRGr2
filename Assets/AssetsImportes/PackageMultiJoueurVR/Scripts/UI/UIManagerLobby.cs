using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class UIManagerLobby : MonoBehaviour
{
    [Header("Panneaux UI")]
    [SerializeField] private GameObject _authentification = default;
    [SerializeField] private GameObject _menuLobby = default;
    [SerializeField] private GameObject _creerLobby = default;
    [SerializeField] private GameObject _rejoindreLobby = default;
    [SerializeField] private GameObject _salleAttente = default;
    [SerializeField] private GameObject _chargement = default;

    [Header("Boutons")]
    [SerializeField] private Button _partieRapideButton = default;
    [SerializeField] private Button _creerLobbyButton = default;
    [SerializeField] private Button _rejoindreLobbyButton = default;
    [SerializeField] private Button _retourJoindreButton = default;
    [SerializeField] private Button _retourCreerButton = default;
    [SerializeField] private Button _retourSalleAttente = default;

    private void Start()
    {
        ActiverUI(3); // Affiche le panneau d'authentification au départ
        // Appeler l'évènement SignIn de l'authentification
        AuthentificationManager.Instance.SignIn.AddListener(() => ActiverUI(0));

        // Rejoint le premier Lobby Actif
        _partieRapideButton.onClick.AddListener(() => LobbyManager.Instance.PartieRapideLobby());

        _creerLobbyButton.onClick.AddListener(() => ActiverUI(1));
        _rejoindreLobbyButton.onClick.AddListener(() => ActiverUI(2));
        _retourCreerButton.onClick.AddListener(() => ActiverUI(0));
        _retourJoindreButton.onClick.AddListener(() => ActiverUI(0));
        _retourSalleAttente.onClick.AddListener(() => ActiverUI(0));

        // Quand un joueur se connecte on appelle la méthode OnClientConnected
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        // Si un chargement s'effectue sur le Lobby j'active le panneau de chargement
        LobbyManager.Instance.OnStartJoindreLobby.AddListener(() => ActiverUI(5));
        // Si une erreur de connexion se produit sur le lobby je retourne au menu initial
        LobbyManager.Instance.OnFailedJoindreLobby.AddListener(() => ActiverUI(0));
    }

    // Cette méthode est appeler par tous les clients et l'host lors de la déconnexion
    private void OnClientDisconnected(ulong obj)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            QuitterLobbyUI();
        }
    }

    public void QuitterLobbyUI()
    {
        ActiverUI(0);
        LobbyManager.Instance.LeaveLobbyAsync();
    }

    private void OnClientConnected(ulong obj)
    {
        // Vérifie que le joueur qui vient de se connecter est bien le joueur local
        // si oui active le panneau de la salle d'attente
        if (obj == NetworkManager.Singleton.LocalClientId)
        {
            ActiverUI(4);
        }
    }

    // Permet d'activer le gameObject correspondant à l'index
    // et de déastiver les autres
    public void ActiverUI(int index)
    {
        GameObject[] uiElements = new GameObject[] { _menuLobby, _creerLobby, _rejoindreLobby, _authentification, _salleAttente, _chargement};
        for (int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].SetActive(i == index);
        }
    }
}





