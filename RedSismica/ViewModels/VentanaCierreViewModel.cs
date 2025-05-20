using System.Linq;
using RedSismica.Models;

namespace RedSismica.ViewModels;

using System.Collections.ObjectModel;

public class VentanaCierreViewModel : ViewModelBase
{
    public ObservableCollection<DatosOrdenInspeccion>? Ordenes { get; set; }
    public GestorCierreOrdenInspeccion Gestor { get; set; }

    public VentanaCierreViewModel()
    {
        Gestor = new GestorCierreOrdenInspeccion();
        Gestor.NuevoCierre(this);
    }

    public void MostrarOrdenesParaSeleccion(IOrderedEnumerable<DatosOrdenInspeccion> ordenesData)
    {
        Ordenes = new ObservableCollection<DatosOrdenInspeccion>(ordenesData);
    }
}