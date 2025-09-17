namespace CRUDProdutos.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }  
        public string Nome { get; set; }
        public int UnidadeId { get; set; }
        public string UnidadeNome { get; set; }
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
    }
}
