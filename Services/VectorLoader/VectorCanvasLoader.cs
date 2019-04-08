using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetBrains.Annotations;
using VectorEditor.Domain.Data.Persistence;
using VectorEditor.Services.SerializeService.Binary;
using VectorEditor.Services.ShapeProvider;

namespace VectorEditor.Services.VectorLoader
{
    public sealed class VectorCanvasLoader : IVectorCanvasLoader
    {
        public static string VecExtension = "vec";

        private readonly ICanvasHistoryManager _canvasHistoryManager;
        private readonly IBinarySerializer _binarySerializer;

        public VectorCanvasLoader([NotNull] ICanvasHistoryManager canvasHistoryManager,
            [NotNull] IBinarySerializer binarySerializer)
        {
            _canvasHistoryManager = canvasHistoryManager ?? throw new ArgumentNullException(nameof(canvasHistoryManager));
            _binarySerializer = binarySerializer ?? throw new ArgumentNullException(nameof(binarySerializer));
        }

        public Task SaveVectorAsync(CancellationToken token = default(CancellationToken))
        {
            using (var dialog = new FolderBrowserDialog()
            {
                SelectedPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName
            })
            {
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    var directoryPath = dialog.SelectedPath;

                    var fileName = "vector_" + DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss");
                    var fileExtension = VecExtension;
                    var filePath = Path.Combine(directoryPath, fileName + "." + fileExtension);

                    var canvasState = _canvasHistoryManager.GetSnapshot();
                    var canvasStateBytes = _binarySerializer.Serialize(canvasState);

                    File.WriteAllBytes(filePath, canvasStateBytes);
                }
                return Task.FromResult(0); //Task.CompletedTask;
            }
        }

        public Task<CanvasState> LoadVectorAsync(CancellationToken token = default(CancellationToken))
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = $".{VecExtension}";
            dlg.Filter = $"Vec files (*.{VecExtension})|*.{VecExtension}";

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                var fileBytes = File.ReadAllBytes(filename);
                var canvasState = _binarySerializer.Deserialize<CanvasState>(fileBytes);
                return Task.FromResult(canvasState);
            }

            return Task.FromResult(default(CanvasState)); //Task.CompletedTask;

        }
    }
}