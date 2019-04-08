using System;
using JetBrains.Annotations;
using LightInject;
using Serilog;
using VectorEditor.Core.MVVM.Base;
using VectorEditor.Setup;

namespace VectorEditor.Core.MVVM.ViewModelFactory
{
    public sealed class ViewModelFactory : IViewModelFactory
    {
        private readonly IServiceFactory _container;
        private readonly ILogger _logger;

        public ViewModelFactory([NotNull] IServiceFactory serviceFactory, [NotNull] ILogger logger)
        {
            _container = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public T CreateViewModel<T>(ModelBase model = null)
            where T : ViewModelBase
        {
            if (model != null)
            {
                _logger.Debug($"Create viewModel: {typeof(T)}, model: {model.GetType()}");
                return _container.GetInstance<ModelBase, T>(model);
            }

            _logger.Debug($"Create viewModel: {typeof(T)}");
            return _container.GetInstance<T>();
        }

        public ViewModelBase CreateViewModel([NotNull] Type viewModelType)
        {
            if (viewModelType == null) throw new ArgumentNullException(nameof(viewModelType));

            if (!viewModelType.IsAssignableFrom(typeof(ViewModelBase)))
                throw new InvalidOperationException("viewModelType invalid");

            _logger.Debug($"Create viewModel: {viewModelType}");
            return (ViewModelBase)_container.GetInstance(viewModelType);
        }
    }
}