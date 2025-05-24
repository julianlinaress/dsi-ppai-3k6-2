namespace RedSismica.Models;

public class MotivoFueraServicio(MotivoTipo motivo, string? comentario)
{
    public MotivoTipo Motivo { get; set; } = motivo;
    public string? Comentario { get; set; } = comentario;
}