using NUnit.Framework;
using System;

namespace LineAdjustment.Tests
{
    public class TextTrackerTests
    {
        [Test]
        [TestCase(null, 5, new string[]{})]
        [TestCase("", 5, new string[] {})]
        [TestCase("test", 5, new string[] { "test" })]
        [TestCase("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua", 12,
            new string[] { "Lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit", "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua" })]
        [TestCase("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua", 12,
            new string[] { "dolor", "sit"}, 11, 2)]
        public void TraverseWordMarkupTest(string input, int lineWidth, string[] expected, int start = 0, int count = 0)
        {
            var tracker = new TextTracker(input, lineWidth);
            var list = new System.Collections.Generic.List<string>();
            foreach (var (pos, length) in tracker.TraverseWordMarkup(start, count))
            {
                list.Add(input.AsSpan(pos, length).ToString());
            }
            var actual = list.ToArray();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(null, 5, new string[] { })]
        [TestCase("", 5, new string[] { })]
        [TestCase("test", 5, new string[] { "test " })]
        [TestCase("Lorem ipsum", 12, new string[] { "Lorem  ipsum" })]
        [TestCase("Lorem ipsum dolor sit", 12, new string[] { "Lorem  ipsum", "dolor    sit" })]
        [TestCase("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua", 12,
            new string[] { "Lorem  ipsum", "dolor    sit", "amet        ", "consectetur ", "adipiscing  ", "elit  sed do", "eiusmod     ", "tempor      ", "incididunt  ", "ut labore et", "dolore magna", "aliqua      " })]
        public void TraverseWideLineTest(string input, int lineWidth, string[] expected)
        {
            var tracker = new TextTracker(input, lineWidth);
            var list = new System.Collections.Generic.List<string>();
            foreach (var (pos, wcount, ccount) in tracker.TraverseLineMarkup())
            {
                list.Add(new string(tracker.GetWideLine(pos, wcount, ccount)));
            }
            var actual = list.ToArray();
            Assert.AreEqual(expected, actual);
        }
    }
}