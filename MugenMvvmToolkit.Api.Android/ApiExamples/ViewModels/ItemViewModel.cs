using System.Threading;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.ViewModels;

namespace ApiExamples.ViewModels
{
    public class ItemViewModel : ViewModelBase, IHasDisplayName, IHasState
    {
        #region Fields

        private string _name;

        private static int _counter;

        #endregion

        #region Constructors

        public ItemViewModel()
        {
            Id = Interlocked.Increment(ref _counter);
            DisplayName = Id.ToString();
        }

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public int Id { get; private set; }

        public string DisplayName { get; set; }

        #endregion

        #region Implementation of interfaces

        public void LoadState(IDataContext state)
        {
            Id = state.GetData<int>("Id");
            Name = state.GetData<string>("Name");
        }

        //If the current application will be stopped
        public void SaveState(IDataContext state)
        {
            state.AddOrUpdate("Id", Id);
            state.AddOrUpdate("Name", Name);
        }

        #endregion
    }
}