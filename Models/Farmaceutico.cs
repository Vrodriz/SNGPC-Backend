namespace SNGPC_B.Api.Models
{
    public class Farmaceutico
    {
        public int Id { get; set; }
        public required string Nome { get; set; } = string.Empty;
        public string CRF { get; set; } = string.Empty;
        public string CRFUF { get; set; } = string.Empty;
        public DateTime CRFDataEmissao { get; set; }
        public string CPF { get; set; } = string.Empty;
        public string? LoginANVISA { get; set; }
        public string? SenhaANVISA { get; set; }
    }
}