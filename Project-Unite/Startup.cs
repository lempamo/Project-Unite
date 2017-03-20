using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Project_Unite.Startup))]
namespace Project_Unite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
