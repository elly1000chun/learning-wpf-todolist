using CommunityToolkit.Mvvm.ComponentModel;

namespace TodoWpf.Models
{
    public partial class TodoItem : ObservableObject
    {
        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private bool isDone;
    }
}




