

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using FrameworkElement = System.Windows.FrameworkElement;
using RoutedEventArgs = System.Windows.RoutedEventArgs;
using SizeChangedEventArgs = System.Windows.SizeChangedEventArgs;

namespace CustomChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Chartmanager chartmanager = null;
        public ChartRenderer chartRenderer = null;
        public MainWindow()
        {
            chartmanager = null;
            chartRenderer = null;

            InitializeComponent();
        }

        private void myCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Render();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Render();
        }

        private void myCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Render();
        }
        
        private void btnExportChart_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image file (*.png)|*.png";
            //saveFileDialog.Filter = "Image file (*.png)|*.png|PDF file (*.pdf)|*.pdf";
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveControlImage(customChart, saveFileDialog.FileName);                
            }
            
        }

        // Save a control's image.
        private void SaveControlImage(FrameworkElement control,
            string filename)
        {
            RenderTargetBitmap rtb = (RenderTargetBitmap)CreateBitmapFromControl(control);
            // Make a PNG encoder.
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            // Save the file.
            using (FileStream fs = new FileStream(filename,
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                encoder.Save(fs);
            }
        }
        private void SaveControltoPDF(FrameworkElement control, string filename)
        {

            var img = BitmapFromSource(CreateBitmapFromControl(control));


            //Aspose

            //// Read Height of input image
            //int h = img.Height;

            //// Read Height of input image
            //int w = img.Width;

            //// Initialize a new PDF document
            //Document doc = new Document();

            //// Add an empty page
            //Page page = doc.Pages.Add();
            //Aspose.Pdf.Image image = new Aspose.Pdf.Image();
            //image.File = filename;

            //// Set page dimensions and margins
            //page.PageInfo.Height = (h);
            //page.PageInfo.Width = (w);
            //page.PageInfo.Margin.Bottom = (0);
            //page.PageInfo.Margin.Top = (0);
            //page.PageInfo.Margin.Right = (0);
            //page.PageInfo.Margin.Left = (0);
            //page.Paragraphs.Add(image);








            //C1.PDF
            //var pdf = new C1PdfDocument(PaperKind.Letter);
            //pdf.Clear();

            //pdf.DrawImage(img, pdf.PageRectangle);

            // Save the file.
            //using (FileStream fs = new FileStream(filename,
            //    FileMode.Create, FileAccess.Write, FileShare.None))
            //{
            //    pdf.Save(fs);
            //}

        }
        private System.Drawing.Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
        }
        public BitmapSource CreateBitmapFromControl(FrameworkElement element)
        {
            // Get the size of the Visual and its descendants.
            Rect rect = VisualTreeHelper.GetDescendantBounds(element);

            // Make a DrawingVisual to make a screen
            // representation of the control.
            DrawingVisual dv = new DrawingVisual();

            // Fill a rectangle the same size as the control
            // with a brush containing images of the control.
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(element);
                ctx.DrawRectangle(brush, null, new Rect(rect.Size));
            }

            // Make a bitmap and draw on it.
            int width = (int)element.ActualWidth;
            int height = (int)element.ActualHeight;
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            return rtb;
        }
        private void btnImportCSV_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";


            if (openFileDialog.ShowDialog() == true)
            {

                if (chartmanager == null) chartmanager = new Chartmanager(openFileDialog.FileName);
                else chartmanager.setInputfile(openFileDialog.FileName);

                List<ChartData> data = chartmanager.readChart();
                csvFilepath.Text = openFileDialog.FileName;

                if ( data != null)
                {
                    foreach ( var item in data)
                    {

                        if (Char.IsDigit(item._deviation, 0)) item.deviation = Convert.ToInt32(item._deviation);
                        else item.deviation = -1;

                        if (Char.IsDigit(item._percentage, 0)) item.percentage = Convert.ToInt32(item._percentage);
                        else item.percentage = -1;

                    }
                    chartRenderer.setChatData(data);
                    totData.Text = string.Format("{0}", data.Count());
                    Render();

                } else
                {
                    string msg = chartmanager.getLastException();

                    if (msg == "System.IO.IOException") 
                        MessageBox.Show("The file is open by another process", "Error");
                    else if (msg == "CsvHelper.TypeConversion.TypeConverterException")
                    {
                        MessageBox.Show("The file format is invalid, Please check your csv file again.", "Error");
                    }
                }
            }

        }



        void Render()
        {

            if (chartRenderer == null)
            {
                chartRenderer = new ChartRenderer((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight);
            }


            chartRenderer.draw();
            myImage.Source = BmpImageFromBmp(chartRenderer.getBmp());

        }

        private static BitmapImage BmpImageFromBmp(Bitmap bmp)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
}
