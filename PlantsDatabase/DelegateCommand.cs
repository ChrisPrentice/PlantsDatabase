using System;
using System.ComponentModel;
using System.Windows.Input;

namespace PlantsDatabase
{
    public class DelegateCommand<T> : ICommand where T : class
    {
        protected bool _isEnabled = true;
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute = null, string name = null)
        {
            if (null == execute)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute ?? (t => _isEnabled);
        }

        public bool CanExecute(object parameter)
        {
            var canExecute = true;

            if (typeof(T) == typeof(bool))
            {
                if (null == _canExecute) return true;

                var castbool = _canExecute as Predicate<bool>;
                canExecute = castbool(false);
            }
            else
            {
                if (null != this._canExecute)
                {
                    canExecute = _canExecute((T)parameter);
                }
            }

            return canExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}

namespace PlantsDatabase.IRIS.Law.SharedWpf
{
    public class DelegateCommand : DelegateCommand<object>, INotifyPropertyChanged
    {
        public DelegateCommand(Action execute, string name = null)
            : base(o => execute(), null, name)
        {
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
                RaiseCanExecuteChanged();
            }
        }

        public void Execute()
        {
            Execute(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler pceh = PropertyChanged;
            pceh?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}