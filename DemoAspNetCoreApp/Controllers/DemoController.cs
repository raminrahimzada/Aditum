using Microsoft.AspNetCore.Mvc;

namespace DemoAspNetCoreApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly AppUserService _userService;

        public DemoController(AppUserService userService)
        {
            _userService = userService;
        }

        //TODO for demo
        protected int CurrentUserId => 1;


        [Route("/api/see")]
        [HttpGet]
        public string Granted()
        {
            if (_userService.GetUserPermission(CurrentUserId, Operations.CanSee))
            {
                return "OK,You can see, try /api/update now ";
            }

            return "Oops,You don't have permission to see ";
        }

        [Route("/api/update")]
        [HttpGet]
        public string Denied()
        {
            if (_userService.GetUserPermission(CurrentUserId, Operations.CanUpdate))
            {
                return "OK,you can update";
            }

            return "Oops,You don't have permission to update";
        }
    }
}
