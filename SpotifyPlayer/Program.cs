// See https://aka.ms/new-console-template for more information
using SpotifyAPI.Web;
using SpotifyPlayer;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Web;

var redirectUri = "http://localhost:3000/callback";
var clientId = "66fddae670eb48ab8661982cd1ee6b89";
var clientSecret = "1d82a46d62f94d4999475c5381d179c6";

var baseUri = "https://accounts.spotify.com/authorize";
var query = HttpUtility.ParseQueryString(string.Empty);
query["response_type"] = "code";
query["client_id"] = clientId;
query["scope"] = "user-read-private user-read-email user-modify-playback-state user-read-playback-state";
query["redirect_uri"] = redirectUri;

var builder = new UriBuilder(baseUri);
builder.Query = query.ToString();

var uri = builder.ToString();
Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });

var token = SpotifyRedirectListener.StartHttpServer(redirectUri, clientId, clientSecret);

var spotify = new SpotifyClient(token);
var me = await spotify.UserProfile.Current();
Console.WriteLine($"Hello there {me.DisplayName}");

await foreach (
  var playlist in spotify.Paginate(await spotify.Playlists.CurrentUsers())
)
{
    Console.WriteLine(playlist.Name);
}

var playback = await spotify.Player.GetCurrentPlayback();

if (playback.IsPlaying)
{
    var paused = await spotify.Player.PausePlayback();

    Console.WriteLine(paused);
}
else
{
    var played = await spotify.Player.ResumePlayback();
    Console.WriteLine(played);
}