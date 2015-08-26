using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkMusicDiscoveryMVVM
{
    /// <summary>
    /// Глобальный класс утилит
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Сделать нижний регистр, но первая буква в верхнем.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToLowerButFirstUp(string name)
        {
            return char.ToUpperInvariant(name[0]) + name.Substring(1).ToLowerInvariant();
        }

        /// <summary>
        /// Проверка на символы кириллицы в строке.
        /// </summary>
        public static bool IsStringRussian(string content)
        {
            char[] letters = content.ToCharArray();

            for (int i = 0; i < letters.Length; i++)
            {
                int c = letters[i];
                if (((c >= 'а') && (c <= 'я')) ||
                    ((c >= 'А') && (c <= 'Я')))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
