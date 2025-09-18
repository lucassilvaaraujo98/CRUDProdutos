using System.Configuration;
using MySql.Data.MySqlClient;

namespace CRUDProdutos
{
    public static class Database
    {
        // Retorna uma nova conexão
        public static MySqlConnection GetConnection()
        {
            var cs = ConfigurationManager.ConnectionStrings["CrudProdutos"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new ConfigurationErrorsException("Connection string 'CrudProdutos' não encontrada no App.config");

            return new MySqlConnection(cs);
        }
    }
}
