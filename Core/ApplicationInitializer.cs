using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using LightInject;
using Mapster;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using VectorEditor.Core.MVVM;
using VectorEditor.Core.MVVM.ViewModelFactory;
using VectorEditor.Domain.Data;
using VectorEditor.Services.AppColorsService;
using VectorEditor.Services.ByteCompressor;
using VectorEditor.Services.CanvasOperation;
using VectorEditor.Services.RasterLoader;
using VectorEditor.Services.SerializeService.Binary;
using VectorEditor.Services.ShapeProvider;
using VectorEditor.Services.VectorLoader;
using VectorEditor.Setup.Mapping;
using VectorEditor.ViewModels;

namespace VectorEditor.Setup
{
    public sealed class ApplicationInitializer
    {
        private readonly App _application;

        private readonly ServiceContainer _container;
        private ILogger _logger;

        public ApplicationInitializer(App application)
        {
            _application = application ?? throw new ArgumentNullException(nameof(application));

            _application.DispatcherUnhandledException += ApplicationOnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            Dispatcher.CurrentDispatcher.UnhandledException += DispatcherOnUnhandledException;

            _container = new ServiceContainer(new ContainerOptions
            {
                EnableVariance = false
            });

            SetupLogger();
            RegisterComponents();
            SetupMapping();
            StartApplication();
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger.Error(e.Exception.ToString());
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Error(e.Exception.ToString());
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Fatal(e.ExceptionObject.ToString());
        }

        private void ApplicationOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Fatal(e.Exception.ToString());
        }

        private void SetupLogger()
        {
            var localFolder = AppDomain.CurrentDomain.BaseDirectory;

            var logPath = Path.Combine(localFolder, "log");

            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            var errorPathFormat = Path.Combine(logPath, "error-{Date}.json");
            var warningPathFormat = Path.Combine(logPath, "warning-{Date}.json");
            var informationPathFormat = Path.Combine(logPath, "information-{Date}.json");
            var debugPathFormat = Path.Combine(logPath, "debug-{Date}.json");

            ILogger log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.With(new ThreadIdEnricher())
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(@event =>
                        @event.Level == LogEventLevel.Error || @event.Level == LogEventLevel.Fatal)
                    .WriteTo.RollingFile(new CompactJsonFormatter(), errorPathFormat,
                        retainedFileCountLimit: 7))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(@event =>
                        @event.Level == LogEventLevel.Warning)
                    .WriteTo.RollingFile(new CompactJsonFormatter(), warningPathFormat,
                        retainedFileCountLimit: 7, fileSizeLimitBytes: 1000000))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(@event =>
                        @event.Level == LogEventLevel.Information)
                    .WriteTo.RollingFile(new CompactJsonFormatter(), informationPathFormat,
                        retainedFileCountLimit: 7, fileSizeLimitBytes: 1000000))
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Debug)
                    .WriteTo.RollingFile(new JsonFormatter(), debugPathFormat, retainedFileCountLimit: 7,
                        fileSizeLimitBytes: 1000000))
                //Todo: setup fatal error email
                //.WriteTo.Email(new EmailConnectionInfo
                //    {
                //        FromEmail = "VectorEditor log error",
                //        ToEmail = "todo: create mail and use it",
                //        MailServer = "smtp.gmail.com",
                //        NetworkCredentials = new NetworkCredential
                //        {
                //            UserName = "todo: create gmail and use it",
                //            Password = "todo: create gmail and use it"
                //        },
                //        EnableSsl = true,
                //        Port = 465,
                //        IsBodyHtml = true,
                //        EmailSubject = "Fatal Error"
                //    },
                //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}",
                //    batchPostingLimit: 10
                //    , restrictedToMinimumLevel: LogEventLevel.Error)
                .CreateLogger();

            _container.RegisterInstance(typeof(ILogger), log);
            _logger = log;
        }

        private void RegisterComponents()
        {
            //Services
            var viewModelFactory = new ViewModelFactory(_container, _logger);
            _container.RegisterInstance(typeof(IViewModelFactory), viewModelFactory);

            var modelFactory = new ModelFactory(_container, _logger);
            _container.RegisterInstance(typeof(IModelFactory), modelFactory);

            _container.Register<IBinarySerializer, ProtobufSerializer>(new PerContainerLifetime());
            _container.Register<IVectorCanvasLoader, VectorCanvasLoader>(new PerContainerLifetime());
            _container.Register<IRasterLoader, RasterLoader>(new PerContainerLifetime());
            _container.Register<ICanvasOperationService, CanvasOperationService>(new PerContainerLifetime());
            _container.Register<ByteCompressor>(new PerContainerLifetime());

            var shapeProvider = new ShapeProvider(
                _container.GetInstance<IBinarySerializer>(), 
                _container.GetInstance<ByteCompressor>(),
                _container.GetInstance<ICanvasOperationService>());

            _container.RegisterInstance<IShapeProvider>(shapeProvider);
            _container.RegisterInstance<ICanvasHistoryManager>(shapeProvider);
            _container.RegisterInstance(AppColorService.GetInstance);

            //ViewModels
            _container.Register<MainWindowViewModel>();
            _container.Register<CommandPanelViewModel>();
            _container.Register<CanvasViewModel>();
            _container.Register<Shape, PropertiesPanelViewModel>(
                (factory, model) => new PropertiesPanelViewModel(model, factory.GetInstance<AppColorService>()));

            //Models
            _container.Register<RectangleShape>();
            _container.Register<PolylineShape>();

            _container.Compile();
        }

        private void SetupMapping()
        {
            TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);

            // ReSharper disable ObjectCreationAsStatement
            new ShapesMappingProfile(_container);

            TypeAdapterConfig.GlobalSettings.Compile();
        }

        private void StartApplication()
        {
            var viewModelFactory = _container.GetInstance<IViewModelFactory>();
            var mainWindowViewModel = viewModelFactory.CreateViewModel<MainWindowViewModel>();
            var mainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowState = WindowState.Maximized
            };

            mainWindow.Show();
        }
    }
}