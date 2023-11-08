using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine.Events;

// Pour tester en local avec ParrelSync on ajoute la librairie
#if UNITY_EDITOR
using ParrelSync;
#endif

public class AuthentificationManager : MonoBehaviour
{
    public static AuthentificationManager Instance;  // Cr�ation Singleton
    
    public UnityEvent SignIn;  // Petmet l'appel du Event de l'ext�rieur
    
    private void Awake()
    {
        Instance = this; // Singleton
        Login();
    }

    // identifier comme m�thode asynchrone afin d'utiliser le await 
    public async void Login()
    {
        // Variables qui contient les options d'initialisation
        // Ceci va nous permettre de savoir si nous sommes sur un clone en local
        InitializationOptions options = new InitializationOptions();

// Si on teste � partir de l'�diteur Unity
#if UNITY_EDITOR
        if (ClonesManager.IsClone())
        {
            // Si c'est un close de l'�diteur on re�oit l'info sur ce clone
            options.SetProfile(ClonesManager.GetArgument());
        }
        else
        {
            // Si ce n'est pas le clone on place le profile � primary
            options.SetProfile("primary");
        }
#endif
        // await nous assure que l'initialisation est termin� avant de poursuivre
        // ici je passe les options � l'initialisation pour savoir s'il s'agit du 
        // clone ou non
        await UnityServices.InitializeAsync(options);
        // m�me principe pour le signin
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        SignIn?.Invoke();  // D�clenche l'�v�nement SignIn
    }
}
