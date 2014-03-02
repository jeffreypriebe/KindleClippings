using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindleClippings.MyClippingsParserHelpers.LocationParserHelpers
{
    abstract class LocationBase : ILocation
    {
        const char separator = '|';

        protected abstract string identifierString { get; }

        protected Clipping clipping;
        string lineSegment;

        
        public LocationBase(Clipping Clipping, string LineSegment)
        {
            this.clipping = Clipping;
            this.lineSegment = LineSegment;
        }

        public bool IsMatch
        {
            get
            {
                return lineSegment.Contains(identifierString);
            }
        }

        public void SetLocation()
        {
            var IdentifierIndex = lineSegment.IndexOf(identifierString) + identifierString.Length;
            var SeparatorIndex = lineSegment.IndexOf(separator, IdentifierIndex + 1);
            var LocationValue = lineSegment.Substring(IdentifierIndex, SeparatorIndex - IdentifierIndex);
            SetLocationValue(LocationValue);
        }

        protected abstract void SetLocationValue(string LocationValue);
    }
}
