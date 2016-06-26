
using System.ComponentModel;

namespace PlantsDatabase
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetValue<T>(ref T target, T value, params string[] changedProperties)
        {
            if (object.Equals(target, value)) return false;

            target = value;

            foreach (var property in changedProperties)
            {
                this.OnPropertyChanged(property);
            }

            return true;
        }
    }
}
