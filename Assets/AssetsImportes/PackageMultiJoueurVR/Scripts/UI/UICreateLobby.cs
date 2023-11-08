using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICreateLobby : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputNomLobby = default;
    [SerializeField] private Slider _sliderNbJoueurs = default;
    [SerializeField] private TMP_Text _txtNbJoueurs = default;
    [SerializeField] private Button _btnCreateLobby = default;

    private void Start()
    {
        _btnCreateLobby.onClick.AddListener(CreateLobbyFromUI);
        ModifierTexteNbJoueurs();
    }

    private void Update()
    {
        // Rends le bouton actif seulement s'il y a du texte dans le input field
        _btnCreateLobby.gameObject.SetActive(_inputNomLobby.text.Trim() != "");
    }

    public void ModifierTexteNbJoueurs()
    {
        _txtNbJoueurs.text = "Joueurs : " + _sliderNbJoueurs.value.ToString();
    }

    public void CreateLobbyFromUI()
    {
        LobbyManager.LobbyData lobbyData = new LobbyManager.LobbyData();
        lobbyData.maxPlayer = (int)_sliderNbJoueurs.value;
        lobbyData.lobbyName = _inputNomLobby.text;

        LobbyManager.Instance.CreateLobby(lobbyData);
    }
}

