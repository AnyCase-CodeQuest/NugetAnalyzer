using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NugetAnalyzer.Web.TagHelpers
{
    [HtmlTargetElement("logo")]
    public class CustomLogoTagHelper : TagHelper
    {
        public string FillColor { get; set; }
        public string HoverFillColor { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        public CustomLogoTagHelper()
        {
            FillColor = "#fff";
            HoverFillColor = "hsla(0,0%,100%,.7)";
            Width = "32";
            Height = "32";
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "svg";
            output.TagMode = TagMode.StartTagAndEndTag;

            var styles = $"fill:{FillColor}; " +
                         $"width:{Width}; " +
                         $"height:{Height}; ";

            output.Attributes.SetAttribute("style", styles);
            output.Attributes.SetAttribute("xmlns", @"http://www.w3.org/2000/svg");
            output.Attributes.SetAttribute("version", "1.1");
            output.Attributes.SetAttribute("viewBox", "0 0 512 512");

            output.Attributes.SetAttribute("onmouseover",
                $"this.style.fill=\"{HoverFillColor}\"; ");
            output.Attributes.SetAttribute("onmouseout",
                $"this.style.fill=\"{FillColor}\"; ");

            output.Content.SetHtmlContent("<path d=\"M485.097, 80.1H291.945c-3.864, 0-7.611-1.394-10.544-3.92l-35.432-30.566c-6.843-5.894-15.578-9.14-24.596-9.14H94.423    " +
                                          "c-14.834,0-26.903,12.069-26.903,26.903v35.957H26.903C12.069,99.334,0,111.403,0,126.237v322.386    " +
                                          "c0,14.834,12.069,26.903,26.903,26.903h390.673c14.834,0,26.903-12.069,26.903-26.903v-35.949h40.618    " +
                                          "c14.834,0,26.903-12.069,26.903-26.903V107.003C512,92.168,499.931,80.1,485.097,80.1zM422.957,448.623    " +
                                          "c0,2.967-2.414,5.381-5.381,5.381H26.903c-2.967,0-5.381-2.414-5.381-5.381V227.428h401.434V448.623zM422.957,205.906H21.522    " +
                                          "v-79.668c0-2.967,2.414-5.381,5.381-5.381h126.943c3.869,0,7.612,1.391,10.542,3.919l35.434,30.559    " +
                                          "c6.836,5.895,15.571,9.141,24.598,9.141h193.156c2.967,0,5.381,2.414,5.381,5.381V205.906zM490.478,385.771    " +
                                          "c0,2.967-2.414,5.381-5.381,5.381h-40.618V169.856c0-14.834-12.069-26.903-26.903-26.903H224.42    " +
                                          "c-3.869,0-7.614-1.391-10.543-3.918l-35.433-30.557c-6.836-5.896-15.571-9.143-24.598-9.143H89.042V63.377    " +
                                          "c0-2.967,2.414-5.381,5.381-5.381h126.95c3.864,0,7.611,1.394,10.544,3.92l35.432,30.566c6.843,5.893,15.578,9.139,24.596,9.139    " +
                                          "h193.153c2.967,0,5.381,2.414,5.381,5.381V385.771z\"/>");
        }
    }
}
