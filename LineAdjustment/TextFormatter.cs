using System;

namespace LineAdjustment
{

    public class TextFormatter : TextMarker
    {

        private const char CHAR_NEWLINE = '\n';
        private const float CAPACITY_MULT = 1.1f;
        private const float CAPACITY_MULT_STEP = .5f;

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

        public TextFormatter(in string input, int width)
            : base(input, width)
        {
        }

        /// <summary>
        /// Записать в буфер растянутую линию по её разметке.
        /// </summary>
        /// <param name="buf">Буфер.</param>
        /// <param name="buf_pos">Позиция буфера.</param>
        /// <param name="line">Разметка линии.</param>
        /// <returns>Успешность операции.</returns>
        private bool TryWriteBuf(in char[] buf, ref int buf_pos, (int pos, int wcount, int ccount) line)
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
        /// Получить растянутый по строкам текст.
        /// </summary>
        /// <param name="capacity_mult">Сколько памяти выделить сразу.</param>
        /// <param name="capacity_step">По сколько добавлять если не хватило.</param>
        /// <returns></returns>
        public string GetWideText(float capacity_mult = CAPACITY_MULT, float capacity_step = CAPACITY_MULT_STEP)
        {
            if ((Input?.Length ?? 0) == 0)
                return string.Empty;

            var buf = new char[(int)(Input.Length * capacity_mult)];
            var i = 0;
            foreach (var (pos, wcount, ccount) in EnumerateLineMarkup())
            {
                if (i > 0)
                    buf[i++] = CHAR_NEWLINE;
                while (!TryWriteBuf(in buf, ref i, (pos, wcount, ccount)))
                {
                    capacity_mult += capacity_step;
                    var buf2 = new char[(int)(Input.Length * capacity_mult)];
                    Array.Copy(buf, buf2, buf.Length);
                    buf = buf2;
                };
            }
            return new string(buf, 0, i);
        }

    }

}