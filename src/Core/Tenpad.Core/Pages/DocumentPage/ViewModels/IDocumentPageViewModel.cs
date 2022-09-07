using System.Threading.Tasks;
using Prism.Commands;

namespace Tenpad.Core
{
    public interface IDocumentPageViewModel
    {
        FileViewModel ActiveDocument { get; set; }
        string DocumentContent { get; set; }
        DelegateCommand CreateDocumentCommand { get; set; }
        DelegateCommand OpenDocumentCommand { get; set; }
        DelegateCommand SaveDocumentCommand { get; set; }
        DelegateCommand SaveAsDocumentCommand { get; set; }
        DelegateCommand ImportDocumentCommand { get; set; }
        void LoadDocument(FileViewModel fileViewModel);
        void CreateDocument();
    }
}
