using System;
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
            var insertText = " L " + Mouse.GetPosition(TheCanvas).ToString();
            var selectionIndex = XAMLCodeBox.SelectionStart;
            XAMLCodeBox.Text = XAMLCodeBox.Text.Insert(selectionIndex, insertText);
            XAMLCodeBox.SelectionStart = selectionIndex + insertText.Length;
        }
    }
}
