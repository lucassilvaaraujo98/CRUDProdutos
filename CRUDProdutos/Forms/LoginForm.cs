using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using CRUDProdutos.Models;

namespace CRUDProdutos
{
    public partial class LoginForm : Form
    {
        public Usuario UsuarioLogado { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string senha = txtSenha.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Informe login e senha.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Database.GetConnection())
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM usuario WHERE login=@login AND senha=@senha", conn);
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@senha", senha);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UsuarioLogado = new Usuario
                            {
                                Id = reader.GetInt32("id"),
                                Login = reader.GetString("login"),
                                Senha = reader.GetString("senha"),
                                Tipo = reader.GetString("tipo")
                            };

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Login ou senha incorretos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao tentar login: " + ex.Message);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnCriarUsuario_Click(object sender, EventArgs e)
        {
            using (var form = new CadastroUsuarioForm())
            {
                form.ShowDialog();
            }
        }

        private void txtLogin_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
