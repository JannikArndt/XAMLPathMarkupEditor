using Microsoft.Win32;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

                var Drawing = new System.Windows.Shapes.Path
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

        private void Print(object sender, RoutedEventArgs e)
        {
            var OriginalSize = new Size(TheCanvas.ActualWidth, TheCanvas.ActualHeight);
            TheCanvas.Width = (TheCanvas.Children[0] as Path).ActualWidth;
            TheCanvas.Height = (TheCanvas.Children[0] as Path).ActualHeight;
            TheCanvas.HorizontalAlignment = HorizontalAlignment.Left;
            TheCanvas.VerticalAlignment = VerticalAlignment.Top;

            try
            {
                PrintDialog dialog = new PrintDialog();

                if (dialog.ShowDialog() != true)
                    return;
                dialog.PrintVisual(TheCanvas, "Drawing");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error while printing", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            TheCanvas.Width = OriginalSize.Width;
            TheCanvas.Height = OriginalSize.Height;
            TheCanvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            TheCanvas.VerticalAlignment = VerticalAlignment.Stretch;
        }
        private void Export(object sender, RoutedEventArgs e)
        {
            var OriginalSize = new Size(TheCanvas.ActualWidth, TheCanvas.ActualHeight);
            TheCanvas.Width = (TheCanvas.Children[0] as Path).ActualWidth;
            TheCanvas.Height = (TheCanvas.Children[0] as Path).ActualHeight;
            TheCanvas.HorizontalAlignment = HorizontalAlignment.Left;
            TheCanvas.VerticalAlignment = VerticalAlignment.Top;

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                ValidateNames = true,
                AddExtension = true,
                FileName = "Drawing.png",
                Filter = "PNG Image|*.png",
                FilterIndex = 1,
                RestoreDirectory = true,
                OverwritePrompt = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filetype = System.IO.Path.GetExtension(saveFileDialog.FileName);
                string filename = saveFileDialog.FileName;
                string filePath = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)TheCanvas.RenderSize.Width,
                    (int)TheCanvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
                rtb.Render(TheCanvas);

                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                using (var fs = System.IO.File.OpenWrite(filename))
                {
                    pngEncoder.Save(fs);
                }
            }

            TheCanvas.Width = OriginalSize.Width;
            TheCanvas.Height = OriginalSize.Height;
            TheCanvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            TheCanvas.VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}
