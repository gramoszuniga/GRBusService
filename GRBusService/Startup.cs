using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GRBusService.Startup))]
namespace GRBusService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
