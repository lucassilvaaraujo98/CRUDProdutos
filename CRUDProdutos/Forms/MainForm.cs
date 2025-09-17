using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using CRUDProdutos.Models;

namespace CRUDProdutos
{
    public partial class MainForm : Form
    {
        private Usuario usuarioLogado;
        private Panel panelBotoes;

        public MainForm(Usuario u)
        {
            InitializeComponent();
            usuarioLogado = u;

            // Configura Form
            this.BackColor = Color.WhiteSmoke;
            this.Text = "Cadastro de Produtos";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 800;
            this.Height = 600;

            // Configura DataGridView
            dgvProdutos.Dock = DockStyle.Fill;
            dgvProdutos.EnableHeadersVisualStyles = false;
            dgvProdutos.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkSlateGray;
            dgvProdutos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProdutos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvProdutos.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvProdutos.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvProdutos.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgvProdutos.GridColor = Color.DarkGray;
            dgvProdutos.RowHeadersVisible = false;
            dgvProdutos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProdutos.MultiSelect = false;
            dgvProdutos.AllowUserToAddRows = false;
            dgvProdutos.AllowUserToResizeColumns = false;
            dgvProdutos.AllowUserToResizeRows = false;
            dgvProdutos.DoubleClick += (s, e) => EditarProdutoSelecionado();
            this.Controls.Add(dgvProdutos);

            // Cria painel de botões na parte inferior
            panelBotoes = new Panel();
            panelBotoes.Dock = DockStyle.Bottom;
            panelBotoes.Height = 60;
            panelBotoes.BackColor = Color.WhiteSmoke;
            this.Controls.Add(panelBotoes);

            // Configura botões e centraliza horizontalmente
            ConfigurarBotao(btnAdicionar, Color.SeaGreen);
            ConfigurarBotao(btnEditar, Color.DodgerBlue);
            ConfigurarBotao(btnExcluir, Color.IndianRed);
            ConfigurarBotao(btnAtualizar, Color.DarkOrange);

            CentralizarBotoes();

            // Permissões
            btnAdicionar.Enabled = usuarioLogado.Tipo == "admin";
            btnEditar.Enabled = usuarioLogado.Tipo == "admin";
            btnExcluir.Enabled = usuarioLogado.Tipo == "admin";
        }

        private void ConfigurarBotao(Button botao, Color corFundo)
        {
            botao.BackColor = corFundo;
            botao.ForeColor = Color.White;
            botao.FlatStyle = FlatStyle.Flat;
            botao.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            botao.Size = new Size(100, 35);
            panelBotoes.Controls.Add(botao);
        }

        private void CentralizarBotoes()
        {
            int totalLargura = 0;
            foreach (Control ctrl in panelBotoes.Controls)
                totalLargura += ctrl.Width + 10;

            int startX = (panelBotoes.Width - totalLargura + 10) / 2;
            int x = startX;
            foreach (Control ctrl in panelBotoes.Controls)
            {
                ctrl.Location = new Point(x, (panelBotoes.Height - ctrl.Height) / 2);
                x += ctrl.Width + 10;
            }

            panelBotoes.Resize += (s, e) => CentralizarBotoes();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await CarregarProdutosAsync();
        }

        private async Task CarregarProdutosAsync()
        {
            try
            {
                var dt = new DataTable();
                await Task.Run(() =>
                {
                    using (var conn = Database.GetConnection())
                    {
                        conn.Open();
                        string query = @"SELECT p.id, p.codigo, p.nome, p.unidade_id, u.nome AS unidade, p.preco, p.quantidade
                                         FROM produto p
                                         JOIN unidade_medida u ON p.unidade_id = u.id
                                         ORDER BY p.id;";
                        using (var cmd = new MySqlCommand(query, conn))
                        using (var adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                });

                dgvProdutos.DataSource = dt;

                // Ajuste proporcional das colunas
                dgvProdutos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                dgvProdutos.Columns["id"].HeaderText = "ID";
                dgvProdutos.Columns["id"].FillWeight = 20;
                dgvProdutos.Columns["codigo"].HeaderText = "Código";
                dgvProdutos.Columns["codigo"].FillWeight = 30;
                dgvProdutos.Columns["nome"].HeaderText = "Nome";
                dgvProdutos.Columns["nome"].FillWeight = 50;
                dgvProdutos.Columns["unidade"].HeaderText = "Unidade";
                dgvProdutos.Columns["unidade"].FillWeight = 30;
                dgvProdutos.Columns["preco"].HeaderText = "Preço";
                dgvProdutos.Columns["preco"].FillWeight = 30;
                dgvProdutos.Columns["quantidade"].HeaderText = "Quantidade";
                dgvProdutos.Columns["quantidade"].FillWeight = 30;

                if (dgvProdutos.Columns.Contains("unidade_id"))
                    dgvProdutos.Columns["unidade_id"].Visible = false;

                dgvProdutos.Columns["id"].ReadOnly = true;
                dgvProdutos.Columns["codigo"].ReadOnly = true;
                dgvProdutos.Columns["preco"].DefaultCellStyle.Format = "C2";
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("Erro ao carregar produtos: " + ex.Message);
            }
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            using (var form = new ProdutoForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                    _ = CarregarProdutosAsync();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e) => EditarProdutoSelecionado();

        private void EditarProdutoSelecionado()
        {
            if (dgvProdutos.CurrentRow == null) return;

            var row = dgvProdutos.CurrentRow;
            var produto = new Produto
            {
                Id = Convert.ToInt32(row.Cells["id"].Value),
                Codigo = row.Cells["codigo"].Value.ToString(),
                Nome = row.Cells["nome"].Value.ToString(),
                UnidadeId = Convert.ToInt32(row.Cells["unidade_id"].Value),
                UnidadeNome = row.Cells["unidade"].Value.ToString(),
                Preco = Convert.ToDecimal(row.Cells["preco"].Value),
                Quantidade = Convert.ToInt32(row.Cells["quantidade"].Value)
            };

            using (var form = new ProdutoForm(produto))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    _ = CarregarProdutosAsync();
            }
        }

        private async void btnExcluir_Click(object sender, EventArgs e)
        {
            if (dgvProdutos.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvProdutos.CurrentRow.Cells["id"].Value);
            string nome = dgvProdutos.CurrentRow.Cells["nome"].Value.ToString();

            var resp = MessageBox.Show($"Excluir '{nome}'?", "Confirmar", MessageBoxButtons.YesNo);
            if (resp != DialogResult.Yes) return;

            await Task.Run(() =>
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("DELETE FROM produto WHERE id=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            });

            await CarregarProdutosAsync();
        }

        private async void btnAtualizar_Click(object sender, EventArgs e)
        {
            await CarregarProdutosAsync();
        }

        private void LogError(Exception ex)
        {
            File.AppendAllText("error.log", $"{DateTime.Now} - {ex}\n");
        }
    }
}
