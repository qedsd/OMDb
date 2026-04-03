using CoreEntry = OMDb.Core.Models.Entry;

namespace OMDb.Maui.Models
{
    public class LabelCollection
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<CoreEntry> Entries { get; set; }
        public object ImageSource { get; set; }
        public int Template { get; set; } = 1;
        public string Id { get; set; }
    }
}
