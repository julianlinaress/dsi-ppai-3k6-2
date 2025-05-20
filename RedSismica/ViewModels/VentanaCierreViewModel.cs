// En este archivo se implementa la funcionalidades de la ventana de cierre de ordenes de inspección.
// que se abre despues de que se haga clik en el botón "Registrar Cierre".
using System.Diagnostics;
using RedSismica.Views;
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
        var ordenesData =  Gestor.BuscarOrdenes();
        Ordenes = new ObservableCollection<DatosOrdenInspeccion>(ordenesData);
        
    }
}