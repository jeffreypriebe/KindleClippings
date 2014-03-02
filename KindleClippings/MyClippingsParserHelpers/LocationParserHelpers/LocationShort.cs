using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindleClippings.MyClippingsParserHelpers.LocationParserHelpers
{
    class LocationShort : Location, ILocation
    {
        protected override string identifierString { get { return " Loc. "; } }

        public LocationShort(Clipping Clipping, string Line)
            : base(Clipping, Line) { }
    }
}
