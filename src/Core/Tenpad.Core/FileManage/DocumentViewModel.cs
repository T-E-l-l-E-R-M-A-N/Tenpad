using System.IO;
using System.Threading.Tasks;

namespace Tenpad.Core
{
    public class DocumentViewModel : FileViewModel
    {
        public string Content { get; set; }
        public DocumentViewModel(FileInfo file) : base(file)
        {
            //ReadTextFile(file.FullName);
        }

        private void ReadTextFile(string fullName)
        {
            Content = File.ReadAllText(fullName);
        }
    }
}