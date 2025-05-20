namespace RedSismica.Models;

public class MotivoFueraServicio
{
    public required MotivoTipo Motivo { get; set; }
    public string? Comentario { get; set; }
}