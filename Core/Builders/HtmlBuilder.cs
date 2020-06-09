using System.Drawing;
using System.Text;
using Core.Extensions;

namespace Core.Builders
{
    /// <summary>
    /// Создание HTML с помощью fluent interface
    /// </summary>
    public class HtmlBuilder
    {
        /// <summary>
        /// Шаблоны HTML кода
        /// </summary>
        private const string HeaderTpl = "<h{1}>{0}</h{1}>";
        private const string BeginUnderlineTpl = "<u>";
        private const string EndUnderlineTpl = "</u>";
        private const string BeginBoldTpl = "<b>";
        private const string EndBoldTpl = "</b>";
        private const string BrTpl = "{0}<br/>";
        private const string BeginFormatTpl = "<font color='{0}' size='{1}'>";
        private const string EndFormatTpl = "</font>";
        private const string BeginUlTpl = "<ul>";
        private const string EndUlTpl = "</ul>";
        private const string BeginOlTpl = "<ol>";
        private const string EndOlTpl = "</ol>";
        private const string LiTpl = "<li>{0}</li>";

        /// <summary>
        /// HTML код
        /// </summary>
        private readonly StringBuilder _content = new StringBuilder();

        /// <summary>
        /// Добавление простого текста
        /// </summary>
        /// <param name="text">Текст</param>
        /// <returns>Контекст</returns>
        public HtmlBuilder Text(string text)
        {
            _content.AppendFormat(BrTpl,text);
            return this;
        }

        /// <summary>
        /// Добавление простого текста с параметрами
        /// </summary>
        /// <param name="text">Текст с {}</param>
        /// <param name="args">Аргументы</param>
        /// <returns>Контекст</returns>
        public HtmlBuilder Text(string text, params object[] args)
        {
            _content.AppendFormat(BrTpl, text.F(args));
            return this;
        }

        /// <summary>
        /// Добавление заголовка H1-H6
        /// </summary>
        /// <param name="text">Текст заголовка</param>
        /// <param name="level">Уровень (1-6)</param>
        /// <returns>Контекст</returns>
        public HtmlBuilder Header(string text, int level = 1)
        {
            _content.AppendFormat(HeaderTpl, text, level);
            return this;
        }

        /// <summary>
        /// Начало добавления подчеркнутого текста
        /// </summary>
        /// <returns>Контекст</returns>
        public HtmlBuilder BeginUnderline()
        {
            _content.Append(BeginUnderlineTpl);
            return this;
        }

        /// <summary>
        /// Конеч подчеркнутого текста
        /// </summary>
        /// <returns>Контекст</returns>
        public HtmlBuilder EndUnderline()
        {
            _content.Append(EndUnderlineTpl);
            return this;
        }

        /// <summary>
        /// Начало выделенного текста
        /// </summary>
        /// <returns>Контекст</returns>
        public HtmlBuilder BeginBold()
        {
            _content.Append(BeginBoldTpl);
            return this;
        }

        /// <summary>
        /// Конец выделенного текста
        /// </summary>
        /// <returns>Контекст</returns>
        public HtmlBuilder EndBold()
        {
            _content.Append(EndBoldTpl);
            return this;
        }

        /// <summary>
        /// Начало форматированного текста
        /// </summary>
        /// <param name="size">Размер (1-7), по умолчанию 3</param>
        /// <param name="color">Цвет, по умолчанию Black</param>
        /// <returns>Контекст</returns>
        public HtmlBuilder BeginFormat(int? size = null, Color? color = null)
        {
            if (!color.HasValue)
            {
                color = Color.Black;
            }
            if (!size.HasValue)
            {
                size = 3;
            }

            _content.AppendFormat(BeginFormatTpl, string.Format("#{0:X2}{1:X2}{2:X2}", color.Value.R, color.Value.G, color.Value.B), size);
            return this;
        }

        /// <summary>
        /// Конец форматированного текста
        /// </summary>
        /// <returns>Контекст</returns>
        public HtmlBuilder EndFormat()
        {
            _content.Append(EndFormatTpl);
            return this;
        }

        /// <summary>
        /// Начало пронумерованного списка
        /// </summary>
        /// <returns>Контекст</returns>
        public HtmlBuilder BeginNumericList()
        {
            _content.Append(BeginOlTpl);
            return this;
        }

        /// <summary>
        /// Конец пронумерованного списка
        /// </summary>
        /// <returns>Контекст</returns>
        public HtmlBuilder EndNumericList()
        {
            _content.Append(EndOlTpl);
            return this;
        }

        /// <summary>
        /// Начало списка
        /// </summary>
        /// <returns>Контекст</returns>
        public HtmlBuilder BeginList()
        {
            _content.Append(BeginUlTpl);
            return this;
        }

        /// <summary>
        /// Конец списка
        /// </summary>
        /// <returns>Контекст</returns>
        public HtmlBuilder EndList()
        {
            _content.Append(EndUlTpl);
            return this;
        }

        /// <summary>
        /// Добавление пункта в список
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Контекст</returns>
        public HtmlBuilder List(string text)
        {
            _content.AppendFormat(LiTpl, text);
            return this;
        }

        /// <summary>
        /// Получение сгенерированного html
        /// </summary>
        /// <returns>Строка html</returns>
        public string Value()
        {
            return _content.ToString();
        }
    }
}
