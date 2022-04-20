using System;

namespace LineAdjustment
{
    public class LineAdjustmentAlgorithm
    {

        private const char CHAR_NEWLINE = '\n';
        private const float CAPACITY_MULT = 1.1f;
        private const float CAPACITY_MULT_STEP = .5f;

        public string Transform(string input, int lineWidth)
        {
            var tracker = new TextTracker(input, lineWidth);
            var mult = CAPACITY_MULT;
            var length = input?.Length ?? 0;
            var buf = new char[(int)(length * mult)];
            var i = 0;
            foreach (var (pos, wcount, ccount) in tracker.TraverseLineMarkup())
            {
                if (i > 0)
                    buf[i++] = CHAR_NEWLINE;
                while (!tracker.WriteWideLine(in buf, ref i, (pos, wcount, ccount)))
                {
                    mult += CAPACITY_MULT_STEP;
                    var buf2 = new char[(int)(length * mult)];
                    Array.Copy(buf, buf2, buf.Length);
                    buf = buf2;
                };
            }
            return new string(buf, 0, i);
        }
    }
}