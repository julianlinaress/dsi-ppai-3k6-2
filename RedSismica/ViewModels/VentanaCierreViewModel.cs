using RedSismica.Models;

namespace RedSismica.ViewModels;

using System.Collections.ObjectModel;

public class VentanaCierreViewModel : ViewModelBase
{
    public ObservableCollection<DatosOrdenInspeccion> Ordenes { get; }
    public GestorCierreOrdenInspeccion Gestor { get; set; }

    public VentanaCierreViewModel()
    {
        Gestor = new GestorCierreOrdenInspeccion();
        var ordenesData =  Gestor.NuevoCierre();
        Ordenes = new ObservableCollection<DatosOrdenInspeccion>(ordenesData);
    }
}