using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ActiveTable
{
    /// <summary>
    /// Объект активной таблицы
    /// </summary>
    class ActiveTableObject
    {
        //Настройки для талицы

        /// <summary>
        /// Задать цвет ячейки
        /// </summary>
        public static Brush NormalCellBrush { get; set; } = Brushes.White;

        /// <summary>
        /// Задать цвет выбранной ячейки
        /// </summary>
        public static Brush SelectedCellBrush { get; set; } = Brushes.LightBlue;

        /// <summary>
        /// Задать высотку заголовка таблицы
        /// </summary>
        public static double TitleHeigth { get; set; } = 25.0;

        /// <summary>
        /// Задать отступ от заголовка таблицы
        /// </summary>
        public static double TitleDownMargin { get; set; } = 3.0;

        /// <summary>
        /// Задать высоту ячейки таблицы
        /// </summary>
        public static double CellHeigth { get; set; } = 20.0;

        /// <summary>
        /// Задать количество знаков после запятой
        /// </summary>
        public static int RoundDouble { get; set; } = 3;

        /// <summary>
        /// Ширина сплиттера для талицы (Должно быть целое нечётное число)
        /// </summary>
        public static double SplitterWidth { get; set; } = 5.0;

        /// <summary>
        /// Ширина корешка таблицы
        /// </summary>
        public static double RootWidth { get; set; } = 20.0;

        /// <summary>
        /// Настройка ширины ячейки таблицы
        /// </summary>
        public static GridLength TableCellLength { get; set; } = GridLength.Auto; //new GridLength(1, GridUnitType.Star);

        public string TableName { get; set; }
        Grid TableGrid { get; set; }

        private Dictionary<int, Column> Columns { get; set; }
        private int ColumnsNumerator { get; set; } = 0;
        private int LineNumerator { get; set; } = 0;

        public static string SystemSeparator { get; set; } = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        /// <summary>
        /// Выбранная строка
        /// </summary>
        private List<int> SelectedLines { get; set; } = new List<int>();

        public ActiveTableObject(string tableName, Grid mainGrid)
        {
            TableName = tableName;
            Columns = new Dictionary<int, Column>();

            //Очищаем грид перед началом работы с ним
            for (int i = 0; i < mainGrid.Children.Count; i++)
            {
                mainGrid.Children.RemoveAt(i);
            }

            ScrollViewer scrollViewer = new ScrollViewer();
            mainGrid.Children.Add(scrollViewer);
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
           
            Grid tableGrid = new Grid();

            scrollViewer.Content = tableGrid;

            TableGrid = tableGrid;

            //Создаём и настраиваем разделитель столбцов
            ColumnDefinition column0 = new ColumnDefinition();
            TableGrid.ColumnDefinitions.Add(column0);
            column0.Width = GridLength.Auto;

            GridSplitter gridSplitter0 = new GridSplitter();
            TableGrid.Children.Add(gridSplitter0);
            gridSplitter0.SetValue(Grid.ColumnProperty, TableGrid.ColumnDefinitions.Count - 1);
            gridSplitter0.ShowsPreview = true;
            gridSplitter0.Width = SplitterWidth;
            gridSplitter0.Opacity = 0;
            gridSplitter0.HorizontalAlignment = HorizontalAlignment.Center;
            gridSplitter0.VerticalAlignment = VerticalAlignment.Stretch;
            gridSplitter0.GotFocus += (object sender, RoutedEventArgs e) => { (sender as GridSplitter).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); };
            gridSplitter0.SetValue(Panel.ZIndexProperty, 1);

            //Создаём и настраиваем корешок таблицы
            ColumnDefinition column1 = new ColumnDefinition();
            TableGrid.ColumnDefinitions.Add(column1);
            column1.Width = new GridLength(RootWidth);
            column1.MinWidth = RootWidth;

            Button rootCell1 = new Button();
            TableGrid.Children.Add(rootCell1);
            rootCell1.SetValue(Grid.ColumnProperty, TableGrid.ColumnDefinitions.Count - 1);
            rootCell1.Name = $"TitleOrigin";
            rootCell1.Content = "";
            rootCell1.Height = TitleHeigth;
            rootCell1.VerticalAlignment = VerticalAlignment.Top;
            rootCell1.BorderThickness = new Thickness(0, 0, 1, 0);
            rootCell1.BorderBrush = Brushes.Gray;
            rootCell1.Background = Brushes.White;
            rootCell1.SetValue(Panel.ZIndexProperty, 0);
            rootCell1.HorizontalAlignment = HorizontalAlignment.Stretch;
            rootCell1.Margin = new Thickness(0, 0, -((SplitterWidth/2)+0.5), 0);
            rootCell1.GotFocus += (object sender, RoutedEventArgs e) => { (sender as Button).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); };

            //Создаём и настраиваем разделитель столбцов
            ColumnDefinition column2 = new ColumnDefinition();
            TableGrid.ColumnDefinitions.Add(column2);
            column2.Width = GridLength.Auto;
            
            GridSplitter gridSplitter1 = new GridSplitter();
            TableGrid.Children.Add(gridSplitter1);
            gridSplitter1.SetValue(Grid.ColumnProperty, TableGrid.ColumnDefinitions.Count - 1);
            gridSplitter1.ShowsPreview = true;
            gridSplitter1.Width = SplitterWidth;
            gridSplitter1.Opacity = 0;
            gridSplitter1.HorizontalAlignment = HorizontalAlignment.Center;
            gridSplitter1.VerticalAlignment = VerticalAlignment.Stretch;
            gridSplitter1.GotFocus += (object sender, RoutedEventArgs e) => { (sender as GridSplitter).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); };
            gridSplitter1.MouseDoubleClick += (object sender, MouseButtonEventArgs e) => { column1.Width = new GridLength(RootWidth); };
            gridSplitter1.SetValue(Panel.ZIndexProperty, 1);

            //Создаём и настраиваем задний корешок таблицы
            ColumnDefinition column3 = new ColumnDefinition();
            TableGrid.ColumnDefinitions.Add(column3);
            column3.Width = new GridLength(RootWidth);
            column3.MinWidth = RootWidth;

            Button rootCell2 = new Button();
            TableGrid.Children.Add(rootCell2);
            rootCell2.SetValue(Grid.ColumnProperty, TableGrid.ColumnDefinitions.Count - 1);
            rootCell2.Name = $"TitleEnd";
            rootCell2.Content = "";
            rootCell2.Height = TitleHeigth;
            rootCell2.VerticalAlignment = VerticalAlignment.Top;
            rootCell2.BorderThickness = new Thickness(1, 0, 0, 0);
            rootCell2.BorderBrush = Brushes.Gray;
            rootCell2.Background = Brushes.White;
            rootCell2.SetValue(Panel.ZIndexProperty, 0);
            rootCell2.HorizontalAlignment = HorizontalAlignment.Stretch;
            rootCell2.Margin = new Thickness(-((SplitterWidth / 2) + 0.5), 0, -((SplitterWidth / 2) + 0.5), 0);
            rootCell2.GotFocus += (object sender, RoutedEventArgs e) => { (sender as Button).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); };
        }

        /// <summary>
        /// Удалить выбранную строку
        /// </summary>
        public void RemoveSelectedLine()
        {
            if (SelectedLines.Count == 0) return;

            for (int i = 0; i < SelectedLines.Count; i++)
            {
                RemoveLine(SelectedLines[0]);
            }

            SelectedLines.Clear();
        }

        /// <summary>
        /// Удалить строку по индексу
        /// </summary>
        public void RemoveLine(int lineNumber)
        {          
            foreach (Column column in Columns.Values)
            {
                column.CurStackPanel.Children.RemoveAt(lineNumber);
            }                         
        }

        /// <summary>
        /// Добавить колонку в таблицу
        /// </summary>
        public void AddColumn(string columnTitle, ColumnTypes columnType, Action<string, int, int, string> cellChangeReaction, List<string> checkBoxItems = null)
        {
            Column column = new Column(columnTitle, columnType, Columns.Count, TableGrid, SelectedLines);
            if (columnType == ColumnTypes.ComboBox)
            {
                if (checkBoxItems != null)
                {
                    column.ComboBoxItems = checkBoxItems;
                }
                else
                {
                    MessageBox.Show("Ошибка! В колонку с выпадающем списком не установлены значения выпадающего списка!");
                }
            } 
            Columns.Add(ColumnsNumerator++, column);

            column.CellClick += CellSelectReaction;
            column.CellChange += cellChangeReaction;
        }

        /// <summary>
        /// Добавить ячейку в таблицу
        /// </summary>
        public void AddCell(string lineName, int columnNumber, int lineNumber, string defaultValue)
        {
            Columns[columnNumber].AddLine(lineName, lineNumber, defaultValue);
        }

        /// <summary>
        /// Добавить строку в таблицу
        /// </summary>
        public void AddLine(string lineName, string[] lineValues)
        {
            if (lineValues.Length != ColumnsNumerator) return;

            for (int i = 0; i < ColumnsNumerator; i++)
            {
                AddCell(lineName, i, LineNumerator, lineValues[i]);
            }

            LineNumerator++;
        }

        /// <summary>
        /// Реакция на выбор ячейки
        /// </summary>
        private void CellSelectReaction(int columnNumber, int lineNumber, ContentControl control)
        {                      
            int trueLineNumber = -1;

            foreach (Column column in Columns.Values)
            {
                if (column.CurStackPanel.Children.Contains(control))
                {
                    trueLineNumber = column.CurStackPanel.Children.IndexOf(control);
                    break;
                }
            }

            if (trueLineNumber == -1) return;

            if (SelectedLines.Count > 0 && SelectedLines[0] == trueLineNumber)
            {
                Columns[columnNumber].SelectedCellClick(columnNumber, trueLineNumber, control);
            }
            else
            {
                if (SelectedLines.Count > 0 && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                {
                    foreach (int selectedLine in SelectedLines)
                    {
                        foreach (Column column in Columns.Values)
                        {
                            (column.CurStackPanel.Children[selectedLine] as ContentControl).Background = NormalCellBrush;
                        }                                                        
                    }

                    int maxLine = SelectedLines[0] > trueLineNumber ? SelectedLines[0] : trueLineNumber;
                    for (int i = 1; i < SelectedLines.Count; i++)
                    {
                        maxLine = SelectedLines[i] > maxLine ? SelectedLines[i] : maxLine;
                    }

                    int minLine = SelectedLines[SelectedLines.Count - 1] < trueLineNumber ? SelectedLines[SelectedLines.Count - 1] : trueLineNumber;
                    for (int i = 0; i < SelectedLines.Count - 1; i++)
                    {
                        minLine = SelectedLines[i] < minLine ? SelectedLines[i] : minLine;
                    }

                    SelectedLines.Clear();
                    for (int i = minLine; i <= maxLine; i++)
                    {
                        SelectedLines.Add(i);
                        foreach (Column column in Columns.Values)
                        {                           
                            (column.CurStackPanel.Children[i] as ContentControl).Background = SelectedCellBrush;
                        }                               
                    }                                          
                }
                else
                {
                    foreach (Column column in Columns.Values)
                    {
                        foreach (int selectedLine in SelectedLines)
                        {
                            (column.CurStackPanel.Children[selectedLine] as ContentControl).Background = NormalCellBrush;                            
                        }

                        (column.CurStackPanel.Children[trueLineNumber] as ContentControl).Background = SelectedCellBrush;                       
                    }
                    SelectedLines.Clear();
                    SelectedLines.Add(trueLineNumber);
                }
            }
        }
    }

    /// <summary>
    /// Колонка таблицы
    /// </summary>
    class Column
    {
        public int LineCount { get => LinesSelectedIndexAndContent.Count; }
        private static int TitleIndex { get; set; } = 0;
        public string СolumnTitle { get; set; }
        public ColumnTypes СolumnType { get; set; }
        public StackPanel CurStackPanel { get; set; }
        private int ColumnNumber { get; set; }
        public List<int> SelectedLines { get; private set; }
        public Dictionary<int, Dictionary<int, string>> LinesSelectedIndexAndContent { get; set; } = new Dictionary<int, Dictionary<int, string>>();

        public event Action<string, int, int, string> CellChange;

        public event Action<int, int, ContentControl> CellClick;

        /// <summary>
        /// Значения для чекбокса, если он есть
        /// </summary>
        public List<string> ComboBoxItems { get; set; }

        /// <summary>
        /// Совершён ли массированный выбор строк
        /// </summary>
        public bool IsMassiveCheck { get; private set; }

        public Column(string columnTitle, ColumnTypes columnType, int columnNumber, Grid tableGrid, List<int> selectedLines)
        {
            СolumnTitle = columnTitle;
            СolumnType = columnType;
            ColumnNumber = columnNumber;
            SelectedLines = selectedLines;

            //Временно удаляем задний корешок
            tableGrid.ColumnDefinitions.RemoveAt(tableGrid.ColumnDefinitions.Count - 1);

            //Создаём и настраиваем столбец и заголовок
            ColumnDefinition column0 = new ColumnDefinition();
            tableGrid.ColumnDefinitions.Add(column0);
            column0.Width = ActiveTableObject.TableCellLength;         

            Button titleCell = new Button();
            titleCell.Name = $"Title{TitleIndex++}";
            titleCell.Content = columnTitle;
            titleCell.Height = ActiveTableObject.TitleHeigth;
            titleCell.VerticalAlignment = VerticalAlignment.Top;
            titleCell.BorderThickness = new Thickness(1, 0, 1, 0);
            titleCell.BorderBrush = Brushes.Gray;
            titleCell.Background = Brushes.White;
            titleCell.SetValue(Panel.ZIndexProperty, 0);
            titleCell.HorizontalAlignment = HorizontalAlignment.Stretch;           
            titleCell.Margin = new Thickness(-((ActiveTableObject.SplitterWidth / 2) + 0.5), 0, -((ActiveTableObject.SplitterWidth / 2) + 0.5), 0);
            titleCell.GotFocus += (object sender, RoutedEventArgs e) => { (sender as Button).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); };

            tableGrid.Children.Add(titleCell);
            titleCell.SetValue(Grid.ColumnProperty, tableGrid.ColumnDefinitions.Count - 1);

            //Создаём и настраиваем стек панель для содержимого таблицы
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Margin = new Thickness(0, ActiveTableObject.TitleHeigth + ActiveTableObject.TitleDownMargin, 0, 0);
            stackPanel.SetValue(Panel.ZIndexProperty, 2);
            stackPanel.SetValue(Grid.ColumnProperty, tableGrid.ColumnDefinitions.Count - 1);
            tableGrid.Children.Add(stackPanel);
            CurStackPanel = stackPanel;

            //Создаём и настраиваем разделитель столбцов
            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = GridLength.Auto;
            tableGrid.ColumnDefinitions.Add(column1);

            GridSplitter gridSplitter = new GridSplitter();
            gridSplitter.ShowsPreview = true;
            gridSplitter.Width = ActiveTableObject.SplitterWidth;
            gridSplitter.Opacity = 0;
            gridSplitter.HorizontalAlignment = HorizontalAlignment.Center;
            gridSplitter.VerticalAlignment = VerticalAlignment.Stretch;
            gridSplitter.GotFocus += (object sender, RoutedEventArgs e) => { (sender as GridSplitter).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); };
            gridSplitter.SetValue(Panel.ZIndexProperty, 1);
            gridSplitter.MouseDoubleClick += (object sender, MouseButtonEventArgs e) => { column0.Width = ActiveTableObject.TableCellLength; };
            tableGrid.Children.Add(gridSplitter);
            gridSplitter.SetValue(Grid.ColumnProperty, tableGrid.ColumnDefinitions.Count - 1);

            //Создаём и настраиваем задний корешок таблицы
            ColumnDefinition column3 = new ColumnDefinition();
            tableGrid.ColumnDefinitions.Add(column3);
            column3.Width = new GridLength(ActiveTableObject.RootWidth);
            column3.MinWidth = ActiveTableObject.RootWidth;

            Button rootCell2 = new Button();
            tableGrid.Children.Add(rootCell2);
            rootCell2.SetValue(Grid.ColumnProperty, tableGrid.ColumnDefinitions.Count - 1);
            rootCell2.Name = $"TitleEnd";
            rootCell2.Content = "";
            rootCell2.Height = ActiveTableObject.TitleHeigth;
            rootCell2.VerticalAlignment = VerticalAlignment.Top;
            rootCell2.BorderThickness = new Thickness(1, 0, 0, 0);
            rootCell2.BorderBrush = Brushes.Gray;
            rootCell2.Background = Brushes.White;
            rootCell2.SetValue(Panel.ZIndexProperty, 0);
            rootCell2.HorizontalAlignment = HorizontalAlignment.Stretch;
            rootCell2.Margin = new Thickness(-((ActiveTableObject.SplitterWidth / 2) + 0.5), 0, -((ActiveTableObject.SplitterWidth / 2) + 0.5), 0);
            rootCell2.GotFocus += (object sender, RoutedEventArgs e) => { (sender as Button).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); };
        }

        /// <summary>
        /// Добавить строку в колонке
        /// </summary>
        public void AddLine(string lineName, int lineNumber, string value)
        {
            Dictionary<int, string> newLine = new Dictionary<int, string>();

            //Создание и настройка ячейки таблицы
            Button tableCell = new Button();
            tableCell.Name = lineName;
            SetContentFromType(ColumnNumber, tableCell, value, СolumnType);
            tableCell.ClickMode = ClickMode.Press;
            tableCell.Click += (object sender, RoutedEventArgs e) => { CellClick(ColumnNumber, lineNumber, tableCell); };
            tableCell.Height = ActiveTableObject.CellHeigth;
            tableCell.VerticalAlignment = VerticalAlignment.Top;
            tableCell.BorderThickness = new Thickness(1, 1, 1, 1);
            tableCell.BorderBrush = Brushes.LightGray;
            tableCell.Background = Brushes.White;
            tableCell.HorizontalAlignment = HorizontalAlignment.Stretch;

            switch (СolumnType)
            {
                case ColumnTypes.Boolean:
                    newLine.Add(0, value);
                    tableCell.HorizontalContentAlignment = HorizontalAlignment.Center;
                    break;
                case ColumnTypes.Double:
                case ColumnTypes.Angle:
                case ColumnTypes.String:
                case ColumnTypes.Integer:               
                case ColumnTypes.Color:
                    newLine.Add(0, value);
                    tableCell.HorizontalContentAlignment = HorizontalAlignment.Left;
                    break;
                case ColumnTypes.ComboBox:
                    newLine.Add(ComboBoxItems.IndexOf(value), value);
                    tableCell.HorizontalContentAlignment = HorizontalAlignment.Left;
                    break;
                default:
                    newLine.Add(0, value);
                    break;
            }

            LinesSelectedIndexAndContent.Add(lineNumber, newLine);
                      
            tableCell.Margin = new Thickness(-((ActiveTableObject.SplitterWidth / 2) + 0.5), 0, -((ActiveTableObject.SplitterWidth / 2) + 0.5), 0);
            CurStackPanel.Children.Add(tableCell);            
        }

        /// <summary>
        /// Установить значение в контент в соответствии с типом колонки
        /// </summary>
        private void SetContentFromType(int columnNumber, ContentControl control, string value, ColumnTypes сolumnType)
        {
            string name = control.Name;
            switch (сolumnType)
            {
                case ColumnTypes.String:
                    control.Content = value;
                    break;
                case ColumnTypes.Integer:
                    double doubleParse = Math.Round(DoubleParse(value), MidpointRounding.ToEven);
                    control.Content = (int) doubleParse;
                    break;
                case ColumnTypes.Boolean:
                    CheckBox checkBox = new CheckBox();
                    control.Content = checkBox;
                    checkBox.HorizontalAlignment = HorizontalAlignment.Center;
                    checkBox.VerticalAlignment = VerticalAlignment.Center;
                    checkBox.IsChecked = bool.Parse(value);
                    checkBox.Click += (object sender, RoutedEventArgs e) => { TextChangedControl(columnNumber, sender); LostFocusControl(sender); };
                    checkBox.MouseMove += (object sender, MouseEventArgs e) => { IsMassiveCheck = SelectedLines.Contains(CurStackPanel.Children.IndexOf(control)) ? true : false;  };                   
                    break;
                case ColumnTypes.Double:
                    double parsedDouble = DoubleParse(value);
                    control.Content = Math.Round(parsedDouble, ActiveTableObject.RoundDouble);
                    break;
                case ColumnTypes.Angle:
                    string trimValue = value.Split( new char[] { ' ' })[0];
                    double parsedAngle = DoubleParse(trimValue);                   
                    control.Content = $"{Math.Round(parsedAngle, ActiveTableObject.RoundDouble)} град.";
                    break;
                case ColumnTypes.Color:
                    StackPanel stack = new StackPanel();
                    control.Content = stack;
                    stack.Orientation = Orientation.Horizontal;
                    Canvas rect = new Canvas();
                    stack.Children.Add(rect);
                    rect.Height = 15;
                    rect.Width = 15;

                    Rectangle rectangle = new Rectangle();
                    rect.Children.Add(rectangle);
                    rectangle.Width = 15;
                    rectangle.Height = 15;
                    rectangle.Stroke = Brushes.DarkGray;
                    rectangle.Fill = new BrushConverter().ConvertFromString(value) as Brush;
                                     
                    TextBlock textBlock = new TextBlock();
                    stack.Children.Add(textBlock);
                    textBlock.Text = value;                                     
                    break;
                case ColumnTypes.ComboBox:
                    control.Content = value;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Преобразовать строку в дробное число
        /// </summary>
        private double DoubleParse(string trimValue)
        {
            double parsedValue = 0;
            if (!double.TryParse(trimValue, out parsedValue))
            {               
                trimValue = trimValue.Replace(".", ActiveTableObject.SystemSeparator);
                trimValue = trimValue.Replace(",", ActiveTableObject.SystemSeparator);
                if (trimValue == "")
                {
                    parsedValue = 0.0;
                }
                else
                {
                    parsedValue = double.Parse(trimValue);
                }              
            }

            return parsedValue;
        }

        /// <summary>
        /// Клик по ячейке выбранной строки. Задание контекста для редактирования данных
        /// </summary>
        public void SelectedCellClick(int columnNumber, int lineNumber, ContentControl control)
        {
            switch (СolumnType)
            {              
                case ColumnTypes.Integer:
                case ColumnTypes.Double:
                case ColumnTypes.String:
                    {
                        TextBox textBox = new TextBox();
                        textBox.Text = control.Content.ToString();
                        control.Content = textBox;
                        textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                        textBox.VerticalAlignment = VerticalAlignment.Center;
                        textBox.Height = ActiveTableObject.CellHeigth;
                        textBox.Width = control.ActualWidth - 6;
                        textBox.BorderThickness = new Thickness(0, 0, 0, 0);
                        textBox.Background = control.Background;                       
                        textBox.TextAlignment = TextAlignment.Left;
                        textBox.LostFocus += (object sender, RoutedEventArgs args) => { TextChangedControl(columnNumber, sender); LostFocusControl(sender); }; //Здесь нужно будет заменить контент контрола на значение и удалить текстбокс
                        textBox.KeyDown += (object sender, KeyEventArgs e) => { if (e.Key == Key.Enter) { (sender as TextBox).Focusable = false; } };
                        if (СolumnType != ColumnTypes.String)
                        {
                            textBox.PreviewTextInput += DoubleTextBox_PreviewTextInput;
                        }                                               
                        textBox.SelectionStart = textBox.Text.Length;

                        Application.Current.Dispatcher.BeginInvoke(new Action(() => { textBox.Focus(); }), System.Windows.Threading.DispatcherPriority.Render);
                    }
                    break;
                case ColumnTypes.Angle:
                    {
                        TextBox textBox = new TextBox();
                        string value = control.Content.ToString();
                        textBox.Text = value.Substring(0, value.Length - 6);
                        control.Content = textBox;
                        textBox.HorizontalAlignment = HorizontalAlignment.Left;
                        textBox.VerticalAlignment = VerticalAlignment.Center;
                        textBox.Height = ActiveTableObject.CellHeigth;
                        textBox.Width = control.ActualWidth - 6;
                        textBox.BorderThickness = new Thickness(0, 0, 0, 0);
                        textBox.Background = control.Background;                      
                        textBox.TextAlignment = TextAlignment.Left;
                        textBox.LostFocus += (object sender, RoutedEventArgs args) => { TextChangedControl(columnNumber, sender); LostFocusControl(sender); }; //Здесь нужно будет заменить контент контрола на значение и удалить текстбокс
                        textBox.KeyDown += (object sender, KeyEventArgs e) => { if (e.Key == Key.Enter) { (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Down)); } };
                        textBox.PreviewTextInput += DoubleTextBox_PreviewTextInput;                       
                        textBox.SelectionStart = textBox.Text.Length;

                        Application.Current.Dispatcher.BeginInvoke(new Action(() => { textBox.Focus(); }), System.Windows.Threading.DispatcherPriority.Render);
                    }
                    break;

                case ColumnTypes.Color:
                    
                    List<string> colorBoxItems = new List<string>() { "black", "gray", "silver", "white", "red", "purple", "yellow", "lime", "blue", "aqua" };
                    List<StackPanel> colorStackPanels = new List<StackPanel>();                   

                    foreach (string colorStr in colorBoxItems)
                    {
                        StackPanel stack = new StackPanel();
                        colorStackPanels.Add(stack);
                        stack.Orientation = Orientation.Horizontal;
                        Canvas rect = new Canvas();
                        stack.Children.Add(rect);
                        rect.Height = 15;
                        rect.Width = 15;

                        Rectangle rectangle = new Rectangle();
                        rect.Children.Add(rectangle);
                        rectangle.Width = 15;
                        rectangle.Height = 15;
                        rectangle.Stroke = Brushes.DarkGray;
                        rectangle.Fill = new BrushConverter().ConvertFromString(colorStr) as Brush;
                                             
                        TextBlock textBlock = new TextBlock();
                        stack.Children.Add(textBlock);
                        textBlock.Text = colorStr;                       
                    }

                    colorStackPanels.Add(control.Content as StackPanel);

                    {
                        StackPanel stack = new StackPanel();
                        colorStackPanels.Add(stack);
                        stack.Orientation = Orientation.Horizontal;
                        Canvas rect = new Canvas();
                        stack.Children.Add(rect);
                        rect.Height = 15;
                        rect.Width = 15;

                        Ellipse ellipse = new Ellipse();
                        rect.Children.Add(ellipse);
                        ellipse.Width = 15;
                        ellipse.Height = 15;
                        ellipse.Fill = Brushes.White;
                        ellipse.Stroke = Brushes.DarkGray;
                     
                        TextBlock textBlock = new TextBlock();
                        stack.Children.Add(textBlock);
                        textBlock.Text = "Выбрать цвет...";                
                    }

                    ComboBox colorBox = new ComboBox();
                    colorBox.ItemsSource = colorStackPanels;
                    colorBox.SelectedItem = control.Content;
                    control.Content = colorBox;
                    colorBox.Width = control.ActualWidth - 6;
                    colorBox.BorderThickness = new Thickness(0, 0, 0, 0);
                    colorBox.Height = ActiveTableObject.CellHeigth;
                    colorBox.HorizontalContentAlignment = HorizontalAlignment.Left;
                    colorBox.VerticalContentAlignment = VerticalAlignment.Center;
                    colorBox.DropDownClosed += (object sender, EventArgs args) => { TextChangedControl(columnNumber, sender); LostFocusControl(sender); };
                  
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => { colorBox.Focus(); colorBox.IsDropDownOpen = true; }), System.Windows.Threading.DispatcherPriority.Render);
                                         
                    break;
                case ColumnTypes.ComboBox:
                    ComboBox comboBox = new ComboBox();
                    comboBox.ItemsSource = ComboBoxItems;
                    comboBox.SelectedItem = control.Content.ToString();
                    control.Content = comboBox;                  
                    comboBox.Width = control.ActualWidth - 6;
                    comboBox.BorderThickness = new Thickness(0, 0, 0, 0);
                    comboBox.Height = ActiveTableObject.CellHeigth;
                    comboBox.HorizontalContentAlignment = HorizontalAlignment.Left;
                    comboBox.VerticalContentAlignment = VerticalAlignment.Center;                    
                    comboBox.DropDownClosed += (object sender, EventArgs args) => { TextChangedControl(columnNumber, sender); LostFocusControl(sender); };
                  
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => { comboBox.Focus(); comboBox.IsDropDownOpen = true; }), System.Windows.Threading.DispatcherPriority.Render);

                    break;
                case ColumnTypes.Boolean:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Корректировка при вводе данных в поле с дробным числом
        /// </summary>
        private void DoubleTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int cursorPosition = (sender as TextBox).SelectionStart;
            int selectionLength = (sender as TextBox).SelectionLength;
            string newText = (sender as TextBox).Text;
            if (selectionLength > 0)
            {
                newText = newText.Remove(cursorPosition, selectionLength);
            }
            newText = newText.Insert(cursorPosition, e.Text);
            char sysSeparator = ActiveTableObject.SystemSeparator.ToCharArray()[0];
            int separatorIndex = newText.IndexOf(sysSeparator);

            bool separatorIsCorrect = !(newText.Substring(separatorIndex + 1, newText.Length - separatorIndex - 1).Contains(sysSeparator.ToString()));
            bool minusIsCorrect = !(newText.Contains("-") && newText.Substring(1, newText.Length - 1).Contains("-"));
            bool isNotNeedless = e.Text.Trim(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', sysSeparator, '-' }).Length == 0;

            if (!(separatorIsCorrect && minusIsCorrect && isNotNeedless))
            {              
                e.Handled = true;        
            }
        }

        private void TextChangedControl(int columnNumber, object sender)
        {
            if (sender is TextBox)
            {
                string name = ((sender as TextBox).Parent as Button).Name;
                if (СolumnType == ColumnTypes.Angle)
                {
                    string trimValue = (sender as TextBox).Text.Split(new char[] { ' ' })[0];
                    
                    foreach (int lineNumber in SelectedLines)
                    {                      
                        CellChange(name, columnNumber, lineNumber, trimValue);
                    }                  
                }
                else
                {
                    foreach (int lineNumber in SelectedLines)
                    {
                        CellChange(name, columnNumber, lineNumber, (sender as TextBox).Text);
                    }                     
                }              
            }
            else if (sender is ComboBox)
            {
                Button button = (sender as ComboBox).Parent as Button;
                if (button is null) return;
                string name = button.Name;
                if (СolumnType == ColumnTypes.ComboBox)
                {
                    if ((sender as ComboBox).SelectedItem == null) return;
                    foreach (int lineNumber in SelectedLines)
                    {
                        CellChange(name, columnNumber, lineNumber, (sender as ComboBox).SelectedItem.ToString());
                    }                       
                }
                else if (СolumnType == ColumnTypes.Color)
                {
                    if ((sender as ComboBox).SelectedItem == null) return;
                    foreach (int lineNumber in SelectedLines)
                    {
                        CellChange(name, columnNumber, lineNumber, (((sender as ComboBox).SelectedItem as StackPanel).Children[1] as TextBlock).Text);
                    }                       
                }
            }
            else if (sender is CheckBox)
            {
                string name = ((sender as CheckBox).Parent as Button).Name;
                if (IsMassiveCheck)
                {
                    foreach (int lineNumber in SelectedLines)
                    {
                        CellChange(name, columnNumber, lineNumber, (sender as CheckBox).IsChecked.Value.ToString());
                    }
                }
                else
                {
                    CellChange(name, columnNumber, CurStackPanel.Children.IndexOf((sender as CheckBox).Parent as Button), (sender as CheckBox).IsChecked.Value.ToString());
                }                                
            }
        }

        private void LostFocusControl(object sender)
        {
            if (sender is TextBox)
            {
                string value = (sender as TextBox).Text;

                foreach (int lineNumber in SelectedLines)
                {
                    SetContentFromType(0, CurStackPanel.Children[lineNumber] as ContentControl, value, СolumnType);
                }                                  
            }           
            else if (sender is ComboBox)
            {
                if (СolumnType == ColumnTypes.ComboBox)
                {
                    if ((sender as ComboBox).SelectedItem is null) return;
                    string value = (sender as ComboBox).SelectedItem.ToString();
                    foreach (int lineNumber in SelectedLines)
                    {
                        SetContentFromType(0, CurStackPanel.Children[lineNumber] as ContentControl, value, СolumnType);
                    }                                                         
                }
                else if (СolumnType == ColumnTypes.Color)
                {
                    if ((sender as ComboBox).SelectedItem is null) return;
                    string value = (((sender as ComboBox).SelectedItem as StackPanel).Children[1] as TextBlock).Text;
                    if ((sender as ComboBox).SelectedIndex == (sender as ComboBox).Items.Count - 1)
                    {
                        System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

                        if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            value = colorDialog.Color.Name;
                            if (colorDialog.Color.IsNamedColor == false)
                            {
                                value = $"#{value.Substring(2, 6).ToUpper()}";
                            }
                        }
                    }

                    foreach (int lineNumber in SelectedLines)
                    {
                        SetContentFromType(0, CurStackPanel.Children[lineNumber] as ContentControl, value, СolumnType);
                    }                         
                }
            }
            else if (sender is CheckBox)
            {
                string value = (sender as CheckBox).IsChecked.Value.ToString();

                if (IsMassiveCheck)
                {
                    foreach (int lineNumber in SelectedLines)
                    {
                        SetContentFromType(0, CurStackPanel.Children[lineNumber] as ContentControl, value, СolumnType);
                    }
                }               
            }
        }
    }

    /// <summary>
    /// Типы колонки
    /// </summary>
    enum ColumnTypes
    {
        String,
        Integer,
        Boolean,
        Double,
        Angle,
        Color,
        ComboBox
    }
}
