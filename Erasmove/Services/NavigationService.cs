namespace Erasmove.Services;

public interface INavigationService
{
    Task GoBackAsync();
    Task GoToAsync(string route);
    Task GoToAsync(string route, IDictionary<string, object> parameters);
    Task DisplayAlertAsync(string title, string message, string cancel);
    Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel);
}

public class NavigationService : INavigationService
{
    public async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    public async Task GoToAsync(string route)
    {
        await Shell.Current.GoToAsync(route);
    }

    public async Task GoToAsync(string route, IDictionary<string, object> parameters)
    {
        await Shell.Current.GoToAsync(route, parameters);
    }

    public async Task DisplayAlertAsync(string title, string message, string cancel)
    {
        await Shell.Current.DisplayAlertAsync(title, message, cancel);
    }

    public async Task<bool> DisplayAlertAsync(string title, string message, string accept, string cancel)
    {
        return await Shell.Current.DisplayAlertAsync(title, message, accept, cancel);
    }
}
