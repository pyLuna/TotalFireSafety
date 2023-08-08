using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

[assembly: OwinStartup(typeof(TotalFireSafety.AuthenticationStartup))]

namespace TotalFireSafety
{
    public class AuthenticationStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //call authorization provider
            var myProvider = new AuthorizationProvider();
            //set auth options
            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/Authenticate/Login"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = myProvider
            };
            //set config on start up
            app.MapSignalR();
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            // Register MVC routes
            //RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Add global MVC authorization filter
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}