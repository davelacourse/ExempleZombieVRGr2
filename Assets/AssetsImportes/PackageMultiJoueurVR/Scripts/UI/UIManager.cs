using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{
    // Repr�sente les 3 boutons de notre sc�ne
    [SerializeField] private Button hostButton = default;
    // [SerializeField] private Button serverButton = default;
    [SerializeField] private Button clientButton = default;

    [SerializeField] private string _joinCode;  // Solution temporaire pour se brancher comme client

    private void Start()
    {
        // souscrit � l'�v�nement sur le click du bouton Host et d�marrer la session Host
        hostButton.onClick.AddListener(() => RelayManager.Instance.CreateRelayGame(12));

        // souscrit � l'�v�nement sur le click du bouton Host et d�marrer la session Host
        // Avec le Relay nous n'avons plus besoin d'un serveur
        //serverButton.onClick.AddListener(() => NetworkManager.Singleton.StartServer());

        // souscrit � l'�v�nement sur le click du bouton Host et d�marrer la session Host
        clientButton.onClick.AddListener(() => RelayManager.Instance.JoinRelayGame(_joinCode));
    }
}
