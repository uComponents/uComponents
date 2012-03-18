using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uComponents.Core.DataTypes.TabsToDropDownPanel
{

    internal class TabsToDropDownOptions
    {

        public List<int> TabIds { get; set; }


        public TabsToDropDownOptions()
        {
            this.TabIds = new List<int>();
        }
    }
}
