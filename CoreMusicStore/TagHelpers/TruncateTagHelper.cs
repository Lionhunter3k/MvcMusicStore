using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreMusicStore.TagHelpers
{
    [HtmlTargetElement(Attributes = nameof(Length))]
    public class TruncateTagHelper : TagHelper
    {
        public int Length { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("length");
            var input = output.Content.IsModified ? output.Content.GetContent() :
                (await output.GetChildContentAsync()).GetContent();
            if (input.Length <= Length)
            {
                output.Content.SetHtmlContent(input);
            }
            else
            {
                output.Content.SetHtmlContent(input.Substring(0,Length) + "...");
          }
        }
    }
}
