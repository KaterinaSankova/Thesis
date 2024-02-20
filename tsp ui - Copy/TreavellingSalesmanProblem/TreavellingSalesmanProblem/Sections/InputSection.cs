using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using TSP.Elements;
using TSP.Enum;

namespace TSP.Sections
{
    public class InputSection : Grid
    {
        private InputMode Mode = InputMode.File;
        //file
        private string? sourceFilePath;
        private string? resultFilePath;
        //folder
        private string? sourceFolderPath;
        private string? resultFolderPath;
        //generate
        private int? numberOfSamples;
        private int? numberOfCities;
        private (double LowestX, double HighestX, double LowestY, double HighestY)? extremeCoordinatesValues;

        private FilePicker? sourceFilePicker;
        private FilePicker? resultFilePicker;
        private FolderPicker? sourceFolderPicker;
        private FolderPicker? resultFolderPicker;
        private TextBox? numberOfSamplesInput;
        private TextBox? numberOfCitiesInput;
        private StackPanel? extremeCoordinatesValuesInput;

        public InputMode InputMode { get { return Mode; } }
        public string SourcePath
        {
            get
            {
                if (Mode == InputMode.File)
                    return sourceFilePath;
                else if (Mode == InputMode.Folder)
                    return sourceFolderPath;
                else
                    throw new Exception("Property SourcePath is not available for this mode.");
            }
        }
        public string ResultPath
        {
            get
            {
                if (Mode == InputMode.File)
                    return resultFilePath;
                else if (Mode == InputMode.Folder)
                    return resultFolderPath;
                else
                    throw new Exception("Property ResultPath is not available for this mode.");
            }
        }

        public int NumberOfSamples
        {
            get
            {
                if (Mode == InputMode.Generate)
                {
                    return (int)numberOfSamples;
                }
                else
                {
                    throw new Exception("Property NumberOfSamples is not available for this mode.");
                }
            }
        }

        public int NumberOfCitites
        {
            get
            {
                if (Mode == InputMode.Generate)
                {
                    return (int)numberOfCities;
                }
                else
                {
                    throw new Exception("Property NumberOfCitites is not available for this mode.");
                }
            }
        }

        public (double LowestX, double HighestX, double LowestY, double HighestY) ExtremeCoordinateValues
        {
            get
            {
                if (Mode == InputMode.Generate)
                {
                    return ((double LowestX, double HighestX, double LowestY, double HighestY))extremeCoordinatesValues;
                }
                else
                {
                    throw new Exception("Property NumberOfCitites is not available for this mode.");
                }
            }
        }

        public InputSection() => DrawFileInputSection();

        public InputSection(int row, int column) : this()
        {
            this.SetValue(Grid.RowProperty, row);
            this.SetValue(Grid.ColumnProperty, column);
        }

        private void DrawFileInputSection()
        {
            PreparePathInputGrid();
            if (this.Children.Count == 0)
                CreateInputTypeSelectionElement();

            sourceFilePicker = new FilePicker("tsp");
            this.Children.Add(new LabeledElement("Source:", sourceFilePicker, 1, 0));

            resultFilePicker = new FilePicker("opt.tour");
            this.Children.Add(new LabeledElement("Result:", resultFilePicker, 2, 0));

            Mode = InputMode.File;
        }

        private void DrawFolderInputSection()
        {
            PreparePathInputGrid();
            if (this.Children.Count == 0)
                CreateInputTypeSelectionElement();

            sourceFolderPicker = new FolderPicker();
            this.Children.Add(new LabeledElement("Source:", sourceFolderPicker, 1, 0));

            resultFolderPicker = new FolderPicker();
            this.Children.Add(new LabeledElement("Result:", resultFolderPicker, 2, 0));

            Mode = InputMode.Folder;
        }

        private void DrawGenerateInputSection()
        {
            PrepareGenerateInputGrid();
            if (this.Children.Count == 0)
                CreateInputTypeSelectionElement();

            numberOfSamplesInput = new TextBox();
            numberOfSamplesInput.Text = "1";
            numberOfSamplesInput.Height = 20;
            numberOfSamplesInput.Width = 35;
            numberOfSamplesInput.HorizontalAlignment = HorizontalAlignment.Left;
            numberOfSamplesInput.SetValue(Grid.RowProperty, 1);
            numberOfSamplesInput.SetValue(Grid.ColumnProperty, 1);
            numberOfSamplesInput.LostFocus += TestSampleNumberInput;
            this.Children.Add(new LabeledElement("Nr. of samples:", numberOfSamplesInput, 1, 0));

            numberOfCitiesInput = new TextBox();
            numberOfCitiesInput.Text = "20";
            numberOfCitiesInput.Height = 20;
            numberOfCitiesInput.Width = 35;
            numberOfCitiesInput.HorizontalAlignment = HorizontalAlignment.Left;
            numberOfCitiesInput.SetValue(Grid.RowProperty, 2);
            numberOfCitiesInput.SetValue(Grid.ColumnProperty, 1);
            numberOfCitiesInput.LostFocus += TestNumberOfCitiesInput;
            this.Children.Add(new LabeledElement("Nr. of cities:", numberOfCitiesInput, 2, 0));

            extremeCoordinatesValuesInput = new StackPanel();
            extremeCoordinatesValuesInput.SetValue(Grid.RowProperty, 3);
            extremeCoordinatesValuesInput.SetValue(Grid.ColumnProperty, 0);
            extremeCoordinatesValuesInput.Orientation = Orientation.Horizontal;
            extremeCoordinatesValuesInput.HorizontalAlignment = HorizontalAlignment.Left;
            extremeCoordinatesValuesInput.VerticalAlignment = VerticalAlignment.Center;
            extremeCoordinatesValuesInput.LostFocus += TestExtremeCoordinateValues;
            this.Children.Add(extremeCoordinatesValuesInput);

            var labels = new string[] { "Lowest X:", "Higest X:", "Lowest Y:", "Higest Y:" };
            var defaultValues = new string[] { "-20", "20", "-20", "20" };
            for (int i = 0; i < 4; i++)
            {
                var inputBox = new TextBox();
                inputBox.Text = defaultValues[i];
                inputBox.Height = 20;
                inputBox.Width = 50;
                inputBox.HorizontalAlignment = HorizontalAlignment.Left;
                inputBox.LostFocus += TestCoordinateValueInput;
                var labeledInput = new LabeledElement(labels[i], inputBox);
                labeledInput.Margin = new Thickness(0, 0, 10, 0);
                extremeCoordinatesValuesInput.Children.Add(labeledInput);

            }

            Mode = InputMode.Generate;
        }

