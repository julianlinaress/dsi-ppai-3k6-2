using System;
using System.Collections.Generic;

namespace RedSismica.Models;

public class FueraDeServicio : Estado
{
    public FueraDeServicio() : base("Fuera de Servicio", "Sismografo")
    {
    }

    public override bool EsFueraDeServicio() => true;
}
