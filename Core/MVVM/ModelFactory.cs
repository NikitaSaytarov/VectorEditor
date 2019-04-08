using System;
using JetBrains.Annotations;
using LightInject;
using Serilog;
using VectorEditor.Core.MVVM.Base;

namespace VectorEditor.Core.MVVM
{
    public class ModelFactory : IModelFactory
    {
        [NotNull] private readonly IServiceFactory _serviceFactory;
        [NotNull] private readonly ILogger _logger;

        public ModelFactory([NotNull] IServiceFactory serviceFactory, [NotNull] ILogger logger)
        {
            _serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public T CreateModel<T>()
            where T : ModelBase
        {
            _logger.Debug($"Create model: {typeof(T)}");
            return _serviceFactory.GetInstance<T>();
        }

        public T CreateModel<TParam, T>(TParam parameter)
            where T : ModelBase
        {
            var type = typeof(T);
            _logger.Debug($"Create model: {type}");
            return _serviceFactory.GetInstance<TParam, T>(parameter);
        }
    }
}