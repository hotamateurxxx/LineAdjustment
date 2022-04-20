using System;
using System.Collections.Generic;

namespace LineAdjustment
{

    public class TextMarker
    {

        protected const char CHAR_SPACE = '\u0020';

        /// <summary>
        /// Вычисление минимальной ширины строки.
        /// </summary>
        /// <param name="words_count">Количество слов.</param>
        /// <param name="chars_count">Количество символов.</param>
        /// <returns>Минимальная ширина строки.</returns>
        private static int CalcLineWidth(int words_count, int chars_count)
            => words_count == 0
                ? 0
                : words_count - 1 + chars_count;

        protected readonly string Input;
        protected readonly int Width;

        public TextMarker(in string input, int width)
        {
            Input = input;
            Width = width;
        }

        /// <summary>
        /// Перечисление информации о разбивке по словам.
        /// </summary>
        /// <param name="start">Стартовое смещение (откуда перечислять).</param>
        /// <param name="count">Количество читаемых слов (сколько перечислять).</param>
        /// <returns>Перечисление в формате (позиция, длина).</returns>
        /// <exception cref="ArithmeticException"></exception>
        public IEnumerable<(int pos, int length)> EnumerateWordMarkup(int start = 0, int count = 0)
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
                            throw new ArithmeticException();
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
        /// Перечисление информации о разбивке по строкам.
        /// </summary>
        /// <returns>Перечисление в формате (позиция, количество слов, количество символов).</returns>
        public IEnumerable<(int pos, int wcount, int ccount)> EnumerateLineMarkup()
        {
            var found = false;
            var pos = 0;
            var wcount = 0;
            var ccount = 0;
            foreach (var (wpos, wlength) in EnumerateWordMarkup())
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

    }

}