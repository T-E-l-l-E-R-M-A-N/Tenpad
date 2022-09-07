namespace Tenpad.Core.Services
{
    public sealed class HistoryNode
    {
        public HistoryNode Previous { get; set; }
        public HistoryNode Next { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }

        public HistoryNode(string name, string fullName)
        {
            Name = name;
            FullName = fullName;
        }
    }
}