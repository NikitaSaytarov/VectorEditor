using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VectorEditor.Services.RasterLoader
{
    public interface IRasterLoader
    {
        Task SaveRasterCanvasAsync(Canvas canvas, CancellationToken token = default(CancellationToken));
        Task PrintCanvasAsync(Canvas canvas, CancellationToken token = default(CancellationToken));
    }
}