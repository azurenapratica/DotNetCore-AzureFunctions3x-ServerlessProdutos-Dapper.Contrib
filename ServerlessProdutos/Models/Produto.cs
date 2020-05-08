using System;
using Dapper.Contrib.Extensions;

namespace ServerlessProdutos.Models
{
    [Table("dbo.Produtos")]
    public class Produto
    {
        [ExplicitKey]
        public string CodigoBarras { get; set; }       
        public string Nome { get; set; }        
        public double Preco { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
    }
}