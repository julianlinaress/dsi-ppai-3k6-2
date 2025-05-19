using System.Diagnostics;
using RedSismica.Views;
using RedSismica.Models;

namespace RedSismica.ViewModels;

using System.Collections.ObjectModel;

public class VentanaCierreViewModel : ViewModelBase
{
    public ObservableCollection<DatosOrdenInspeccion> Ordenes { get; }

    public VentanaCierreViewModel()
    {
        var gestor = new GestorCierreOrdenInspeccion();
        var ordenesData = gestor.BuscarOrdenes();
        Ordenes = new ObservableCollection<DatosOrdenInspeccion>(ordenesData);
        
    }
}