        private void SwitchToFileInput(object sender, RoutedEventArgs e)
        {
            if (Mode != InputMode.File)
                DrawFileInputSection();
        }

        private void SwitchToFolderInput(object sender, RoutedEventArgs e)
        {
            if (Mode != InputMode.Folder)
                DrawFolderInputSection();
        }

        private void SwitchToGenerateInput(object sender, RoutedEventArgs e)
        {
            if (Mode != InputMode.Generate)
                DrawGenerateInputSection();
        }

        private void TestIntegerNumberInput(TextBox textBox, int minValue, int maxValue)
        {
            int numberOfSamples;

            bool isNumber = int.TryParse(textBox.Text, out numberOfSamples);
            if (!isNumber)
                textBox.Text = minValue.ToString();
            if (numberOfSamples < minValue)
                textBox.Text = minValue.ToString();
            if (numberOfSamples > maxValue)
                textBox.Text = maxValue.ToString();
        }

        private void TestDoubleNumberInput(TextBox textBox, int minValue, int maxValue)
        {
            double numberOfSamples;

            bool isNumber = double.TryParse(textBox.Text, out numberOfSamples);
            if (!isNumber)
                textBox.Text = "0";
            if (numberOfSamples < minValue)
                textBox.Text = minValue.ToString();
            if (numberOfSamples > maxValue)
                textBox.Text = maxValue.ToString();

        }

        private void TestSampleNumberInput(object sender, RoutedEventArgs e) => TestIntegerNumberInput((TextBox)sender, 1, 100);

        private void TestNumberOfCitiesInput(object sender, RoutedEventArgs e) => TestIntegerNumberInput((TextBox)sender, 1, 5000);

        private void TestCoordinateValueInput(object sender, RoutedEventArgs e) => TestDoubleNumberInput((TextBox)sender, -500000, 500000);


        private void TestExtremeCoordinateValues(object sender, RoutedEventArgs e)
        {
            double lowestX, highestX, lowestY, highestY;
            var stackPanel = (StackPanel)sender;
            double.TryParse(((TextBox)((LabeledElement)stackPanel.Children[0]).Children[1]).Text, out lowestX);
            double.TryParse(((TextBox)((LabeledElement)stackPanel.Children[1]).Children[1]).Text, out highestX);
            double.TryParse(((TextBox)((LabeledElement)stackPanel.Children[2]).Children[1]).Text, out lowestY);
            double.TryParse(((TextBox)((LabeledElement)stackPanel.Children[3]).Children[1]).Text, out highestY);

            if (lowestX > highestX)
                ((TextBox)((LabeledElement)stackPanel.Children[0]).Children[1]).Text = "-500000";
            if (lowestY > highestY)
                ((TextBox)((LabeledElement)stackPanel.Children[2]).Children[1]).Text = "-500000";
        }

        private void CreateInputTypeSelectionElement()
        {
            List<(string Label, RoutedEventHandler Handler)> buttonProperties = [
                ("File", SwitchToFileInput),
                ("Folder", SwitchToFolderInput),
                ("Generate", SwitchToGenerateInput)
            ];
            var inputRadioButtonGroup = new RadioButtonGroup(buttonProperties, "input");
            this.Children.Add(new LabeledElement("Input:", inputRadioButtonGroup, 0, 0));
        }

        private void RestoreInputSelectionGrid()
        {
            while (this.Children.Count > 1)
                this.Children.RemoveAt(1);
            while (this.RowDefinitions.Count > 1)
                this.RowDefinitions.RemoveAt(1);
            while (this.ColumnDefinitions.Count > 1)
                this.ColumnDefinitions.RemoveAt(1);
        }

        private void PreparePathInputGrid()
        {
            if (this.ColumnDefinitions.Count == 0)
            {
                GridExtentions.AddColumnToGrid(this, 500);
                GridExtentions.AddRowToGrid(this, 30);
            }
            else
            {
                RestoreInputSelectionGrid();
            }

            GridExtentions.AddRowToGrid(this, 30);
            GridExtentions.AddRowToGrid(this, 30);
        }

        private void PrepareGenerateInputGrid()
        {
            if (this.ColumnDefinitions.Count == 0)
            {
                GridExtentions.AddColumnToGrid(this, 500);
                GridExtentions.AddRowToGrid(this, 30);
            }
            else
            {
                RestoreInputSelectionGrid();
            }

            GridExtentions.AddRowToGrid(this, 30);
            GridExtentions.AddRowToGrid(this, 30);
            GridExtentions.AddRowToGrid(this, 30);
            GridExtentions.AddRowToGrid(this, 30);
        }
    }
}
