using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using CRUDProdutos.Models;

namespace CRUDProdutos
{
    public partial class CadastroUsuarioForm : Form
    {
        public CadastroUsuarioForm()
        {
            InitializeComponent();
        }

        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string senha = txtSenha.Text.Trim();
            string tipo = cmbTipo.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(tipo))
            {
                MessageBox.Show("Preencha todos os campos.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool sucesso = await Task.Run(() =>
                {
                    using (var conn = Database.GetConnection())
                    {
                        conn.Open();
                        var cmd = new MySqlCommand(
                            "INSERT INTO usuario (login, senha, tipo) VALUES (@login, @senha, @tipo)",
                            conn
                        );
                        cmd.Parameters.AddWithValue("@login", login);
                        cmd.Parameters.AddWithValue("@senha", senha);
                        cmd.Parameters.AddWithValue("@tipo", tipo);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                });

                if (sucesso)
                {
                    MessageBox.Show("Usuário criado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062) // Duplicate entry
            {
                MessageBox.Show("Login já existe!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao criar usuário: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
