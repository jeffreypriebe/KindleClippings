using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindleClippings.MyClippingsParserHelpers.LocationParserHelpers
{
    interface ILocation
    {        
        void SetLocation();
        bool IsMatch { get; }
    }
}
