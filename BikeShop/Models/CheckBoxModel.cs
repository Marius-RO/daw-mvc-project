namespace BikeShop.Models
{
    public class CheckBoxModel <T>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Checked { get; set; }
        public T Object { get; set; }
    }
}