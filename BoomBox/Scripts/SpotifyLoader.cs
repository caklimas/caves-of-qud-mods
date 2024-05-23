using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XRL;

namespace BoomBox.Scripts
{
    [HasModSensitiveStaticCache]
    public static class SpotifyLoader
    {
        [ModSensitiveStaticCache]
        public static String token;

        [ModSensitiveCacheInit]
        public static void InitToken()
        {
            token = getToken();
        }

        public static string getToken()
        {
            var redirectUri = "http://localhost:3000/callback";
            var clientId = "66fddae670eb48ab8661982cd1ee6b89";
            var clientSecret = "1d82a46d62f94d4999475c5381d179c6";

            var baseUri = "https://accounts.spotify.com/authorize";
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["response_type"] = "code";
            query["client_id"] = clientId;
            query["scope"] = "user-read-private user-read-email user-modify-playback-state user-read-playback-state ugc-image-upload";
            query["redirect_uri"] = redirectUri;

            var builder = new UriBuilder(baseUri);
            builder.Query = query.ToString();

            var uri = builder.ToString();
            Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });

            var token = SpotifyRedirectListener.StartHttpServerAsync(redirectUri, clientId, clientSecret);
            return token;
        }
    }
}
