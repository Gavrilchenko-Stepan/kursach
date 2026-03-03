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
    public partial class NewChatForm : Form
    {
        public int SelectedDepartmentId { get; private set; }

        private List<DepartmentItem> departments = new List<DepartmentItem>();
        private int currentDepartmentId;

        public NewChatForm(int currentDeptId)
        {
            InitializeComponent();

            currentDepartmentId = currentDeptId;

            lstDepartments.DrawMode = DrawMode.OwnerDrawFixed;
            lstDepartments.DrawItem += LstDepartments_DrawItem;
            lstDepartments.SelectedIndexChanged += LstDepartments_SelectedIndexChanged;

            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtSearch.Enter += TxtSearch_Enter;
            txtSearch.Leave += TxtSearch_Leave;

            btnCreate.Click += BtnCreate_Click;

            LoadDepartments();
        }

        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Поиск отдела...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void TxtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Поиск отдела...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private void LoadDepartments()
        {
            departments = new List<DepartmentItem>
            {
                new DepartmentItem { Id = 1, Name = "IT", Description = "Информационные технологии", OnlineCount = 3 },
                new DepartmentItem { Id = 2, Name = "Production", Description = "Производственный отдел", OnlineCount = 5 },
                new DepartmentItem { Id = 3, Name = "Supply", Description = "Отдел снабжения", OnlineCount = 2 },
                new DepartmentItem { Id = 4, Name = "Security", Description = "Служба безопасности", OnlineCount = 1 },
                new DepartmentItem { Id = 5, Name = "HR", Description = "Отдел кадров", OnlineCount = 2 },
                new DepartmentItem { Id = 6, Name = "Engineering", Description = "Инженерный отдел", OnlineCount = 4 }
            };

            UpdateDepartmentsList(departments);
        }

        private void UpdateDepartmentsList(List<DepartmentItem> depts)
        {
            lstDepartments.Items.Clear();
            foreach (var dept in depts)
            {
                if (dept.Id != currentDepartmentId)
                {
                    lstDepartments.Items.Add(dept);
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(search) || search == "поиск отдела...")
            {
                UpdateDepartmentsList(departments);
                return;
            }

            var filtered = departments.FindAll(d =>
                d.Name.ToLower().Contains(search) ||
                d.Description.ToLower().Contains(search));

            UpdateDepartmentsList(filtered);
        }

        private void LstDepartments_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || lstDepartments.Items[e.Index] == null) return;

            var dept = lstDepartments.Items[e.Index] as DepartmentItem;
            e.DrawBackground();

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color backColor = selected ? Color.FromArgb(230, 242, 255) : Color.White;

            using (var backBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }

            using (var nameFont = new Font("Segoe UI", 11, FontStyle.Bold))
            {
                e.Graphics.DrawString(dept.Name, nameFont, Brushes.Black,
                    e.Bounds.X + 10, e.Bounds.Y + 5);
            }

            string info = $"{dept.Description} • {dept.OnlineCount} онлайн";
            using (var infoFont = new Font("Segoe UI", 9))
            {
                e.Graphics.DrawString(info, infoFont, Brushes.Gray,
                    e.Bounds.X + 10, e.Bounds.Y + 28);
            }

            e.DrawFocusRectangle();
        }

        private void LstDepartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnCreate.Enabled = lstDepartments.SelectedItem != null;
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            var selectedDept = lstDepartments.SelectedItem as DepartmentItem;
            if (selectedDept != null)
            {
                SelectedDepartmentId = selectedDept.Id;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private class DepartmentItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int OnlineCount { get; set; }
        }
    }
}
