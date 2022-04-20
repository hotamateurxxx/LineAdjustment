using System;
using System.Collections.Generic;

namespace LineAdjustment
{
    public struct TextIterator
    {

        private const char CHAR_SPACE = '\u0020';

        /// <summary>
        /// Вычисление минимальной ширины строки.
        /// </summary>
        /// <param name="wcount">Количество слов.</param>
        /// <param name="ccount">Количество символов.</param>
        /// <returns>Минимальная ширина строки.</returns>
        private static int CalcLineWidth(int wcount, int ccount)
            => wcount == 0
                ? 0
                : wcount - 1 + ccount;

        /// <summary>
        /// Вычисление количества пробелов между словами в строке.
        /// </summary>
        /// <param name="wcount">Количество слов.</param>
        /// <param name="ccount">Количество символов.</param>
        /// <param name="width">Ширина строки.</param>
        /// <returns>Количесто пробелов в формате (each, first).</returns>
        private static (int each, int first) CalcLineSpaces(int wcount, int ccount, int width)
            => (
                (width - ccount) / (wcount - 1), 
                (width - ccount) % (wcount - 1)
            );

        private readonly string Input;
        private readonly int Width;

        public TextIterator(in string input, in int width)
        {
            Input = input;
            Width = width;
        }

        /// <summary>
        /// Получить итератор слов по входящей строке.
        /// </summary>
        /// <param name="start">Стартовое смещение.</param>
        /// <param name="count">Количество читаемых слов.</param>
        /// <returns>Перечисление слов в формате (позиция, длина).</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IEnumerable<(int pos, int length)> GetWords(int start = 0, int count = 0)
        {
            var found = false;
            var i = start;
            var pos = i;
            var length = Input?.Length ?? 0;
            var retCount = 0;
            while (i < length)
            {
                if (Input[i] == CHAR_SPACE)
                {
                    if (found)
                    {
                        var ccount = i - pos;
                        if (ccount > Width)
                            throw new ArgumentOutOfRangeException();
                        yield return (pos, ccount);
                        if ((count > 0) && (++retCount == count))
                            yield break;
                        found = false;
                    }
                }
                else
                {
                    if (!found)
                    {
                        found = true;
                        pos = i;
                    }
                }
                i++;
            }
            if (found)
                yield return (pos, i - pos);
        }

        /// <summary>
        /// Получить итератор строк по входящей строке.
        /// </summary>
        /// <returns>Перечисление строк в формате (позиция, количество слов, количество символов).</returns>
        public IEnumerable<(int pos, int wcount, int ccount)> GetLines()
        {
            var found = false;
            var pos = 0;
            var wcount = 0;
            var ccount = 0;
            foreach (var (wpos, wlength) in GetWords())
            {
                var newWidth = CalcLineWidth(wcount + 1, ccount + wlength);
                if (newWidth > Width)
                {
                    yield return (pos, wcount, ccount);
                    found = false;
                }
                if (!found)
                {
                    found = true;
                    pos = wpos;
                    wcount = 1;
                    ccount = wlength;
                }
                else
                {
                    wcount++;
                    ccount += wlength;
                }
            }
            if (found)
                yield return (pos, wcount, ccount);
        }

        /// <summary>
        /// Строка на входе (технический метод для контроля).
        /// </summary>
        /// <param name="pos">Позиция начала строки.</param>
        /// <param name="ccount">Количество символов в словах.</param>
        /// <returns>Строка для проверки.</returns>
        public ReadOnlySpan<char> GetSourceLine(int pos, int ccount)
        {
            var i = pos;
            while (ccount > 0)
            {
                if (Input[i] != CHAR_SPACE)
                    ccount--;
                i++;
            }
            return Input.AsSpan(pos, i - pos);
        }

        /// <summary>
        /// Получить растянутую по ширине строку.
        /// </summary>
        /// <param name="pos">Позиция.</param>
        /// <param name="wcount">Количество слов.</param>
        /// <param name="ccount">Количество символов.</param>
        /// <returns>Растянутая по ширине строка.</returns>
        public char[] GetWideLine(int pos, int wcount, int ccount)
        {
            var buf = new char[Width];
            Array.Fill(buf, CHAR_SPACE, 0, Width);
            if (wcount == 1)
            {
                Input.CopyTo(pos, buf, 0, ccount);
            }
            else
            {
                var wnum = 0;
                var rpos = 0;
                var (each, first) = CalcLineSpaces(wcount, ccount, Width);
                foreach (var (wpos, wlength) in GetWords(pos, wcount))
                {
                    rpos += wnum > 0 
                        ? each + (wnum > first ? 0 : 1)
                        : 0;
                    Input.CopyTo(wpos, buf, rpos, wlength);
                    rpos += wlength;
                    wnum++;
                }
            }
            return buf;
        }

    }

}