using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TinTucMoiNhat.Startup))]
namespace TinTucMoiNhat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
