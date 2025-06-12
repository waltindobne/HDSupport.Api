using HD_Support_API.Enums;
using System.ComponentModel.DataAnnotations.Schema;

[Table("TAB_Equipamentos")]
public class Equipamentos
{
    public int Id { get; set; }
    public string? Idf_Patrimonio { get; set; }
    public string? Modelo_Equipamento { get; set; }
    public string? Tpo_Equipamento { get; set; }
    public string? Dtl_Equipamento { get; set; }
    public string? Img_Equipamento { get; set; }

    private DateTime _dtaEmprestimoInicio;
    public DateTime Dta_Emprestimo_Inicio
    {
        get => DateTime.SpecifyKind(_dtaEmprestimoInicio, DateTimeKind.Utc);
        set => _dtaEmprestimoInicio = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private DateTime _dtaEmprestimoFinal;
    public DateTime Dta_Emprestimo_Final
    {
        get => DateTime.SpecifyKind(_dtaEmprestimoFinal, DateTimeKind.Utc);
        set => _dtaEmprestimoFinal = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public StatusEquipamento Stt_Equipamento { get; set; }
    public string? Profissional_Hd { get; set; }
}
