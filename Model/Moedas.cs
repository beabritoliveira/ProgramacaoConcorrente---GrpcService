using System.ComponentModel.DataAnnotations;

namespace CriptoMoed.Model
{
    public class Moedas
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }

        public Moedas(int Id, string Nome, decimal Valor)
        {
            Id = Id;
            Nome = Nome;
            Valor = Valor;
        }


        public Moedas()
        {
        }
    }
}
