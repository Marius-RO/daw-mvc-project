using BikeShop.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

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
            var ctx = new Models.ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));

            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "admin@admin.com";
                user.Email = "admin@admin.com";

                var adminCreated = userManager.Create(user, "admin1234");
                if (adminCreated.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }


            if (!roleManager.RoleExists("Seller"))
            {
          
                var role = new IdentityRole();
                role.Name = "Seller";
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "seller@seller.com";
                user.Email = "seller@seller.com";

                var sellerCreated = userManager.Create(user, "seller1234");
                if (sellerCreated.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Seller");
                }
            }
        }
    }
}
