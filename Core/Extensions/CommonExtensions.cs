using System;
using System.Linq;
using System.Text;

namespace Core.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Аналог contains
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool In<T>(this T source, params T[] list)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return list.Contains(source);
        }

        /// <summary>
        /// Проверка вхождения образца в заданный промежуток
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static bool Between<T>(this T actual, T lower, T upper) where T : IComparable<T>
        {
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
        }

        /// <summary>
        /// Raises the event.
        /// Does nothing if eventHandler is null.
        /// Because the event handler is passed as parameter, it is only fetched from the event field one time.
        /// This makes
        /// <code>MyEvent.RaiseEvent(x,y);</code>
        /// thread-safe
        /// whereas
        /// <code>if (MyEvent != null) MyEvent(x,y);</code>
        /// would not be safe.
        /// </summary>
        /// <remarks>Using this method is only thread-safe under the Microsoft .NET memory model,
        /// not under the less strict memory model in the CLI specification.</remarks>
        public static void RaiseEvent(this EventHandler eventHandler, object sender, EventArgs e)
        {
            if (eventHandler != null)
            {
                eventHandler(sender, e);
            }
        }

        /// <summary>
        /// Raises the event.
        /// Does nothing if eventHandler is null.
        /// Because the event handler is passed as parameter, it is only fetched from the event field one time.
        /// This makes
        /// <code>MyEvent.RaiseEvent(x,y);</code>
        /// thread-safe
        /// whereas
        /// <code>if (MyEvent != null) MyEvent(x,y);</code>
        /// would not be safe.
        /// </summary>
        public static void RaiseEvent<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
        {
            if (eventHandler != null)
            {
                eventHandler(sender, e);
            }
        }

        /// <summary>
        /// Получить последовательность байт с преамбулой кодировки из текста 
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] GetBytesWithPreamble(this Encoding encoding, string text)
        {
            byte[] encodedText = encoding.GetBytes(text);
            byte[] bom = encoding.GetPreamble();
            if (bom.Length > 0)
            {
                var result = new byte[bom.Length + encodedText.Length];
                bom.CopyTo(result, 0);
                encodedText.CopyTo(result, bom.Length);
                return result;
            }
            return encodedText;
        }
    }
}
