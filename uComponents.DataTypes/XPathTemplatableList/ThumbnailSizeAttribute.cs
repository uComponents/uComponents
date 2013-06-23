using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uComponents.DataTypes.XPathTemplatableList
{
    public class ThumbnailSizeAttribute : Attribute
    {

        public int Height { get; set; }

        public int Width { get; set; }

        public string CssClass { get; set; }

        public ThumbnailSizeAttribute()
        {
        }
    }
}
