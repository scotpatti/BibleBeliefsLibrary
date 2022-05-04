using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BibleBeliefs.Repository
{
    public class BaseDTO : INotifyPropertyChanged
    {
        #region INPC
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!(field == null) && field.Equals(value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
        #endregion
    }
}
