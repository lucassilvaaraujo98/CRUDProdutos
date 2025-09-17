using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using CRUDProdutos.Models;

namespace CRUDProdutos
{
    public partial class ProdutoForm : Form
    {
        private Produto produto;

        public ProdutoForm()
        {
            InitializeComponent();
        }

        public ProdutoForm(Produto p) : this()
        {
            produto = p;
            txtCodigo.Text = produto.Codigo;
            txtNome.Text = produto.Nome;
            cmbUnidade.SelectedValue = produto.UnidadeId;
            numPreco.Value = produto.Preco;
            numQuantidade.Value = produto.Quantidade;
        }

        private async void ProdutoForm_Load(object sender, EventArgs e)
        {
            await CarregarUnidadesAsync();
        }

        private async Task CarregarUnidadesAsync()
        {
            try
            {
                var dt = new DataTable();
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id, nome FROM unidade_medida ORDER BY nome";
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                cmbUnidade.DataSource = dt;
                cmbUnidade.DisplayMember = "nome";
                cmbUnidade.ValueMember = "id";

                if (produto != null)
                    cmbUnidade.SelectedValue = produto.UnidadeId;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar unidades: " + ex.Message);
            }
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigo.Text.Trim();
            string nome = txtNome.Text.Trim();
            int unidadeId = Convert.ToInt32(cmbUnidade.SelectedValue);
            decimal preco = numPreco.Value;
            int quantidade = (int)numQuantidade.Value;

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(nome))
            {
                MessageBox.Show("Preencha todos os campos.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd;

                    if (produto == null)
                    {
                        cmd = new MySqlCommand(
                            "INSERT INTO produto (codigo, nome, unidade_id, preco, quantidade) VALUES (@codigo, @nome, @unidade, @preco, @quantidade)",
                            conn);
                    }
                    else
                    {
                        cmd = new MySqlCommand(
                            "UPDATE produto SET codigo=@codigo, nome=@nome, unidade_id=@unidade, preco=@preco, quantidade=@quantidade WHERE id=@id",
                            conn);
                        cmd.Parameters.AddWithValue("@id", produto.Id);
                    }

                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@unidade", unidadeId);
                    cmd.Parameters.AddWithValue("@preco", preco);
                    cmd.Parameters.AddWithValue("@quantidade", quantidade);

                    cmd.ExecuteNonQuery();
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                MessageBox.Show("Código já existe!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar produto: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
