using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace VectorEditor.Core.MVVM.Helpers
{
    public class EventCommandExecuter : TriggerAction<DependencyObject>
    {
        #region Constructors

        public EventCommandExecuter()
            : this(CultureInfo.CurrentCulture)
        {
        }

        public EventCommandExecuter(CultureInfo culture)
        {
            Culture = culture;
        }

        #endregion

        #region Properties

        #region Command

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EventCommandExecuter), new PropertyMetadata(null));

        #endregion

        #region EventArgsConverterParameter

        public object EventArgsConverterParameter
        {
            get { return (object)GetValue(EventArgsConverterParameterProperty); }
            set { SetValue(EventArgsConverterParameterProperty, value); }
        }

        public static readonly DependencyProperty EventArgsConverterParameterProperty =
            DependencyProperty.Register("EventArgsConverterParameter", typeof(object), typeof(EventCommandExecuter), new PropertyMetadata(null));

        #endregion

        public IValueConverter EventArgsConverter { get; set; }

        public CultureInfo Culture { get; set; }

        #endregion

        protected override void Invoke(object parameter)
        {
            var cmd = Command;

            if (cmd != null)
            {
                var param = parameter;

                if (EventArgsConverter != null)
                {
                    param = EventArgsConverter.Convert(parameter, typeof(object), EventArgsConverterParameter, CultureInfo.InvariantCulture);
                }

                if (cmd.CanExecute(param))
                {
                    cmd.Execute(param);
                }
            }
        }
    }
}