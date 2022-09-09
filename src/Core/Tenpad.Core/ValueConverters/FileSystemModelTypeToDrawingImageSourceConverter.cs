using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Tenpad.Core.ValueConverters
{
    internal sealed class FileSystemModelTypeToDrawingImageSourceConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var drawing = new DrawingImage();
            if (value is IFileSystemModel model)
            {
                drawing = model.Type switch
                {
                    FileSystemModelType.Directory => Application.Current.FindResource("DirectoryIconDrawingImage") as DrawingImage,
                    FileSystemModelType.File => Application.Current.FindResource(
                        (model as FileViewModel).Extension switch
                    {
                        ".CS" => "CSIconDrawingImage",
                        ".TXT" => "TXTIconDrawingImage",
                        ".DOC" => "DOCIconDrawingImage",
                        ".HTM" or ".HTML" => "HTMIconDrawingImage",
                        ".XML" => "XMLIconDrawingImage",
                    }) as DrawingImage,
                };
            }
            return drawing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
