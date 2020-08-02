using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace web.Pages.Admin
{
    [Authorize(Policy = "AdminPolicy")]
    public class AttributePolicyProtectedModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}