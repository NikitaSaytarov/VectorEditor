using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VectorEditor.Services.RasterLoader
{
    public sealed class RasterLoader : IRasterLoader
    {
        private const double defaultDpi = 96.0;

        public Task SaveRasterCanvasAsync(Canvas canvas, CancellationToken token = default(CancellationToken))
        {
            using (var dialog = new FolderBrowserDialog
            {
                SelectedPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
                    .FullName
            })
            {
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    var renderTargetBitmap = GetRenderTargetBitmapFromControl(canvas);

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                    var directoryPath = dialog.SelectedPath;
                    var fileName = "ruster_" + DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss");
                    var fileExtension = "png";
                    var filePath = Path.Combine(directoryPath, fileName + "." + fileExtension);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }

                return Task.FromResult(0); //Task.CompletedTask;
            }
        }

        public Task PrintCanvasAsync(Canvas canvas, CancellationToken token = default(CancellationToken))
        {
            System.Windows.Controls.PrintDialog printDialog = new System.Windows.Controls.PrintDialog();
            System.Drawing.Printing.PrinterSettings printer = new System.Drawing.Printing.PrinterSettings();
            System.Printing.LocalPrintServer localPrintServer = new System.Printing.LocalPrintServer();

            System.Printing.PrintTicket pt = new System.Printing.PrintTicket();
            System.Printing.PrintQueue pq = new System.Printing.PrintQueue(localPrintServer, printer.PrinterName, System.Printing.PrintSystemDesiredAccess.UsePrinter);

            System.Printing.PageMediaSize PMS = new System.Printing.PageMediaSize(canvas.ActualWidth + 20, canvas.ActualHeight + 20);
            System.Windows.Size pageSize = new System.Windows.Size(canvas.ActualWidth + 20, canvas.ActualHeight + 20);
            canvas.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
            canvas.Measure(pageSize);

            pt.PageMediaSize = PMS;
            pt.PageMediaType = System.Printing.PageMediaType.Unknown;
            
            pq.DefaultPrintTicket.PageMediaSize = PMS;
            pq.DefaultPrintTicket.PageMediaType = System.Printing.PageMediaType.Unknown;
            
            printDialog.PrintQueue = pq;
            printDialog.PrintTicket = pt;
            //printDialog.PrintQueue.Commit();
            
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(canvas, "Состояние холста");
            }
            return Task.FromResult(0); //Task.CompletedTask;
        }


        private BitmapSource GetRenderTargetBitmapFromControl(Visual targetControl, double dpi = defaultDpi)
        {
            if (targetControl == null) return null;

            var bounds = VisualTreeHelper.GetDescendantBounds(targetControl);
            var renderTargetBitmap = new RenderTargetBitmap((int)(bounds.Width * dpi / 96.0),
                                                            (int)(bounds.Height * dpi / 96.0),
                                                            dpi,
                                                            dpi,
                                                            PixelFormats.Pbgra32);

            var drawingVisual = new DrawingVisual();

            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush(targetControl);
                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(), bounds.Size));
            }

            renderTargetBitmap.Render(drawingVisual);
            return renderTargetBitmap;
        }
    }
}
