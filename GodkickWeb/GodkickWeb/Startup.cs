using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GodkickWeb.Startup))]
namespace GodkickWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
