using BoomBox.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XRL;

namespace BoomBox.Scripts.Spotify
{
    [HasModSensitiveStaticCache]
    public static class SpotifyLoader
    {
        public const string REDIRECT_URI = "http://localhost:3000/callback";
        public const string CLIENT_ID = "66fddae670eb48ab8661982cd1ee6b89";

        [ModSensitiveStaticCache]
        public static string token;

        [ModSensitiveCacheInit]
        public static void InitToken()
        {
            if (string.IsNullOrEmpty(token))
            {
                token = getToken();
            }
        }

        public static string getToken()
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
