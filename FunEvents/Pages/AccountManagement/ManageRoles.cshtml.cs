using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FunEvents.Pages.AccountManagement
{
    //[Authorize(Roles = "Administrator")]
    public class ManageRolesModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
