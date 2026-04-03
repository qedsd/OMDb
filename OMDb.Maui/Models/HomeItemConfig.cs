namespace OMDb.Maui.Models
{
    public class HomeItemConfig
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public HomeItemConfig() { }
        public HomeItemConfig(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
