using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomGeneratorLibrary
{
    /// <summary>
    /// Вопрос
    /// </summary>
    public class Question
    {
        #region vars
        private string text;
        /// <summary>
        /// Текст вопроса
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        private List<Answer> answers;
        /// <summary>
        /// Варианты ответа
        /// </summary>
        public List<Answer> Answers
        {
            get { return answers; }
            set { answers = value; }
        }
        #endregion

        #region funcs
        /// <summary>
        /// Конструктор
        /// </summary>
        public Question()
        {
            answers = new List<Answer>();
        }

        public Question(string text)
            : this()
        {
            Text = text;
        }

        /// <summary>
        /// Распределение шансов равномерно
        /// </summary>
        public void ProcessChances()
        {
            double chance = 100 / Answers.Count;
            foreach (Answer answer in Answers)
            {
                answer.Chance = chance;
            }
        }
        #endregion
    }
}
