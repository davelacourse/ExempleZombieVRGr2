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
    public static AuthentificationManager Instance;  // Création Singleton
    
    public UnityEvent SignIn;  // Petmet l'appel du Event de l'extérieur
    
    private void Awake()
    {
        Instance = this; // Singleton
        Login();
    }

    // identifier comme méthode asynchrone afin d'utiliser le await 
    public async void Login()
    {
        // Variables qui contient les options d'initialisation
        // Ceci va nous permettre de savoir si nous sommes sur un clone en local
        InitializationOptions options = new InitializationOptions();

// Si on teste à partir de l'éditeur Unity
#if UNITY_EDITOR
        if (ClonesManager.IsClone())
        {
            // Si c'est un close de l'éditeur on reçoit l'info sur ce clone
            options.SetProfile(ClonesManager.GetArgument());
        }
        else
        {
            // Si ce n'est pas le clone on place le profile à primary
            options.SetProfile("primary");
        }
#endif
        // await nous assure que l'initialisation est terminé avant de poursuivre
        // ici je passe les options à l'initialisation pour savoir s'il s'agit du 
        // clone ou non
        await UnityServices.InitializeAsync(options);
        // même principe pour le signin
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        SignIn?.Invoke();  // Déclenche l'évènement SignIn
    }
}
