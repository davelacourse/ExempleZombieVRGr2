using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{
    // Représente les 3 boutons de notre scène
    [SerializeField] private Button hostButton = default;
    // [SerializeField] private Button serverButton = default;
    [SerializeField] private Button clientButton = default;

    [SerializeField] private string _joinCode;  // Solution temporaire pour se brancher comme client

    private void Start()
    {
        // souscrit à l'évènement sur le click du bouton Host et démarrer la session Host
        hostButton.onClick.AddListener(() => RelayManager.Instance.CreateRelayGame(12));

        // souscrit à l'évènement sur le click du bouton Host et démarrer la session Host
        // Avec le Relay nous n'avons plus besoin d'un serveur
        //serverButton.onClick.AddListener(() => NetworkManager.Singleton.StartServer());

        // souscrit à l'évènement sur le click du bouton Host et démarrer la session Host
        clientButton.onClick.AddListener(() => RelayManager.Instance.JoinRelayGame(_joinCode));
    }
}
