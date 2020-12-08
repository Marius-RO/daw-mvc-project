using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BikeShop.Startup))]
namespace BikeShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
