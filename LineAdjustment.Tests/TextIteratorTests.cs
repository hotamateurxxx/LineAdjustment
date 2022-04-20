using NUnit.Framework;
using System;

namespace LineAdjustment.Tests
{
    public class TextIteratorTests
    {
        [Test]
        [TestCase(null, 5, new string[]{})]
        [TestCase("", 5, new string[] {})]
        [TestCase("test", 5, new string[] { "test" })]
        [TestCase("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua", 12,
            new string[] { "Lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit", "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua" })]
        [TestCase("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua", 12,
            new string[] { "dolor", "sit"}, 11, 2)]
        public void GetWordsTest(string input, int lineWidth, string[] expected, int start = 0, int count = 0)
        {
            var iterator = new TextIterator(input, lineWidth);
            var list = new System.Collections.Generic.List<string>();
            foreach (var (pos, length) in iterator.GetWords(start, count))
            {
                list.Add(input.AsSpan(pos, length).ToString());
            }
            var actual = list.ToArray();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(null, 5, new string[] { })]
        [TestCase("", 5, new string[] { })]
        [TestCase("test", 5, new string[] { "test" })]
        [TestCase("Lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna aliqua", 12,
            new string[] { "Lorem ipsum", "dolor sit", "amet", "consectetur", "adipiscing", "elit sed do", "eiusmod", "tempor", "incididunt", "ut labore et", "dolore magna", "aliqua" })]
        public void GetSourceLinesTest(string input, int lineWidth, string[] expected)
        {
            var iterator = new TextIterator(input, lineWidth);
            var list = new System.Collections.Generic.List<string>();
            foreach (var (pos, wcount, ccount) in iterator.GetLines())
            {
                list.Add(iterator.GetSourceLine(pos, ccount).ToString());
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
        public void GetWideLinesTest(string input, int lineWidth, string[] expected)
        {
            var iterator = new TextIterator(input, lineWidth);
            var list = new System.Collections.Generic.List<string>();
            foreach (var (pos, wcount, ccount) in iterator.GetLines())
            {
                list.Add(new string(iterator.GetWideLine(pos, wcount, ccount)));
            }
            var actual = list.ToArray();
            Assert.AreEqual(expected, actual);
        }
    }
}