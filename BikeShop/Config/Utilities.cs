using BikeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BikeShop.Config
{
    public abstract class Utilities
    {
        // exceptionnotfound.net/const-vs-static-vs-readonly-in-c-sharp-applications/

        // controllers
        public const string BIKE_CONTROLLER = "Bike";
        public const string PIECE_CONTROLLER = "Piece";
        public const string BIKER_TYPE_CONTROLLER = "BikerType";
        public const string BIKE_CATEGORY_CONTROLLER = "BikeCategory";
        public const string ORDER_CONTROLLER = "Order";

        // actions
        public const string ACTION_INDEX = "Index";
        public const string ACTION_DETAILS = "Details";
        public const string ACTION_NEW = "New";
        public const string ACTION_CREATE = "Create";
        public const string ACTION_EDIT = "Edit";
        public const string ACTION_UPDATE = "Update";
        public const string ACTION_DELETE = "Delete";

        // views
        public const string VIEW_INDEX = "Index";
        public const string VIEW_DETAILS = "Details";
        public const string VIEW_NEW = "New";
        public const string VIEW_EDIT = "Edit";

        // roles
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_SELLER = "Seller";
        public const string ROLE_CLIENT = "Client";

        // other info`s
        public const string BIKES_IMAGES_PATH = "../Images/Bikes/";
        public const string PIECES_IMAGES_PATH = "../Images/Pieces/";


        public static List<CheckBoxModel<Piece>> GetAllPiecesCheckBoxes(ApplicationDbContext ctx, string userId = null, 
            bool reverseSearch = false, bool forSale = false, bool inStoc = false)
        {

            var checkboxList = new List<CheckBoxModel<Piece>>();
            List<Piece> pieceList;
            if(userId != null)
            {
                if (reverseSearch)
                {
                    pieceList = ctx.Pieces.Where(b => b.UserId != userId).ToList();
                }
                else
                {
                    pieceList = ctx.Pieces.Where(b => b.UserId == userId).ToList();
                }
            }
            else
            {
                pieceList = ctx.Pieces.ToList();
            }
            foreach (var piece in pieceList)
            {
                if(((inStoc && piece.Quantity == 0)) || (forSale && !piece.IsIndependent))
                {
                    continue;
                }

                checkboxList.Add(new CheckBoxModel<Piece>
                {
                    Id = piece.PieceId,
                    Name = piece.Name,
                    DisplayName = "  " + piece.Name,
                    Checked = false,
                    Object = piece
                });
            }
            return checkboxList;
        }

        public static List<CheckBoxModel<Bike>> GetAllBikeCheckBoxes(ApplicationDbContext ctx, string userId = null, 
            bool reverseSearch = false, bool inStoc = false)
        {
            var checkBoxList = new List<CheckBoxModel<Bike>>();
            List<Bike> bikeList;
            if (userId != null)
            {
                if (reverseSearch)
                {
                    bikeList = ctx.Bikes.Where(b => b.UserId != userId).ToList();
                }
                else
                {
                    bikeList = ctx.Bikes.Where(b => b.UserId == userId).ToList();
                }
            }
            else
            {
                bikeList = ctx.Bikes.ToList();
            }
            foreach (var bike in bikeList)
            {
                if (inStoc && bike.Quantity == 0)
                {
                    continue;
                }

                checkBoxList.Add(new CheckBoxModel<Bike>
                {
                    Id = bike.BikeId,
                    Name = bike.Name,
                    DisplayName = "  " + bike.Name,
                    Checked = false,
                    Object = bike
                });
            }
            return checkBoxList;
        }

        public static IEnumerable<SelectListItem> GetAllBikerTypes(ApplicationDbContext ctx, string userId = null, bool reverseSearch = false)
        {
            var selectList = new List<SelectListItem>();
            List<BikerType> typeList;
            if (userId != null)
            {
                if (reverseSearch)
                {
                    typeList = ctx.BikerTypes.Where(b => b.UserId != userId).ToList();
                }
                else
                {
                    typeList = ctx.BikerTypes.Where(b => b.UserId == userId).ToList();
                }
            }
            else
            {
                typeList = ctx.BikerTypes.ToList();
            }
            foreach (var type in typeList)
            {
                selectList.Add(new SelectListItem
                {
                    Value = type.BikerTypeId.ToString(),
                    Text = type.Name
                });
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> GetAllBikeCategories(ApplicationDbContext ctx, string userId = null, 
            bool reverseSearch = false)
        {
            var selectList = new List<SelectListItem>();
            List<BikeCategory> categoryList;
            if (userId != null)
            {
                if (reverseSearch)
                {
                    categoryList = ctx.BikeCategories.Where(b => b.UserId != userId).ToList();
                }
                else
                {
                    categoryList = ctx.BikeCategories.Where(b => b.UserId == userId).ToList();
                }
            }
            else
            {
                categoryList = ctx.BikeCategories.ToList();
            }
            foreach (var category in categoryList)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.Name
                });
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> GetBasicOptions()
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem
            {
                Value = "false",
                Text = "Nu"
            });
            selectList.Add(new SelectListItem
            {
                Value = "true",
                Text = "Da"
            });

            return selectList;
        }

    }
}
