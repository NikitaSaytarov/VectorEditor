using System.Threading;
using System.Threading.Tasks;
using VectorEditor.Domain.Data.Persistence;

namespace VectorEditor.Services.VectorLoader
{
    public interface IVectorCanvasLoader
    {
        Task SaveVectorAsync(CancellationToken token = default(CancellationToken));
        Task<CanvasState> LoadVectorAsync(CancellationToken token = default(CancellationToken));
    }
}