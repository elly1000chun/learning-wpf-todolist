using CommunityToolkit.Mvvm.ComponentModel;

namespace TodoWpf.Models
{
    public partial class TodoItem : ObservableObject
    {
        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private bool isDone;

        [ObservableProperty]
        private DateTime createdAt = DateTime.Now;

        [ObservableProperty]
        private DateTime? updatedAt;

        [ObservableProperty]
        private DateTime? dueDate;
    }
}




