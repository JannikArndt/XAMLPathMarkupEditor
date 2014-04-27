using System;
using System.Windows;
using System.Windows.Controls;
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
    }
}
