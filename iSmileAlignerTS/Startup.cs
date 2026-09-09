using Microsoft.Owin;
using Owin;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

[assembly: OwinStartupAttribute(typeof(iSmileAlignerTS.Startup))]
namespace iSmileAlignerTS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
