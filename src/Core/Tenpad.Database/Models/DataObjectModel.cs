namespace Tenpad.Database
{
    public class DataObjectModel
    {
        public string Id { get; set; }
        public string Content { get; set; }

        public DataObjectModel(string id, string content)
        {
            Id = id;
            Content = content;
        }
    }
}