using CommunityToolkit.Mvvm.ComponentModel;

namespace MepoExpedicaoRfid.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public NavigationViewModel Nav { get; }
    public StatusViewModel Status { get; }

    public DashboardViewModel Dashboard { get; }
    public FilaViewModel Fila { get; }
    public SaidaViewModel Saida { get; }
    public EntradaViewModel Entrada { get; }
    public ConsultaTagViewModel Consulta { get; }
    public ConfigViewModel Config { get; }

    [ObservableProperty] private object? currentView;

    public MainViewModel(
        NavigationViewModel nav,
        StatusViewModel status,
        DashboardViewModel dashboard,
        FilaViewModel fila,
        SaidaViewModel saida,
        EntradaViewModel entrada,
        ConsultaTagViewModel consulta,
        ConfigViewModel config)
    {
        Nav = nav;
        Status = status;

        Dashboard = dashboard;
        Fila = fila;
        Saida = saida;
        Entrada = entrada;
        Consulta = consulta;
        Config = config;

        CurrentView = dashboard;
    }
}
