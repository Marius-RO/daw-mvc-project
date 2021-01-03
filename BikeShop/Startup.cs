using BikeShop.Config;
using BikeShop.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.Diagnostics;

[assembly: OwinStartupAttribute(typeof(BikeShop.Startup))]
namespace BikeShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateAdminAndUserRoles();
        }

        private void CreateAdminAndUserRoles()
        {

            var ctx = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));

            if (!roleManager.RoleExists(Utilities.ROLE_ADMIN))
            {
                var role = new IdentityRole();
                role.Name = Utilities.ROLE_ADMIN;
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "admin_1@admin.bikeshop.ro";
                user.Email = "admin_1@admin.bikeshop.ro";

                var adminCreated = userManager.Create(user, "admin1234");
                if (adminCreated.Succeeded)
                {
                    userManager.AddToRole(user.Id, Utilities.ROLE_ADMIN);
                }
            }


            if (!roleManager.RoleExists(Utilities.ROLE_SELLER))
            {
          
                var role = new IdentityRole();
                role.Name = Utilities.ROLE_SELLER;
                roleManager.Create(role);

                // first seller
                var user = new ApplicationUser();
                user.UserName = "seller_1@seller.bikeshop.ro";
                user.Email = "seller_1@seller.bikeshop.ro";

                var sellerCreated = userManager.Create(user, "seller1234");
                if (sellerCreated.Succeeded)
                {
                    userManager.AddToRole(user.Id, Utilities.ROLE_SELLER);
                }

                // second seller
                user = new ApplicationUser();
                user.UserName = "seller_2@seller.bikeshop.ro";
                user.Email = "seller_2@seller.bikeshop.ro";

                sellerCreated = userManager.Create(user, "seller1234");
                if (sellerCreated.Succeeded)
                {
                    userManager.AddToRole(user.Id, Utilities.ROLE_SELLER);
                }
            }


            if (!roleManager.RoleExists(Utilities.ROLE_CLIENT))
            {
                var role = new IdentityRole();
                role.Name = Utilities.ROLE_CLIENT;
                roleManager.Create(role);

                // first client
                var user = new ApplicationUser();
                user.UserName = "client_1@yahoo.com";
                user.Email = "client_1@yahoo.com";

                var sellerCreated = userManager.Create(user, "client1234");
                if (sellerCreated.Succeeded)
                {
                    userManager.AddToRole(user.Id, Utilities.ROLE_CLIENT);
                }

                // second client
                user = new ApplicationUser();
                user.UserName = "client_2@gmail.com";
                user.Email = "client_2@gmail.com";

                sellerCreated = userManager.Create(user, "client1234");
                if (sellerCreated.Succeeded)
                {
                    userManager.AddToRole(user.Id, Utilities.ROLE_CLIENT);
                }
            }

            if (!roleManager.RoleExists(Utilities.ROLE_ADMIN) || 
                !roleManager.RoleExists(Utilities.ROLE_SELLER) ||
                !roleManager.RoleExists(Utilities.ROLE_CLIENT))
            {
                Debug.WriteLine("[CreateAdminAndUserRoles] Au fost probleme la crearea rolurilor de utilizator");
                Environment.Exit(-1);
            }

        }
    }
}
