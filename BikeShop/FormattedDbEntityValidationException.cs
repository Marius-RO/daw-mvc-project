using System.Data.Entity.Validation;

namespace BikeShop
{
    internal class FormattedDbEntityValidationException
    {
        private DbEntityValidationException e;

        public FormattedDbEntityValidationException(DbEntityValidationException e)
        {
            this.e = e;
        }
    }
}