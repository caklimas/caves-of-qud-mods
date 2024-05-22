// See https://aka.ms/new-console-template for more information
using SpotifyAPI.Web;
using SpotifyPlayer;
using System.Diagnostics;
using System.Web;

var redirectUri = "http://localhost:3000/callback";
var clientId = "66fddae670eb48ab8661982cd1ee6b89";
var clientSecret = "1d82a46d62f94d4999475c5381d179c6";

var baseUri = "https://accounts.spotify.com/authorize";
var query = HttpUtility.ParseQueryString(string.Empty);
query["response_type"] = "code";
query["client_id"] = clientId;
query["scope"] = "user-read-private user-read-email";
query["redirect_uri"] = redirectUri;

var builder = new UriBuilder(baseUri);
builder.Query = query.ToString();

var uri = builder.ToString();
Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });

var token = await SpotifyRedirectListener.StartHttpServerAsync(redirectUri, clientId, clientSecret);

var client = new SpotifyClient(token);
await client.Player.PausePlayback();

Console.WriteLine($"Hello there");
