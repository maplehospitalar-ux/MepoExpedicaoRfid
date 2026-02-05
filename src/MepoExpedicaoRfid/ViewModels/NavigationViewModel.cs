using CommunityToolkit.Mvvm.Input;

namespace MepoExpedicaoRfid.ViewModels;

public sealed class NavigationViewModel
{
    public IRelayCommand Dashboard { get; private set; } = null!;
    public IRelayCommand Fila { get; private set; } = null!;
    public IRelayCommand Saida { get; private set; } = null!;
    public IRelayCommand Entrada { get; private set; } = null!;
    public IRelayCommand Consulta { get; private set; } = null!;
    public IRelayCommand Config { get; private set; } = null!;

    public void Configure(MainViewModel main)
    {
        Dashboard = new RelayCommand(() => main.CurrentView = main.Dashboard);
        Fila = new RelayCommand(() => main.CurrentView = main.Fila);
        Saida = new RelayCommand(() => main.CurrentView = main.Saida);
        Entrada = new RelayCommand(() => main.CurrentView = main.Entrada);
        Consulta = new RelayCommand(() => main.CurrentView = main.Consulta);
        Config = new RelayCommand(() => main.CurrentView = main.Config);
    }
}
