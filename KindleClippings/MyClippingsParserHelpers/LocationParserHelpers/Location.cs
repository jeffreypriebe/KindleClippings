using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindleClippings.MyClippingsParserHelpers.LocationParserHelpers
{
    class Location : LocationBase, ILocation
    {
        protected override string identifierString { get { return " Location "; } }
        
        public Location(Clipping Clipping, string Line)
            : base(Clipping, Line) {}

        protected override void SetLocationValue(string LocationValue)
        {
            clipping.Location = LocationValue;
        }
    }
}
