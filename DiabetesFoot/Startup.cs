using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DiabetesFoot.Startup))]
namespace DiabetesFoot
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
