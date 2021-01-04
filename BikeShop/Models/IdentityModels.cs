using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using BikeShop.Config;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BikeShop.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Bike> Bikes { get; set; }
        public DbSet<Piece> Pieces { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DeliveryInfo> DeliveryInfos { get; set; }
        public DbSet<BikerType> BikerTypes { get; set; }
        public DbSet<BikeCategory> BikeCategories { get; set; }

        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new Initp());   
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //public class Initp : DropCreateDatabaseAlways<ApplicationDbContext>
        public class Initp : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
        {
            protected override void Seed(ApplicationDbContext ctx)
            {
                CreateAdminAndUserRoles(ctx, addInitialTestingData: false);

                try
                {
                    ctx.SaveChanges();
                    base.Seed(ctx);
                }
                catch (DbEntityValidationException e)
                {
                    var newException = new FormattedDbEntityValidationException(e);
                    Debug.WriteLine("[Seed] Au fost probleme la adaugarea datelor de test");

                }

            }

            private void CreateAdminAndUserRoles(ApplicationDbContext ctx, bool addInitialTestingData = false)
            {

                RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));
                UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));

                string adminId = "";
                string seller1Id = "";
                string seller2Id = "";
                string client1Id = "";
                string client2Id = "";

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
                        adminId = user.Id;
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
                        seller1Id = user.Id;
                    }

                    // second seller
                    user = new ApplicationUser();
                    user.UserName = "seller_2@seller.bikeshop.ro";
                    user.Email = "seller_2@seller.bikeshop.ro";

                    sellerCreated = userManager.Create(user, "seller1234");
                    if (sellerCreated.Succeeded)
                    {
                        userManager.AddToRole(user.Id, Utilities.ROLE_SELLER);
                        seller2Id = user.Id;
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
                        client1Id = user.Id;
                    }

                    // second client
                    user = new ApplicationUser();
                    user.UserName = "client_2@gmail.com";
                    user.Email = "client_2@gmail.com";

                    sellerCreated = userManager.Create(user, "client1234");
                    if (sellerCreated.Succeeded)
                    {
                        userManager.AddToRole(user.Id, Utilities.ROLE_CLIENT);
                        client2Id = user.Id;
                    }
                }

                if (!roleManager.RoleExists(Utilities.ROLE_ADMIN) ||
                    !roleManager.RoleExists(Utilities.ROLE_SELLER) ||
                    !roleManager.RoleExists(Utilities.ROLE_CLIENT))
                {
                    Debug.WriteLine("[CreateAdminAndUserRoles] Au fost probleme la crearea rolurilor de utilizator");
                    Environment.Exit(-1);
                }

                if (addInitialTestingData)
                {
                    addTestingData(ctx, adminId, "A", 0, 0, 0, 0, "", "", 0, 0, "", "", "");
                    addTestingData(ctx, seller1Id, "S1", 2, 2, 4, 2, client1Id, client2Id, 0, 0, "C1", "C2", "1");
                    addTestingData(ctx, seller2Id, "S2", 4, 4, 8, 4, client2Id, client1Id, 2, 2, "C2", "C1", "2");
                }
            }
        

            private void addTestingData(ApplicationDbContext ctx, string userId, string symbol, int categoriesNr, int typesNr,
                int piecesNr, int bikesNr, string client1Id, string client2Id, int deliveriesNr, int ordersNr, string a, string b, string i)
            {

                categoriesNr++;
                BikeCategory category1 = new BikeCategory
                {
                    CategoryId = categoriesNr,
                    Name = symbol + " - categorie 1",
                    Description = symbol + " - categorie 1",
                    UserId = userId
                };

                categoriesNr++;
                BikeCategory category2 = new BikeCategory
                {
                    CategoryId = categoriesNr,
                    Name = symbol + " - categorie 2",
                    Description = symbol + " - categorie 2",
                    UserId = userId
                };



                typesNr++;
                BikerType type1 = new BikerType
                {
                    BikerTypeId = typesNr,
                    Name = symbol + " - tip 1",
                    Description = symbol + " - tip 1",
                    UserId = userId
                };

                typesNr++;
                BikerType type2 = new BikerType
                {
                    BikerTypeId = typesNr,
                    Name = symbol + " - tip 2",
                    Description = symbol + " - tip 2",
                    UserId = userId
                };

                piecesNr++;
                Piece piece1 = new Piece
                {
                    PieceId = piecesNr,
                    Name = symbol + " - piesa 1 vanzare",
                    Description = symbol + " - piesa 1",
                    FabricationDate = DateTime.Now,
                    Quantity = 4,
                    Price = 100,
                    IsIndependent = true,
                    IsAccessory = false,
                    ImagePath = "../Images/Pieces/piece_" + piecesNr + ".jpg",
                    UserId = userId
                };

                piecesNr++;
                Piece piece2 = new Piece
                {
                    PieceId = piecesNr,
                    Name = symbol + " - piesa 2 interna",
                    Description = symbol + " - piesa 2",
                    FabricationDate = DateTime.Now,
                    Quantity = 5,
                    Price = 50,
                    IsIndependent = false,
                    IsAccessory = true,
                    ImagePath = "../Images/Pieces/piece_" + piecesNr + ".jpg",
                    UserId = userId
                };

                piecesNr++;
                Piece piece3 = new Piece
                {
                    PieceId = piecesNr,
                    Name = symbol + " - piesa 3 interna",
                    Description = symbol + " - piesa 3",
                    FabricationDate = DateTime.Now,
                    Quantity = 5,
                    Price = 50,
                    IsIndependent = false,
                    IsAccessory = false,
                    ImagePath = "../Images/Pieces/piece_" + piecesNr + ".jpg",
                    UserId = userId
                };

                piecesNr++;
                Piece piece4 = new Piece
                {
                    PieceId = piecesNr,
                    Name = symbol + " - piesa 4 vanzare",
                    Description = symbol + " - piesa 4",
                    FabricationDate = DateTime.Now,
                    Quantity = 5,
                    Price = 50,
                    IsIndependent = true,
                    IsAccessory = true,
                    ImagePath = "../Images/Pieces/piece_" + piecesNr + ".jpg",
                    UserId = userId
                };

                bikesNr++;
                Bike bike1 = new Bike
                {
                    BikeId = bikesNr,
                    Name = symbol + " - bike 1",
                    Description = symbol + " - bike 1",
                    FabricationDate = DateTime.Now,
                    Quantity = 5,
                    Price = 90,
                    ImagePath = "../Images/Bikes/bike_" + bikesNr + ".jpg",
                    BikerTypeId = typesNr - 1,
                    BikeCategoryId = categoriesNr - 1,
                    UserId = userId
                };

                bikesNr++;
                Bike bike2 = new Bike
                {
                    BikeId = bikesNr,
                    Name = symbol + " - bike 2",
                    Description = symbol + " - bike 2",
                    FabricationDate = DateTime.Now,
                    Quantity = 5,
                    Price = 90,
                    ImagePath = "../Images/Bikes/bike_" + bikesNr + ".jpg",
                    BikerTypeId = typesNr,
                    BikeCategoryId = categoriesNr,
                    UserId = userId
                };

                bike1.Pieces = new List<Piece>();
                bike1.Pieces.Add(piece1);
                bike1.Pieces.Add(piece3);
                bike2.Pieces = new List<Piece>();
                bike2.Pieces.Add(piece1);
                bike2.Pieces.Add(piece3);

                ctx.BikerTypes.Add(type1);
                ctx.BikerTypes.Add(type2);

                ctx.BikeCategories.Add(category1);
                ctx.BikeCategories.Add(category2);

                ctx.Pieces.Add(piece1);
                ctx.Pieces.Add(piece2);

                ctx.Bikes.Add(bike1);
                ctx.Bikes.Add(bike2);


                if (symbol != "A")
                {

                    deliveriesNr++;
                    DeliveryInfo deliveryInfo1 = new DeliveryInfo
                    {
                        DeliveryInfoId = deliveriesNr,
                        Name = a + " - comanda " + i,
                        PhoneNumber = "0712345678",
                        Address = "adresa " + i
                    };

                    deliveriesNr++;
                    DeliveryInfo deliveryInfo2 = new DeliveryInfo
                    {
                        DeliveryInfoId = deliveriesNr,
                        Name = b + " - comanda " + i,
                        PhoneNumber = "0712345678",
                        Address = "adresa " + i
                    };

                    ctx.DeliveryInfos.Add(deliveryInfo1);
                    ctx.DeliveryInfos.Add(deliveryInfo2);


                    ordersNr++;
                    Order order1 = new Order
                    {
                        OrderId = ordersNr,
                        SellerId = userId,
                        OrderDate = "10.12.2020",
                        OrderValue = 150.5f,
                        DeliveryInfo = deliveryInfo1,
                        UserId = client1Id
                    };

                    ordersNr++;
                    Order order2 = new Order
                    {
                        OrderId = ordersNr,
                        SellerId = userId,
                        OrderDate = "12.12.2020",
                        OrderValue = 250,
                        DeliveryInfo = deliveryInfo2,
                        UserId = client2Id
                    };

                    // go through custom validation
                    order1.Bikes = new List<Bike>();
                    order1.Bikes.Add(bike1);
                    order1.BikesListCheckBoxes = new List<CheckBoxModel<Bike>>();
                    order1.BikesListCheckBoxes.Add(new CheckBoxModel<Bike>
                    {
                        Id = bike1.BikeId,
                        Name = bike1.Name,
                        DisplayName = "  " + bike1.Name,
                        Checked = true,
                        Object = bike1
                    });

                    order1.Pieces = new List<Piece>();
                    order1.Pieces.Add(piece1);
                    order1.Pieces.Add(piece4);
                    order1.PiecesListCheckBoxes = new List<CheckBoxModel<Piece>>();
                    order1.PiecesListCheckBoxes.Add(new CheckBoxModel<Piece>
                    {
                        Id = piece1.PieceId,
                        Name = piece1.Name,
                        DisplayName = "  " + piece1.Name,
                        Checked = true,
                        Object = piece1
                    });
                    order1.PiecesListCheckBoxes.Add(new CheckBoxModel<Piece>
                    {
                        Id = piece2.PieceId,
                        Name = piece2.Name,
                        DisplayName = "  " + piece2.Name,
                        Checked = true,
                        Object = piece2
                    });


                    order2.Bikes = new List<Bike>();
                    order2.Bikes.Add(bike1);
                    order2.Bikes.Add(bike2);
                    order2.BikesListCheckBoxes = new List<CheckBoxModel<Bike>>();
                    order2.BikesListCheckBoxes.Add(new CheckBoxModel<Bike>
                    {
                        Id = bike1.BikeId,
                        Name = bike1.Name,
                        DisplayName = "  " + bike1.Name,
                        Checked = true,
                        Object = bike1
                    });
                    order2.BikesListCheckBoxes.Add(new CheckBoxModel<Bike>
                    {
                        Id = bike2.BikeId,
                        Name = bike2.Name,
                        DisplayName = "  " + bike2.Name,
                        Checked = true,
                        Object = bike2
                    });


                    ctx.Orders.Add(order1);
                    ctx.Orders.Add(order2);

                }

            }
        }
    }
}