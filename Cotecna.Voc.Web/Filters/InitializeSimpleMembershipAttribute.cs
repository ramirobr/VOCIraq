using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using Cotecna.Voc.Web.Models;
using System.Linq;
using System.Web.Security;

namespace Cotecna.Voc.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<UsersContext>(null);

                try
                {
                    using (var context = new UsersContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);

                    var roles = (SimpleRoleProvider)Roles.Provider;
                    var membership = (SimpleMembershipProvider)System.Web.Security.Membership.Provider;

                    if (!roles.RoleExists("Admin"))
                        roles.CreateRole("Admin");

                    if (!roles.RoleExists("SuperAdmin"))
                        roles.CreateRole("SuperAdmin");

                    if (!roles.RoleExists("Issuer"))
                        roles.CreateRole("Issuer");

                    if (!roles.RoleExists("Coordinator"))
                        roles.CreateRole("Coordinator");

                    if (!roles.RoleExists("Client"))
                        roles.CreateRole("Client");

                    if (!roles.RoleExists("BorderAgent"))
                        roles.CreateRole("BorderAgent");

                    if (membership.GetUser("admin@cotecna.ch", false) == null)
                        WebSecurity.CreateUserAndAccount("admin@cotecna.ch", "111111", new { IsActive = true });

                    if (!roles.GetRolesForUser("admin@cotecna.ch").Contains("SuperAdmin"))
                        roles.AddUsersToRoles(new[] { "admin@cotecna.ch" }, new[] { "SuperAdmin" });

                    if (membership.GetUser("carlos.chauca@cotecna.com.ec", false) == null)
                        WebSecurity.CreateUserAndAccount("carlos.chauca@cotecna.com.ec", "111111", new { IsActive = true });

                    if (!roles.GetRolesForUser("carlos.chauca@cotecna.com.ec").Contains("SuperAdmin"))
                        roles.AddUsersToRoles(new[] { "carlos.chauca@cotecna.com.ec" }, new[] { "SuperAdmin" });

                    if (membership.GetUser("julie.ferreira@cotecna.ch", false) == null)
                        WebSecurity.CreateUserAndAccount("julie.ferreira@cotecna.ch", "111111", new { IsActive = true });

                    if (!roles.GetRolesForUser("julie.ferreira@cotecna.ch").Contains("SuperAdmin"))
                        roles.AddUsersToRoles(new[] { "julie.ferreira@cotecna.ch" }, new[] { "SuperAdmin" });

                    if (membership.GetUser("christelle.girard@cotecna.com.ec", false) == null)
                        WebSecurity.CreateUserAndAccount("christelle.girard@cotecna.com.ec", "111111", new { IsActive = true });

                    if (!roles.GetRolesForUser("christelle.girard@cotecna.com.ec").Contains("SuperAdmin"))
                        roles.AddUsersToRoles(new[] { "christelle.girard@cotecna.com.ec" }, new[] { "SuperAdmin" });


                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }
    }
}
