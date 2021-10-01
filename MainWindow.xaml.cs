using System.Collections.Generic;
using System.Windows;

namespace ActiveTable
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ActiveTableObject ActiveTableObject { get; set; }
        private int Counter { get; set; } = 4;

        public MainWindow()
        {
            InitializeComponent();

            //Добавляем столбцы в таблицу
            ActiveTableObject = new ActiveTableObject("MainTable1", ActiveTableWPF);
            ActiveTableObject.AddColumn("Номер", ColumnTypes.Integer, Function);
            ActiveTableObject.AddColumn("Строка", ColumnTypes.String, Function);
            List<string> comboBoxItems = new List<string>() { "Поверхность1", "Поверхность2", "Поверхность3" };
            ActiveTableObject.AddColumn("Поверхность", ColumnTypes.ComboBox, Function, comboBoxItems);            
            ActiveTableObject.AddColumn("Угол", ColumnTypes.Angle, Function);
            ActiveTableObject.AddColumn("Да/Нет", ColumnTypes.Boolean, Function);            
            ActiveTableObject.AddColumn("Цвет", ColumnTypes.Color, Function);

            //Добавляем строки в таблицу
            ActiveTableObject.AddLine("Строка0", new string[] { "1", "Строка1", "Поверхность1", "90.00", "true", "red" });
            ActiveTableObject.AddLine("Строка1", new string[] { "2", "Строка2", "Поверхность3", "90.00", "true", "green" });
            ActiveTableObject.AddLine("Строка2", new string[] { "3", "Строка3", "Поверхность2", "90.00", "true", "blue" });
            ActiveTableObject.AddLine("Строка3", new string[] { "4", "Строка4", "Поверхность1", "90.00", "true", "white" });
        }

        void Function(string lineName, int columnNumber, int lineNumber, string newValue)
        {
            //MessageBox.Show($"Имя строки: {lineName}, Столбец: {columnNumber}, Строка: {lineNumber}, Новое значение: {newValue}");
        }

        private void AddBuddon_Click(object sender, RoutedEventArgs e)
        {
            ActiveTableObject.AddLine($"Строка{Counter++}", new string[] { "1", "Строка1", "Поверхность1", "90.00", "true", "green" });
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            ActiveTableObject.RemoveSelectedLine();
        }
    }
}
