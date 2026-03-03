using Messenger.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForms
{
    public partial class LoginForm : Form
    {
        public User AuthenticatedUser { get; private set; }
        public string ServerIP { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
            LoadLogo();
            SubscribeEvents();

            AcceptButton = btnLogin;
            CancelButton = btnCancel;
        }

        private void SubscribeEvents()
        {
            btnLogin.Click += BtnLogin_Click;
            txtUsername.TextChanged += TxtUsername_TextChanged;
            txtPassword.TextChanged += TxtPassword_TextChanged;
            txtServerIP.TextChanged += TxtServerIP_TextChanged;
        }

        private void LoadLogo()
        {
            try
            {
                string[] possiblePaths = {
                    "logo.png",
                    "Resources\\logo.png",
                    "..\\..\\Resources\\logo.png"
                };

                foreach (string path in possiblePaths)
                {
                    if (System.IO.File.Exists(path))
                    {
                        picLogo.Image = Image.FromFile(path);
                        return;
                    }
                }
                CreateDefaultLogo();
            }
            catch
            {
                CreateDefaultLogo();
            }
        }

        private void CreateDefaultLogo()
        {
            Bitmap bmp = new Bitmap(80, 80);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.FromArgb(0, 51, 102));
                using (Font font = new Font("Segoe UI", 30, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    g.DrawString("М", font, brush, new Rectangle(0, 0, 80, 80), sf);
                }
            }
            picLogo.Image = bmp;
        }

        private void TxtUsername_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
                txtUsername.ForeColor = Color.Black;
        }

        private void TxtPassword_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                txtPassword.ForeColor = Color.Black;
        }

        private void TxtServerIP_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtServerIP.Text))
                txtServerIP.ForeColor = Color.Black;
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServerIP.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "Заполните все поля";
                return;
            }

            ServerIP = txtServerIP.Text;

            // Здесь будет реальная аутентификация через сервер
            // Временная заглушка для демонстрации
            AuthenticatedUser = new User
            {
                Id = 1,
                Username = txtUsername.Text,
                FullName = "Пользователь",
                DepartmentId = 1,
                DepartmentName = "Отдел",
                Position = "Должность"
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
