using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace XAML_Path_Markup_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void XAMLCodeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var Transformations = new TransformGroup();
                Transformations.Children.Add(new ScaleTransform(double.Parse(SetScaleX.Text), double.Parse(SetScaleY.Text)));
                Transformations.Children.Add(new RotateTransform(double.Parse(SetRotateAngle.Text), double.Parse(SetCenterX.Text), double.Parse(SetCenterY.Text)));

                Path Drawing = new Path
                {
                    RenderTransform = Transformations,
                    Fill = Brushes.Black,
                    Data = Geometry.Parse(XAMLCodeBox.Text)
                };

                TheCanvas.Children.Clear();

                Canvas.SetTop(Drawing, 0);
                Canvas.SetLeft(Drawing, 0);
                TheCanvas.Children.Add(Drawing);
            }
            catch (Exception)
            { }
        }

        private void TheCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Coordinates.Text = Mouse.GetPosition(TheCanvas).ToString();
        }

        private void TheCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            String insertText;
            if (XAMLCodeBox.Text == "")
                insertText = "M " + Mouse.GetPosition(TheCanvas).ToString() + " Z";
            else
                insertText = " L " + Mouse.GetPosition(TheCanvas).ToString();
            var selectionIndex = XAMLCodeBox.SelectionStart;
            XAMLCodeBox.Text = XAMLCodeBox.Text.Insert(selectionIndex, insertText);
            XAMLCodeBox.SelectionStart = selectionIndex + insertText.Length;
        }

        private void FormatCode(object sender, RoutedEventArgs e)
        {
            var Parts = XAMLCodeBox.Text.Split(new string[] { "M" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Parts.Length; i++)
                Parts[i] = Parts[i].Trim(new char[] { '\n', ' ' });
            XAMLCodeBox.Text = String.Join("\nM ", Parts);

            // Fix for removing leading Ms
            int n;
            if (int.TryParse(XAMLCodeBox.Text.Substring(0, 1), out n))
                XAMLCodeBox.Text = "M " + XAMLCodeBox.Text;

            Parts = Regex.Split(XAMLCodeBox.Text, "(L)|(C)|(Z)");
            for (int i = 0; i < Parts.Length; i++)
                Parts[i] = Parts[i].Trim();
            XAMLCodeBox.Text = String.Join(" ", Parts);
        }

        private void RoundValues(object sender, RoutedEventArgs e)
        {
            var Parts = Regex.Split(XAMLCodeBox.Text, "(M)|(L)|(C)|(Z)|( )|(,)");
            for (int i = 0; i < Parts.Length; i++)
                if (Parts[i].Contains("."))
                {
                    var Number = double.Parse(Parts[i]);
                    Number = Math.Round(Number);
                    Parts[i] = Number.ToString();
                }
            XAMLCodeBox.Text = String.Join("", Parts);
        }
    }
}
