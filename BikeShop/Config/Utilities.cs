using BikeShop.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BikeShop.Config
{
    public abstract class Utilities
    {
        public static readonly int MAX_NAME_FIELD_LENGTH = 20;

        public static readonly string BIKE_CONTROLLER = "Bike";
        public static readonly string PIECE_CONTROLLER = "Piece";
        public static readonly string BIKER_TYPE_CONTROLLER = "BikerType";
        public static readonly string BIKE_CATEGORY_CONTROLLER = "BikeCategory";
        public static readonly string ORDER_CONTROLLER = "Order";
        public static readonly string ACTION_INDEX = "Index";
        public static readonly string ACTION_DETAILS = "Details";
        public static readonly string ACTION_NEW = "New";
        public static readonly string ACTION_CREATE = "Create";
        public static readonly string ACTION_EDIT = "Edit";
        public static readonly string ACTION_UPDATE = "Update";
        public static readonly string ACTION_DELETE = "Delete";

        public static readonly string ROLE_ADMIN = "Admin";
        public static readonly string ROLE_SELLER = "Seller";
        public static readonly string ROLE_CLIENT = "Client";

        public static List<CheckBoxModel<Piece>> GetAllPiecesCheckBoxes(ApplicationDbContext ctx)
        {
            var checkboxList = new List<CheckBoxModel<Piece>>();
            foreach (var piece in ctx.Pieces.ToList())
            {
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

        public static List<CheckBoxModel<Bike>> GetAllBikeCheckBoxes(ApplicationDbContext ctx)
        {
            var checkBoxList = new List<CheckBoxModel<Bike>>();
            foreach (var bike in ctx.Bikes.ToList())
            {
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

        public static IEnumerable<SelectListItem> GetAllBikerTypes(ApplicationDbContext ctx)
        {
            var selectList = new List<SelectListItem>();
            foreach (var type in ctx.BikerTypes.ToList())
            {
                selectList.Add(new SelectListItem
                {
                    Value = type.BikerTypeId.ToString(),
                    Text = type.Name
                });
            }
            return selectList;
        }

        public static IEnumerable<SelectListItem> GetAllBikeCategories(ApplicationDbContext ctx)
        {
            var selectList = new List<SelectListItem>();
            foreach (var category in ctx.BikeCategories.ToList())
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
