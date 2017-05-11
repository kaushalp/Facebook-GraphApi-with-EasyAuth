using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EasyAuthKaushal.Startup))]
namespace EasyAuthKaushal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
