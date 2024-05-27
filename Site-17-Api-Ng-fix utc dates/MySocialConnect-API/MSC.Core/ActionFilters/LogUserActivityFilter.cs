using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MSC.Core.BusinessLogic;
using MSC.Core.Extensions;

namespace MSC.Core.ActionFilters;

//applied to the base controller
//also, added as a service using extention AddServices
public class LogUserActivityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        //gives us ActionExecutedContext meaning the api action has completed and wll get the result context back from it
        //if we want to something before the action then use ActionExecutingContext as context
        var resultContext = await next();

        //ensure that user is authenicated
        if(!resultContext.HttpContext.User.Identity.IsAuthenticated) 
            return;

        //can also use individual items or full user
        var userName = resultContext.HttpContext.User.GetUserName();
        var guid = resultContext.HttpContext.User.GetGuid();
        var id = resultContext.HttpContext.User.GetId();
        var claims = resultContext.HttpContext.User.GetUserClaims();
        if (claims == null)
            return;

        //get the reference to the user business logic
        var userBl = resultContext.HttpContext.RequestServices.GetRequiredService<IUserBusinessLogic>();
        
        //log activity
        await userBl.LogUserActivityAsync(id);
    }
}
