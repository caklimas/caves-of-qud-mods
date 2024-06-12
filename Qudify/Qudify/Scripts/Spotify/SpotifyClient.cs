using Qudify.Scripts.Models;
using LitJson;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using XRL.World.Conversations;
using Qudify.Qudify.Scripts.Models.Search;
using Qudify.Qudify.Scripts.Models.ResumePlayback;
using XRL.UI;

namespace Qudify.Scripts.Spotify
{
    internal static class SpotifyClient
    {
        private const string BASE_URL = "https://api.spotify.com/v1/";

        public static void PausePlayback()
        {
            if (SpotifyLoader.GetToken() == null)
            {
                return;
            }

            try
            {
                Console.WriteLine("Pausing spotify player");
                var baseUri = $"{BASE_URL}me/player/pause";

                var builder = new UriBuilder(baseUri);

                var request = getRequest(builder.ToString(), "PUT");
                request.ContentLength = 0;

                // Get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($" Pause Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static void ResumePlayback()
        {
            ResumePlayback(null);
        }

        public static void ResumePlayback(string trackId)
        {
            if (SpotifyLoader.GetToken() == null)
            {
                return;
            }

            try
            {
                Console.WriteLine("Resuming spotify player");
                var baseUri = $"{BASE_URL}me/player/play";

                var builder = new UriBuilder(baseUri);

                var request = getRequest(builder.ToString(), "PUT");

                if (trackId != null)
                {
                    request.ContentType = "application/json";

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        var json = JsonMapper.ToJson(new ResumePlaybackData() { uris = new[] { trackId } });
                        streamWriter.Write(json);
                    }
                }
                else
                {
                    request.ContentLength = 0;
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($" Resume Playback Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static PlaybackStateResponse GetPlaybackState()
        {
            if (SpotifyLoader.GetToken() == null)
            {
                return null;
            }

            try
            {
                Console.WriteLine("Getting playback state");

                var request = getRequest($"{BASE_URL}me/player", "GET");
                request.ContentLength = 0;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                            var responseBody = reader.ReadToEnd();

                            var responsePlayback = JsonMapper.ToObject<PlaybackStateResponse>(responseBody);
                            return responsePlayback;
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return null;
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public static AvailableDevicesResponse GetAvailableDevices()
        {
            if (SpotifyLoader.GetToken() == null || !SpotifyLoader.CheckPremium())
            {
                return new AvailableDevicesResponse();
            }

            try
            {
                Console.WriteLine("Getting available devices");

                var request = getRequest($"{BASE_URL}me/player/devices", "GET");

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                            var responseBody = reader.ReadToEnd();
                            Console.WriteLine($"Playback state: {responseBody}");

                            var responsePlayback = JsonMapper.ToObject<AvailableDevicesResponse>(responseBody);
                            return responsePlayback;
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return null;
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public static void TransferPlayback(string deviceId)
        {
            if (SpotifyLoader.GetToken() == null || !SpotifyLoader.CheckPremium())
            {
                return;
            }

            try
            {
                var request = getRequest($"{BASE_URL}me/player", "PUT");
                request.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var json = JsonMapper.ToJson(new TransferDevice() { device_ids = new[] { deviceId } });
                    streamWriter.Write(json);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($"Transfer Playback Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Transfer Playback Exception: {ex.Message}");
            }
        }

        public static SpotifyAccessToken RefreshAccessToken(string refreshToken)
        {
            if (SpotifyLoader.GetToken() == null)
            {
                return null;
            }

            try
            {
                Console.WriteLine("Refreshing access token");
                var postData = $"grant_type=refresh_token&refresh_token={refreshToken}&client_id={SpotifyLoader.CLIENT_ID}";
                Console.WriteLine($"Post data: {postData}");

                var request = (HttpWebRequest)WebRequest.Create("https://accounts.spotify.com/api/token");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                // Write the request data to the request stream
                var dataBytes = Encoding.UTF8.GetBytes(postData);
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(dataBytes, 0, dataBytes.Length);
                }

                // Send the request and get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseText = reader.ReadToEnd();
                            Console.WriteLine($"Access Token Response: {responseText}");

                            var accessTokenResponse = JsonMapper.ToObject<AccessTokenResponse>(responseText);
                            return new SpotifyAccessToken { AccessToken = accessTokenResponse, Created = DateTime.UtcNow };
                        }
                    }
                    else
                    {
                        // Read the error response for more details
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            var errorResponse = reader.ReadToEnd();
                            Console.WriteLine($"Error Response: {errorResponse}");
                        }

                        throw new Exception($"Failed to get token. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Refresh Token Exception: {ex.Message}");
                throw;
            }
        }

        public static SpotifyUserProfile GetUserProfile()
        {
            if (SpotifyLoader.GetToken() == null)
            {
                return null;
            }

            try
            {
                Console.WriteLine("Getting current user profile");

                var request = getRequest($"{BASE_URL}me", "GET");

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                            var responseBody = reader.ReadToEnd();

                            var responsePlayback = JsonMapper.ToObject<SpotifyUserProfile>(responseBody);
                            return responsePlayback;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"User Profile Error Status: {response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"User Profile Exception: {ex.Message}");
                return null;
            }
        }

        public static void SetVolume(int volumePercent)
        {
            if (SpotifyLoader.GetToken() == null)
            {
                return;
            }

            try
            {
                Console.WriteLine($"Setting volume to {volumePercent}%");
                var baseUri = $"{BASE_URL}me/player/volume";
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["volume_percent"] = volumePercent.ToString();

                var builder = new UriBuilder(baseUri);
                builder.Query = query.ToString();

                var request = getRequest(builder.ToString(), "PUT");
                request.ContentLength = 0;

                // Get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Check the response status code
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($"Set Volume Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Set Volume Exception: {ex.Message}");
            }
        }

        public static void SkipToNext()
        {
            if (SpotifyLoader.GetToken() == null || !SpotifyLoader.CheckPremium())
            {
                return;
            }

            try
            {
                var request = getRequest($"{BASE_URL}me/player/next", "POST");
                request.ContentLength = 0;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($"Skip to Next Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Skip to Next Exception: {ex.Message}");
            }
        }

        public static void SkipToPrevious()
        {
            if (SpotifyLoader.GetToken() == null || !SpotifyLoader.CheckPremium())
            {
                return;
            }

            try
            {
                var request = getRequest($"{BASE_URL}me/player/previous", "POST");
                request.ContentLength = 0;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        Console.WriteLine($"Skip to Previous Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Skip to Previous Exception: {ex.Message}");
            }
        }

        public static SpotifyTracks Search(string query)
        {
            var baseUri = "https://api.spotify.com/v1/search";
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["q"] = query;
            queryString["type"] = "track";

            var builder = new UriBuilder(baseUri);
            builder.Query = queryString.ToString();

            var request = getRequest(builder.ToString(), "GET");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                        var responseBody = reader.ReadToEnd();
                        var tracks = JsonMapper.ToObject<SpotifyTracks>(responseBody);

                        return tracks;
                    }
                }
                else
                {
                    Console.WriteLine($"Search Error Status: {response.StatusCode}");
                    return null;
                }
            }
        }

        private static HttpWebRequest getRequest(string url, string method)
        {
            Console.WriteLine($"Creating request for {url}");

            // Create a HttpWebRequest for the specified URL
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Headers["Authorization"] = "Bearer " + SpotifyLoader.GetToken().AccessToken.access_token;

            return request;
        }
    }
}
