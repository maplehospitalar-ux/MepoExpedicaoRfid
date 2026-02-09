using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MepoExpedicaoRfid.Models;
using MepoExpedicaoRfid.ViewModels;

namespace MepoExpedicaoRfid.Views;

public partial class FilaView : UserControl
{
    public FilaView()
    {
        InitializeComponent();
    }

    private async void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Duplo clique abre
        if (e.ClickCount < 2) return;

        var item = (sender as FrameworkElement)?.DataContext as FilaItem;
        if (item is null) return;

        if (DataContext is not FilaViewModel vm) return;

        try
        {
            await vm.AbrirItem.ExecuteAsync(item);
        }
        catch
        {
            // sem crash de UI
        }
    }

    private async void MenuAbrir_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not FilaViewModel vm) return;

        var item = GetItemFromMenuSender(sender);
        if (item is null) return;

        try
        {
            await vm.AbrirItem.ExecuteAsync(item);
        }
        catch { }
    }

    private async void MenuReimprimir_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is not FilaViewModel vm) return;

        var item = GetItemFromMenuSender(sender);
        if (item is null) return;

        try
        {
            await vm.ReimprimirItem.ExecuteAsync(item);
        }
        catch { }
    }

    private static FilaItem? GetItemFromMenuSender(object sender)
    {
        if (sender is not MenuItem mi) return null;
        if (mi.Parent is not ContextMenu cm) return null;
        return (cm.PlacementTarget as FrameworkElement)?.DataContext as FilaItem;
    }
}
