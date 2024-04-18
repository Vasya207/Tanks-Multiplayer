using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine.PlayerLoop;

namespace Networking.Client
{
    public static class AuthenticationWrapper
    {
        public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

        public static async Task<AuthState> DoAuth(int maxTries = 5)
        {
            if (AuthState == AuthState.Authenticated) return AuthState;

            AuthState = AuthState.Authenticating;
            int currentTries = 0;
            
            while (AuthState == AuthState.Authenticating && currentTries < maxTries)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticated;
                    break;
                }

                currentTries++;
                await Task.Delay(1000);
            }

            return AuthState;
        }
    }

    public enum AuthState
    {
        NotAuthenticated,
        Authenticating,
        Authenticated,
        Error,
        TimeOut
    }
}
