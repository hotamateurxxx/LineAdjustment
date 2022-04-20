using NUnit.Framework;
using System;

namespace LineAdjustment.Tests
{
    public class TextMarkerTests
    {
        [Test]
        [TestCase(null, 5, new string[]{})]
        [TestCase("", 5, new string[] {})]
        [TestCase("test", 5, new string[] { "test" })]
        [TestCase("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua", 12,
            new string[] { "Lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit", "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua" })]
        [TestCase("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua", 12,
            new string[] { "dolor", "sit"}, 11, 2)]
        public void EnumerateWordMarkupTest(string input, int lineWidth, string[] expected, int start = 0, int count = 0)
        {
            var marker = new TextMarker(input, lineWidth);
            var list = new System.Collections.Generic.List<string>();
            foreach (var (pos, length) in marker.EnumerateWordMarkup(start, count))
            {
                list.Add(input.AsSpan(pos, length).ToString());
            }
            var actual = list.ToArray();
            Assert.AreEqual(expected, actual);
        }
    }
}