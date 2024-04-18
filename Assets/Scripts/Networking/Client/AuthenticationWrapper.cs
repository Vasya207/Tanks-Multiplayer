using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Networking.Client
{
    public static class AuthenticationWrapper
    {
        public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

        public static async Task<AuthState> DoAuth(int maxRetries = 5)
        {
            if (AuthState == AuthState.Authenticated) return AuthState;

            if (AuthState == AuthState.Authenticating)
            {
                Debug.LogWarning("Already authenticating");
                await Authenticating();
                return AuthState;
            }

            await SignInAnonymouslyAsync(maxRetries);
            return AuthState;
        }

        private static async Task<AuthState> Authenticating()
        {
            while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
            {
                await Task.Delay(200);
            }

            return AuthState;
        }

        private static async Task SignInAnonymouslyAsync(int maxRetries)
        {
            AuthState = AuthState.Authenticating;
            int currentRetries = 0;
            
            while (AuthState == AuthState.Authenticating && currentRetries < maxRetries)
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                    if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                    {
                        AuthState = AuthState.Authenticated;
                        return;
                    }
                }
                catch (AuthenticationException authenticationException)
                {
                    Debug.LogError(authenticationException);
                    AuthState = AuthState.Error;
                }
                catch (RequestFailedException requestFailedException)
                {
                    Debug.LogError(requestFailedException);
                    AuthState = AuthState.Error;
                }

                currentRetries++;
                await Task.Delay(1000);
            }

            if (AuthState != AuthState.Authenticated)
            {
                Debug.LogWarning($"Player was not signed in successfully after {currentRetries} retries");
                AuthState = AuthState.TimeOut;
            }
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
