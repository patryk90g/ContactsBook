using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ContactsBook.Startup))]
namespace ContactsBook
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
