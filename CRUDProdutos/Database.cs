using System.Configuration;
using MySql.Data.MySqlClient;

namespace CRUDProdutos
{
    public static class Database
    {
        // Retorna uma nova conexão — o caller deve usar "using" para fechar/dispô-la.
        public static MySqlConnection GetConnection()
        {
            var cs = ConfigurationManager.ConnectionStrings["CrudProdutos"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new ConfigurationErrorsException("Connection string 'CrudProdutos' não encontrada no App.config");

            return new MySqlConnection(cs);
        }
    }
}
