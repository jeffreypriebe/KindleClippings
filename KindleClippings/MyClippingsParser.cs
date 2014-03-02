using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using KindleClippings.MyClippingsParserHelpers.LocationParserHelpers;

namespace KindleClippings
{
    public static class MyClippingsParser
    {
        private const string ClippingSeparator = "==========";
        private const string Line1RegexPattern = @"^(.+?)(?: \(([^)]+?)\))?$";
        private const string Line2DateRegexPattern = @"^Added on (.*)$";
        private const string LIne2TypeRegexPattern = @"^- ([^\s]+) .*$";

        public static IEnumerable<Clipping> Parse(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return Parse(stream);
            }
        }

        public static IEnumerable<Clipping> Parse(Stream stream)
        {
            var clippings = new Collection<Clipping>();

            using (var sr = new StreamReader(stream))
            {
                int lineNumber = 0;
                string line = null;
                int clippingLineNumber = 0;
                Clipping clipping = new Clipping();

                try
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        lineNumber++;

                        if (line == ClippingSeparator)
                        {
                            clippings.Add(clipping);
                            clippingLineNumber = 0;
                            clipping = new Clipping();
                        }
                        else
                        {
                            clippingLineNumber++;
                        }

                        switch (clippingLineNumber)
                        {
                            case 1:
                                ParseLine1_AuthorTitle(line, clipping);
                                break;
                            case 2:
                                ParseLine2_Location(line, clipping);
                                break;
                            case 4:
                                ParseLine4_ClippingContent(line, clipping);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error encountered parsing line " + lineNumber + ": " + ex.Message, ex);
                }
            }

            return clippings;
        }

        private static void ParseLine1_AuthorTitle(string line, Clipping clipping)
        {
            var match = Regex.Match(line, Line1RegexPattern);
            if (match.Success)
            {
                var bookName = match.Groups[1].Value.Trim();
                var author = match.Groups[2].Value.Trim();

                clipping.BookName = bookName;
                clipping.Author = author;
            }
            else
            {
                throw new Exception("Clipping Line 1 did not match regex pattern.");
            }
        }

        private static void ParseLine2_Location(string line, Clipping clipping)
        {
            var lineSegmentSplitChar = '|';
            var split = line.Split(lineSegmentSplitChar);

            var lineDateInfo = split.Last();
            var lineLocInfo = String.Join(lineSegmentSplitChar.ToString(), split.Take(split.Length - 1));

            var ParsedClippingType = ParseClippingType(lineLocInfo);
            
            if (ParsedClippingType.HasValue)
                clipping.ClippingType = ParsedClippingType.Value;

            var location = GetLocationInfo(clipping, line);
            location.SetLocation();

            clipping.DateAdded = ParseDateInfo(lineDateInfo);
        }

        private static DateTime ParseDateInfo(string DateInfo)
        {
            var dateAddedR = new Regex(Line2DateRegexPattern, RegexOptions.Compiled);
            var dateAddedString = dateAddedR.Replace(DateInfo.Trim(), "$1");

            return DateTime.Parse(dateAddedString);
        }

        private static ILocation GetLocationInfo(Clipping Clipping, string Line)
        {
            var pageNumber = new PageNumber(Clipping, Line);
            if (pageNumber.IsMatch)
                return pageNumber;

            var location = new Location(Clipping, Line);
            if (location.IsMatch)
                return location;

            var locationShort = new LocationShort(Clipping, Line);
            if (locationShort.IsMatch)
                return locationShort;

            throw new Exception("Location portion of line did not match a known location type.");
        }

        private static ClippingTypeEnum? ParseClippingType(string LineLocationInformation)
        {
            var TypeMatchLocation = new Regex(LIne2TypeRegexPattern, RegexOptions.Compiled);
            var ClippingTypeString = TypeMatchLocation.Replace(LineLocationInformation, "$1");
           
            ClippingTypeEnum ClippingType;
            if (Enum.TryParse(ClippingTypeString, out ClippingType))
                return ClippingType;
            else
                throw new Exception("Location portion of line did not match an expected ClippingType.");
        }

        private static void ParseLine4_ClippingContent(string line, Clipping clipping)
        {
            clipping.Text = line.Trim();
        }
    }
}
