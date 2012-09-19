using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace RandomGeneratorLibrary
{
    /// <summary>
    /// Генератор ответов на вопросы
    /// </summary>
    public class Generator
    {
        #region vars
        private Random random;
        private QuestionList questions;
        /// <summary>
        /// Вопросы 
        /// </summary>
        public QuestionList Questions
        {
            get { return questions; }
            set { questions = value; }
        }
        #endregion

        #region funcs
        /// <summary>
        /// Конструктор
        /// </summary>
        public Generator()
        {
            random = new Random(DateTime.Now.Millisecond);
            questions = new QuestionList();
        }

        /// <summary>
        /// Получение ответов на все вопросы
        /// </summary>
        /// <returns></returns>
        public List<Answer> GetAnswers()
        {
            List<Answer> answers = new List<Answer>();

            foreach (Question question in Questions)
            {
                Answer answer = GetAnswer(question);
                answers.Add(answer);
            }

            return answers;
        }

        /// <summary>
        /// Получение ответа на конкретный вопрос
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public Answer GetAnswer(Question question)
        {
            if (question.Answers.Count > 0)
            {
                // Строим числовой массив, в зависимости от попадания 
                // в промежуток между значениями и определяем выпавший ответ
                List<double> line = new List<double>();
                double current_value = 0;
                foreach (Answer answer in question.Answers)
                {
                    current_value = current_value + answer.Chance;
                    line.Add(current_value);
                }
                if (current_value > 0)
                {
                    // Генерируем (псевдо)случайное число
                    // и определяем выпавший ответ
                    double random_value = current_value * random.NextDouble();
                    for (int i = 0; i < line.Count; i++)
                    {
                        if (random_value <= line[i])
                        {
                            return question.Answers[i];
                        }
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public List<string> GetStringAnswers()
        {
            List<Answer> answers = GetAnswers();
            List<string> text = new List<string>();

            for (int i = 0; i < Questions.Count; i++)
            {
                text.Add((i + 1) + ") " + Questions[i].Text);
                if (answers[i] != null)
                {
                    text.Add(" - " + answers[i].Text);
                }
            }

            return text;
        }

        /// <summary>
        /// Получение полного номера версии
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }


        /// <summary>
        /// Получение короткого номера версии
        /// </summary>
        /// <returns></returns>
        public static string GetShortVersion()
        {
            string version = GetVersion();
            
            string short_version = "";
            string[] splitted = version.Split('.');
            short_version += splitted[0];
            short_version += "." + splitted[1];
            return short_version;
        }
        #endregion
    }
}
