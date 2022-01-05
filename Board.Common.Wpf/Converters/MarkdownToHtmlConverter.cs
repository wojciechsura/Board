using Markdig;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Board.Common.Wpf.Converters
{
    public class MarkdownToHtmlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                value = string.Empty;

            if (value is string sValue)
            {
                var pipeline = new MarkdownPipelineBuilder()
               .Use(new Markdig.Extensions.Tables.PipeTableExtension());

                string html = Markdown.ToHtml(sValue, pipeline.Build());
                string pre, post;

                var preStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Board.Common.Wpf.Resources.Html.markdown-pre.html");
                using (StreamReader sr = new StreamReader(preStream))
                    pre = sr.ReadToEnd();

                var postStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Board.Common.Wpf.Resources.Html.markdown-post.html");
                using (StreamReader sr = new StreamReader(postStream))
                    post = sr.ReadToEnd();

                return $"{pre}{html}{post}";
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
