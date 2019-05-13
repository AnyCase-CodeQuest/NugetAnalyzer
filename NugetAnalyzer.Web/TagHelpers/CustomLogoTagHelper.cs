using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NugetAnalyzer.Web.TagHelpers
{
    [HtmlTargetElement("custom-logo")]
    public class CustomLogoTagHelper : TagHelper
    {
        public string Color { get; set; }
        public string HoverColor { get; set; }
        public string FontSize { get; set; }

        public CustomLogoTagHelper()
        {
            Color = "rgba(0,0,0,.2)";
            HoverColor = "rgba(0,0,0,.4)";
            FontSize = "14";
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            var styles = $"color:{Color}; " +
                         $"border: 2px solid {Color};" +
                         $"font-size:{FontSize};" +
                         "display:block;" +
                         "border-radius:50%;" +
                         "padding: 3px;";

            output.Attributes.SetAttribute("style", styles);
            output.Attributes.SetAttribute("onmouseover",
                $"this.style.color=\"{HoverColor}\"; this.style.border=\"2px solid {HoverColor}\"");
            output.Attributes.SetAttribute("onmouseout",
                $"this.style.color=\"{Color}\"; this.style.border=\"2px solid {Color}\"");

            output.Content.SetContent("NA");
        }
    }
}
