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

            btnLogin.Click += BtnLogin_Click;
            txtUsername.TextChanged += TxtUsername_TextChanged;
            txtPassword.TextChanged += TxtPassword_TextChanged;
            txtServerIP.TextChanged += TxtServerIP_TextChanged;

            AcceptButton = btnLogin;
            CancelButton = btnCancel;
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

            if (txtUsername.Text == "admin" && txtPassword.Text == "password")
            {
                AuthenticatedUser = new User
                {
                    Id = 1,
                    Username = "admin",
                    FullName = "Соколов Андрей",
                    DepartmentId = 1,
                    DepartmentName = "IT",
                    Position = "Начальник IT отдела"
                };
                DialogResult = DialogResult.OK;
                Close();
            }
            else if (txtUsername.Text == "ivanov" && txtPassword.Text == "password")
            {
                AuthenticatedUser = new User
                {
                    Id = 2,
                    Username = "ivanov",
                    FullName = "Иванов Иван",
                    DepartmentId = 2,
                    DepartmentName = "Production",
                    Position = "Начальник производства"
                };
                DialogResult = DialogResult.OK;
                Close();
            }
            else if (txtUsername.Text == "petrov" && txtPassword.Text == "password")
            {
                AuthenticatedUser = new User
                {
                    Id = 3,
                    Username = "petrov",
                    FullName = "Петров Петр",
                    DepartmentId = 2,
                    DepartmentName = "Production",
                    Position = "Мастер цеха"
                };
                DialogResult = DialogResult.OK;
                Close();
            }
            else if (txtUsername.Text == "volkov" && txtPassword.Text == "password")
            {
                AuthenticatedUser = new User
                {
                    Id = 4,
                    Username = "volkov",
                    FullName = "Волков Владимир",
                    DepartmentId = 3,
                    DepartmentName = "Supply",
                    Position = "Начальник снабжения"
                };
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                lblError.Text = "Неверный логин или пароль";
            }
        }
    }
}
