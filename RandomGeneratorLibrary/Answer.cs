using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomGeneratorLibrary
{
    /// <summary>
    /// Ответ на вопрос
    /// </summary>
    public class Answer
    {
        #region vars
        private string text;
        /// <summary>
        /// Текст ответа
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private double chance;
        /// <summary>
        /// Шанс выпадения
        /// </summary>
        public double Chance
        {
            get { return chance; }
            set { chance = value; }
        }
        #endregion

        #region funcs
        /// <summary>
        /// Конструктор
        /// </summary>
        public Answer()
        {
        }

        public Answer(string text, double chance)
        {
            Text = text;
            Chance = chance;
        }
        #endregion
    }
}
