using System.ComponentModel.DataAnnotations;

namespace CriptoMoed.Model
{
    public class Carteira
    {
        [Key]
        public string numeroConta { get; set; }
        public int idMoeda { get; set; }
        public string codResponsavel { get; set; }
        public string? nomeResponsavel { get; set; }
        public decimal qtdMoedas { get; set; }
        public string? statusConta { get; set; }

        public Carteira(int idMoeda, string codResponsavel, string nomeResponsavel, decimal totalConta, string numeroConta)
        {
            this.idMoeda = idMoeda;
            this.codResponsavel = codResponsavel;
            this.nomeResponsavel = nomeResponsavel;
            this.qtdMoedas = totalConta;
            this.numeroConta = numeroConta;
            if (qtdMoedas > 0 ) {
                this.statusConta = "Positivo";
            }
            else {
                this.statusConta = "Neutro";
            }

            
        }

        public Carteira()
        {
        }

    }
}
