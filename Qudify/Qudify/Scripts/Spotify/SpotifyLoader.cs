using Qudify.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XRL;
using XRL.UI;

namespace Qudify.Scripts.Spotify
{
    [HasModSensitiveStaticCache]
    public static class SpotifyLoader
    {
        internal const string REDIRECT_URI = "http://localhost:3000/callback";
        internal const string CLIENT_ID = "66fddae670eb48ab8661982cd1ee6b89";

        private static SpotifyAccessToken token;
        private static bool fetchedToken = false;

        internal static SpotifyUserProfile Profile { get; private set; }

        [ModSensitiveCacheInit]
        internal static void InitToken()
        {
            InitToken(false);
        }

        internal static void InitToken(bool forceInit)
        {
            if (token == null && (!fetchedToken || forceInit))
            {
                fetchedToken = true;
                token = Init();

                if (token != null)
                {
                    Profile = SpotifyClient.GetUserProfile();
                }
            }
        }

        internal static SpotifyAccessToken GetToken()
        {
            if (token == null)
            {
                return token;
            }

            var difference = DateTime.UtcNow - token.Created;
            if (difference < TimeSpan.FromHours(1))
            {
                return token;
            }

            var refreshToken = token.AccessToken.refresh_token;
            var refreshedToken = SpotifyClient.RefreshAccessToken(refreshToken);
            token = refreshedToken;

            return token;
        }

        internal static bool IsLoggedIn() { return token != null; }

        internal static bool ValidateRequest()
        {
            return ValidateRequest(validateSelectDevices: true);
        }

        internal static bool ValidateRequest(bool validateSelectDevices)
        {
            if (!CheckPremium())
            {
                Popup.Show("You need to have a Premium account to do this action.");
                return false;
            }

            if (validateSelectDevices && !SelectedSpotifyDevice.HasSelectedDevice()) {
                Popup.Show("You need to select a device to do this action.");
                return false;
            }


            return true;
        }

        private static bool CheckPremium()
        {
            if (!Profile.IsPremium)
            {
                Popup.Show("You need to have a Premium account to do this action.");
                return false;
            }

            return true;
        }

        private static SpotifyAccessToken Init()
        {
            PkceCode.Init();

            var baseUri = "https://accounts.spotify.com/authorize";
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["response_type"] = "code";
            query["client_id"] = CLIENT_ID;
            query["scope"] = "user-read-private user-read-email user-modify-playback-state user-read-playback-state ugc-image-upload";
            query["redirect_uri"] = REDIRECT_URI;
            query["code_challenge_method"] = "S256";
            query["code_challenge"] = PkceCode.CodeChallenge;

            var builder = new UriBuilder(baseUri);
            builder.Query = query.ToString();

            var uri = builder.ToString();
            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });

            var token = SpotifyRedirectListener.StartHttpServerAsync(REDIRECT_URI, CLIENT_ID, PkceCode.CodeVerifier);
            return token;
        }
    }
}
