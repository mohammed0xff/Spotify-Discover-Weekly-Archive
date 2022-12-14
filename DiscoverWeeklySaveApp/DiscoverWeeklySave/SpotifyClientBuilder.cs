using Microsoft.AspNetCore.Authentication;
using SpotifyAPI.Web;

namespace DiscoverWeeklySave
{
    public class SpotifyClientBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SpotifyClientConfig _spotifyClientConfig;

        public SpotifyClientBuilder(IHttpContextAccessor httpContextAccessor, SpotifyClientConfig spotifyClientConfig)
        {
            _httpContextAccessor = httpContextAccessor;
            _spotifyClientConfig = spotifyClientConfig;
        }

        public async Task<SpotifyClient> BuildClient()
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("Spotify", "access_token");
            if (token == null) throw new Exception("Invalid Token");
            return new SpotifyClient(_spotifyClientConfig.WithToken(token));
        }
    }
}
