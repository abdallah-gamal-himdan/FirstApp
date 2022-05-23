using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
namespace API.Helpers
{
    public class ActivityLog : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();
            if(result.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = result.HttpContext.User.GetUserId();
                var repo = result.HttpContext.RequestServices.GetService<IUserRepository>();
                var user = await repo.GetUserByIdAsync(userId);
                user.LastActive = DateTime.Now;
                await repo.SaveAllAsync();
            }
        }
    }
}