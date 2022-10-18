using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpotifyAPI.Web;

namespace DiscoverWeeklySave
{
    public class ProfileModel : PageModel
    {
        private readonly SpotifyClient _spotifyClient;
        public PrivateUser CurrentUser { get; set; }

        public ProfileModel(SpotifyClientBuilder spotifyClientBuilder)
        {
            _spotifyClient = spotifyClientBuilder.BuildClient().Result;
        }

        public async Task OnGet()
        {
            CurrentUser = await _spotifyClient.UserProfile.Current();
        }

        public async Task<IActionResult> OnPost()
        {
            await HttpContext.SignOutAsync();
            return Redirect("https://open.spotify.com/");
        }
    }
}
