using Erasmove.ViewModels;

namespace Erasmove.Views;

public partial class AddLieuView : ContentPage
{
    public AddLieuView(AddLieuViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}