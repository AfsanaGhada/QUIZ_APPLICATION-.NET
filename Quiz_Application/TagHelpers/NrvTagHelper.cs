using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Quiz_Application.TagHelpers
{
    [HtmlTargetElement("nrv")] // Recognize <nrv> tag
    public class NrvTagHelper : TagHelper
    {
        public string Img { get; set; } = string.Empty;  // Image source
        public string Text { get; set; } = string.Empty; // Alt text
        public int Width { get; set; } = 100;  // Default width
        public int Height { get; set; } = 100; // Default height

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img"; // Convert <nrv> to <img>
            output.Attributes.SetAttribute("src", Img);
            output.Attributes.SetAttribute("alt", Text);
            output.Attributes.SetAttribute("width", Width);
            output.Attributes.SetAttribute("height", Height);
        }
    }
}
