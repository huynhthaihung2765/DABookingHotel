using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookingHotel.Startup))]
namespace BookingHotel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
