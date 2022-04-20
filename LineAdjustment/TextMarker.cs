using System;
using System.Collections.Generic;

namespace LineAdjustment
{
    public struct TextMarker
    {

        private const char CHAR_SPACE = '\u0020';

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

        /// <summary>
        /// Вычисление количества пробелов между словами в строке.
        /// </summary>
        /// <param name="words_count">Количество слов.</param>
        /// <param name="chars_count">Количество символов.</param>
        /// <param name="line_width">Ширина строки.</param>
        /// <returns>Количесто пробелов в формате (each, first).</returns>
        private static (int each, int first) CalcLineSpaces(int words_count, int chars_count, int line_width)
            => (
                (line_width - chars_count) / (words_count - 1), 
                (line_width - chars_count) % (words_count - 1)
            );

        private readonly string Input;
        private readonly int Width;

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

        /// <summary>
        /// Записать в буфер растянутую линию по её разметке.
        /// </summary>
        /// <param name="buf">Буфер.</param>
        /// <param name="buf_pos">Позиция буфера.</param>
        /// <param name="line">Разметка линии.</param>
        /// <returns>Успешность операции.</returns>
        public bool WriteWideLine(in char[] buf, ref int buf_pos, (int pos, int wcount, int ccount) line)
        {
            if (buf.Length < buf_pos + Width)
                return false;

            Array.Fill(buf, CHAR_SPACE, buf_pos, Width);
            if (line.wcount == 1)
            {
                Input.CopyTo(line.pos, buf, buf_pos, line.ccount);
            }
            else
            {
                var wnum = 0;
                var rpos = 0;
                var (each, first) = CalcLineSpaces(line.wcount, line.ccount, Width);
                foreach (var (wpos, wlength) in EnumerateWordMarkup(line.pos, line.wcount))
                {
                    rpos += wnum > 0
                        ? each + (wnum > first ? 0 : 1)
                        : 0;
                    Input.CopyTo(wpos, buf, buf_pos + rpos, wlength);
                    rpos += wlength;
                    wnum++;
                }
            }
            buf_pos += Width;
            return true;
        }

        /// <summary>
        /// Получить растянутую по ширине строку.
        /// </summary>
        /// <param name="pos">Позиция.</param>
        /// <param name="words_count">Количество слов.</param>
        /// <param name="chars_count">Количество символов.</param>
        /// <returns>Растянутая по ширине строка.</returns>
        public char[] GetWideLine(int pos, int words_count, int chars_count)
        {
            var buf = new char[Width];
            var buf_pos = 0;
            WriteWideLine(buf, ref buf_pos, (pos, words_count, chars_count));
            return buf;
        }

    }

}