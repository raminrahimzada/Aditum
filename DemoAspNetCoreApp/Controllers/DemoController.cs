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


        [Route("/api/granted")]
        [HttpGet]
        public string Granted()
        {
            //demo operation
            const int operationId = 1;
            if (_userService.GetUserPermission(CurrentUserId, operationId))
            {
                return "OK,Go on";
            }

            return "Oops,You don't have permission";
        }

        [Route("/api/denied")]
        [HttpGet]
        public string Denied()
        {
            //demo operation
            const int operationId = 2;
            if (_userService.GetUserPermission(CurrentUserId, operationId))
            {
                return "OK,Go on";
            }

            return "Oops,You don't have permission";
        }
    }
}
