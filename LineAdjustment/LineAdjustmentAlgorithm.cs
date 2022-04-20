using System;
using System.Collections.Generic;
using System.Text;

namespace LineAdjustment
{
    public class LineAdjustmentAlgorithm
    {



        private const char CHAR_NEWLINE = '\n';
        private const float CAPACITY_MULT = 2;

        /*
        private static string AllocateString(int length)
            => new string(CHAR_SPACE, (int)(length * CAPACITY_MULT));

        private static string Allocate(int length, LineIterator context)
        {
            return string.Create(length, context, (chars, state) =>
            {
                // NOTE: We don't access the context variable in this delegate since 
                // it would cause a closure and allocation.
                // Instead we access the state parameter.

                // will track our position within the string data we are populating
                var position = 0;

                // copy the first string data to index 0 of the Span<char>
                state.FirstString.AsSpan().CopyTo(chars);
                position += state.FirstString.Length; // update the position

                // add a space in the current position and increement position by 1
                chars[position++] = spaceSeparator;

                // copy the second string data to a slice at current position
                state.SecondString.AsSpan().CopyTo(chars.Slice(position));
                position += state.SecondString.Length; // update the position

                // add a space in the current position and increement position by 1
                chars[position++] = spaceSeparator;

                // copy the third string data to a slice at current position
                state.ThirdString.AsSpan().CopyTo(chars.Slice(position));
            });
        }
        */

        public string Transform(string input, int lineWidth)
        {
            var tracker = new TextTracker(input, lineWidth);
            var rval = new StringBuilder();
            foreach (var (pos, wcount, ccount) in tracker.EnumerateLines())
            {
                if (rval.Length > 0)
                    rval.Append(CHAR_NEWLINE);
                rval.Append(tracker.GetWideLine(pos, wcount, ccount));
            }
            return rval.ToString();
        }
    }
}