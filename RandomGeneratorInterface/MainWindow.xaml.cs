using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RandomGeneratorLibrary;
using System.IO;
using Microsoft.Win32;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace RandomGeneratorInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Команды для горячих клавиш
        public static RoutedCommand com_add_question = new RoutedCommand();
        public static RoutedCommand com_add_answer = new RoutedCommand();
        public static RoutedCommand com_delete_question = new RoutedCommand();
        public static RoutedCommand com_process_chances = new RoutedCommand();
        public static RoutedCommand com_gen_input = new RoutedCommand();
        public static RoutedCommand com_gen_file = new RoutedCommand();
        public static RoutedCommand com_delete_answer = new RoutedCommand();

        private Generator generator;
        private Question current_question;
        private List<TextBox> question_answers_text;
        private List<TextBox> question_answers_chance;
        private Question Current_question
        {
            get
            {
                return current_question;
            }
            set
            {
                // Сохраняем предыдущие введенные данные в вопрос
                if (current_question != null)
                {
                    SaveQuestion();
                }

                ClearAnswers();

                if (value != null)
                {
                    UpdateQuestion(value);
                }
                else
                {
                    DisableInterface();
                }
            }
        }

        private void DisableInterface()
        {
            tbQuestion.IsEnabled = false;
            btAddAnswer.IsEnabled = false;
            btDeleteAnswer.IsEnabled = false;
            btProcessChaces.IsEnabled = false;
            btDelete.IsEnabled = false;
        }

        private void EnableInterface()
        {
            tbQuestion.IsEnabled = true;
            btAddAnswer.IsEnabled = true;
            btDeleteAnswer.IsEnabled = true;
            btProcessChaces.IsEnabled = true;
            btDelete.IsEnabled = true;
        }

        public MainWindow()
        {
            generator = new Generator();
            question_answers_text = new List<TextBox>();
            question_answers_chance = new List<TextBox>();

            string command = Environment.CommandLine;
            string[] parts = command.Split(' ');
            if (parts[parts.Length - 1].Contains("language="))
            {
                string language = parts[parts.Length - 1];
                language = language.Split('=')[1];
                Properties.Settings.Default.Language = language;
                Properties.Settings.Default.Save();
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(language);
            }
            else
            {
                string language = Properties.Settings.Default.Language;
                if (language != "")
                {
                    System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo(language);
                }
            }
            
            InitializeComponent();

            this.Title += " " + Generator.GetShortVersion();
            
            questionsListView.ItemsSource = null;
            questionsListView.ItemsSource = generator.Questions;

            InitHotKeys();
        }

        /// <summary>
        /// Инициализация горячих клавиш
        /// </summary>
        private void InitHotKeys()
        {
            KeyGesture gesture = null;
            KeyBinding bind = null;

            CommandBindings.Add(new CommandBinding(com_add_question, btAdd_Click));
            gesture = new KeyGesture(Key.Q, ModifierKeys.Control);
            bind = new KeyBinding(com_add_question, gesture);
            InputBindings.Add(bind);

            CommandBindings.Add(new CommandBinding(com_add_answer, btAddAnswer_Click));
            gesture = new KeyGesture(Key.F, ModifierKeys.Control);
            bind = new KeyBinding(com_add_answer, gesture);
            InputBindings.Add(bind);

            CommandBindings.Add(new CommandBinding(com_delete_question, btDelete_Click));
            gesture = new KeyGesture(Key.D, ModifierKeys.Control);
            bind = new KeyBinding(com_delete_question, gesture);
            InputBindings.Add(bind);

            CommandBindings.Add(new CommandBinding(com_process_chances, btProcessChaces_Click));
            gesture = new KeyGesture(Key.R, ModifierKeys.Control);
            bind = new KeyBinding(com_process_chances, gesture);
            InputBindings.Add(bind);

            CommandBindings.Add(new CommandBinding(com_gen_file, menuClickGenerateFile));
            gesture = new KeyGesture(Key.W, ModifierKeys.Control);
            bind = new KeyBinding(com_gen_file, gesture);
            InputBindings.Add(bind);

            CommandBindings.Add(new CommandBinding(com_gen_input, menuClickGenerateOutput));
            gesture = new KeyGesture(Key.E, ModifierKeys.Control);
            bind = new KeyBinding(com_gen_input, gesture);
            InputBindings.Add(bind);

            CommandBindings.Add(new CommandBinding(com_delete_answer, btDeleteAnswer_Click));
            gesture = new KeyGesture(Key.G, ModifierKeys.Control);
            bind = new KeyBinding(com_delete_answer, gesture);
            InputBindings.Add(bind);
        }

        /// <summary>
        /// Обновление текущего вопроса
        /// </summary>
        /// <param name="value"></param>
        public void UpdateQuestion(Question value)
        {
            current_question = null;
            current_question = value;

            // Активируем контролы
            EnableInterface();

            // Выводим текст и варианты другого вопроса
            //ClearAnswers();
            tbQuestion.Text = current_question.Text;
            if (current_question.Answers.Count > 0)
            {
                foreach (Answer answer in current_question.Answers)
                {
                    AddAnswer(answer);
                }
            }
        }

        /// <summary>
        /// Добавление нового вопроса 
        /// </summary>
        private void AddQuestion(Question question)
        {
            generator.Questions.Add(question);
            int index = generator.Questions.Count - 1;
        }

        /// <summary>
        /// Сохраняем текущий вопрос из введенных данных
        /// </summary>
        private void SaveQuestion()
        {
            current_question.Text = tbQuestion.Text;
            current_question.Answers.Clear();
            for (int i = 0; i < question_answers_text.Count; i++)
            {
                string text = question_answers_text[i].Text;
                double chance = 0;
                try
                {
                    chance = double.Parse(question_answers_chance[i].Text);
                }
                catch
                {
                }
                Answer answer = new Answer(text, chance);
                current_question.Answers.Add(answer);
            }

            questionsListView.ItemsSource = null;
            questionsListView.ItemsSource = generator.Questions;
        }

        /// <summary>
        /// Очистка вариантов ответа
        /// </summary>
        private void ClearAnswers()
        {
            tbQuestion.Text = "";
            question_answers_text.Clear();
            question_answers_chance.Clear();
            answers_grid.RowDefinitions.Clear();
            answers_grid.Children.Clear();
        }

        /// <summary>
        /// Добавление варианта в список
        /// </summary>
        private void AddAnswer(Answer answer)
        {
            // Создаем новую строку
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(30);
            answers_grid.RowDefinitions.Add(row);
            
            // Добавляем контролы
            TextBox text_box = new TextBox();
            text_box.Text = answer.Text;
            
            TextBox text_value = new TextBox();
            text_value.Text = answer.Chance.ToString();

            int row_number = answers_grid.RowDefinitions.Count - 1;

            answers_grid.Children.Add(text_box);
            Grid.SetColumn(text_box, 0);
            Grid.SetRow(text_box, row_number);

            answers_grid.Children.Add(text_value);
            Grid.SetColumn(text_value, 1);
            Grid.SetRow(text_value, row_number);

            // И добавляем эти контролы в списки для дальнейшего использования
            question_answers_text.Add(text_box);
            question_answers_chance.Add(text_value);
        }

        /// <summary>
        /// Генерация с выводом либо в файл
        /// </summary>
        /// <param name="to_file"></param>
        private void Generate(bool to_file)
        {
            if (generator.Questions.Count > 0)
            {
                if (current_question != null)
                {
                    SaveQuestion();
                }
                List<string> lines = generator.GetStringAnswers();

                if (to_file == true)
                {
                    // Случай вывода в файл
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.DefaultExt = "txt";
                    if (dialog.ShowDialog() == true)
                    {
                        string filename = dialog.FileName;
                        File.WriteAllLines(filename, lines);
                        MessageBox.Show(Properties.Resources.msgFileSave + "\n" + filename);
                    }
                }
                else
                {
                    string text = Properties.Resources.msgQuestions + "\n";
                    foreach (string line in lines)
                    {
                        text += line + "\n";
                    }
                    // Случай простого вывода
                    MessageBox.Show(text);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.msgQuestionsNotFound);
            }
        }

        private void btAddAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (current_question != null)
            {
                current_question.Answers.Add(new Answer());
                int index = current_question.Answers.Count - 1;
                AddAnswer(current_question.Answers[index]);
            }
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            AddQuestion(new Question("..."));
            btDelete.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource questionViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("questionViewSource")));
            System.Windows.Data.CollectionViewSource generatorViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("generatorViewSource")));
        }

        private void questionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Current_question = (Question)questionsListView.SelectedItem;
        }

        private void btProcessChaces_Click(object sender, RoutedEventArgs e)
        {
            SaveQuestion();
            ClearAnswers();
            current_question.ProcessChances();
            UpdateQuestion(current_question);
        }

        private void menuClickLoad(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = "xml";
            dialog.Multiselect = false;
            dialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Generator));
                FileStream stream = File.Open(dialog.FileName, FileMode.Open);
                generator = (Generator)serializer.Deserialize(stream);
                
                Current_question = null;
                questionsListView.ItemsSource = null;
                questionsListView.ItemsSource = generator.Questions;
            }
        }

        private void menuClickSave(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "xml";
            if (dialog.ShowDialog() == true)
            {
                string filename = dialog.FileName;
                
                FileStream stream = File.Create(filename);

                if(stream != null)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Generator));
                    serializer.Serialize(stream, generator);
                    MessageBox.Show(Properties.Resources.msgFileSave + "\n" + filename);
                }
            }
        }

        private void menuClickExit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuClickGenerateOutput(object sender, RoutedEventArgs e)
        {
            Generate(false);
        }

        private void menuClickGenerateFile(object sender, RoutedEventArgs e)
        {
            Generate(true);
        }

        private void menuClickAbout(object sender, RoutedEventArgs e)
        {
            string caption = Properties.Resources.msgAboutCaption;

            string text = Properties.Resources.msgAbout;

            text += Generator.GetVersion();

            MessageBox.Show(text, caption);
        }

        private void menuClickHelp(object sender, RoutedEventArgs e)
        {
            string caption = Properties.Resources.msgHelpCaption;

            string text = Properties.Resources.msgHelp;
            
            MessageBox.Show(text, caption);
        }

        private void menuClickNew(object sender, RoutedEventArgs e)
        {
            generator = new Generator();
            current_question = null;
            questionsListView.ItemsSource = null;
            questionsListView.ItemsSource = generator.Questions;
            ClearAnswers();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Создаем диалог с вопросом о сохранении
            string text = Properties.Resources.msgSaveFile;
            string caption = Properties.Resources.msgSaveFileCaption;
            MessageBoxButton button = MessageBoxButton.YesNo;
            
            // Выводим и проверяем результат
            MessageBoxResult result = MessageBox.Show(text, caption, button);
            if (result == MessageBoxResult.Yes)
            {
                menuClickSave(null, null);
            }
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            generator.Questions.Remove(current_question);
            Current_question = null;
            questionsListView.ItemsSource = null;
            questionsListView.ItemsSource = generator.Questions;
            if (generator.Questions.Count == 0)
            {
                btDelete.IsEnabled = false;
            }
        }

        private void btDeleteAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (question_answers_text.Count > 0)
            {
                question_answers_text.RemoveAt(question_answers_text.Count - 1);
                question_answers_chance.RemoveAt(question_answers_chance.Count - 1);

                SaveQuestion();
                ClearAnswers();
                UpdateQuestion(current_question);
            }
        }

        private void menuClickEnglish(object sender, RoutedEventArgs e)
        {
            Close();
            Properties.Settings.Default.Language = "en-US";
            Properties.Settings.Default.Save();
            Process.Start("RandomGeneratorInterface.exe");
            //Process.Start("RandomGeneratorInterface.exe", "language=en-US");
        }

        private void menuClickRussian(object sender, RoutedEventArgs e)
        {
            Close();
            Properties.Settings.Default.Language = "ru-RU";
            Properties.Settings.Default.Save();
            Process.Start("RandomGeneratorInterface.exe");
            //Process.Start("RandomGeneratorInterface.exe", "language=ru-RU");
        }
    }
}
