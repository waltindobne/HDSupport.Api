using System.ComponentModel;

namespace HD_Support_API.Enums
{
    public enum StatusConversa
    {
        [Description("Não Aceito")]
        NaoAceito = 1,
        [Description("Encerrado")]
        Encerrado = 2,
        [Description("Em andamento")]
        EmAndamento = 3,
    }
}
