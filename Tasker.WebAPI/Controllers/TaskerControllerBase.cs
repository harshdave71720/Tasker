using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tasker.Core.Aggregates.UserAggregate;
using System.Linq;

namespace Tasker.WebAPI.Controllers
{
    public class TaskerControllerBase : ControllerBase
    {
        protected int LoggedUserId { get { return int.Parse(HttpContext.User.FindFirst("AppUserId").Value); } }
    }
}
