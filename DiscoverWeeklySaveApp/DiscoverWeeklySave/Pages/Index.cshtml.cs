using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpotifyAPI.Web;

namespace DiscoverWeeklySave.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SpotifyClient _spotifyClientBuilder;
        public IndexModel(SpotifyClientBuilder spotifyClientBuilder)
        {
            _spotifyClientBuilder = spotifyClientBuilder.BuildClient().Result;
        }
        public async Task OnGet()
        {
            var user = await _spotifyClientBuilder.UserProfile.Current();
            ViewData["Name"] = user.DisplayName;
        }
    }
}


