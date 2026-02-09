using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MepoExpedicaoRfid.Models;
using MepoExpedicaoRfid.ViewModels;

namespace MepoExpedicaoRfid.Views;

public partial class FilaView : UserControl
{
    private DateTime _lastOpenAtUtc = DateTime.MinValue;
    private string? _lastOpenKey;
    private bool _opening;

    public FilaView()
    {
        InitializeComponent();
    }

    private async void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Duplo clique abre
        if (e.ClickCount < 2) return;

        e.Handled = true; // evita bubbling/duplicação (ex.: Border interno/TextBlock)

        var item = (sender as FrameworkElement)?.DataContext as FilaItem;
        if (item is null) return;

        if (DataContext is not FilaViewModel vm) return;

        // Debounce simples (protege contra 2 disparos do mesmo double-click)
        var key = $"{item.Origem}|{item.NumeroPedido}";
        var now = DateTime.UtcNow;

        if (_opening) return;
        if (_lastOpenKey == key && (now - _lastOpenAtUtc).TotalMilliseconds < 600) return;

        _lastOpenKey = key;
        _lastOpenAtUtc = now;
        _opening = true;

        try
        {
            await vm.AbrirItem.ExecuteAsync(item);
        }
        catch
        {
            // sem crash de UI
        }
        finally
        {
            _opening = false;
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
