using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HNStationaryStore.Startup))]
namespace HNStationaryStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
