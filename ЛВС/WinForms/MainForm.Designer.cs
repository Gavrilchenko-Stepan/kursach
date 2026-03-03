namespace WinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        // Верхняя панель
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.PictureBox picUserAvatar;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblUserDepartment;
        private System.Windows.Forms.Label lblUserStatus;
        private System.Windows.Forms.Button btnNewChat;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnLogout;

        // Левая панель (чаты)
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Label lblChatsHeader;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearchIcon;
        private System.Windows.Forms.ListBox lstChats;

        // Правая панель (чат)
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelChatHeader;
        private System.Windows.Forms.PictureBox picChatAvatar;
        private System.Windows.Forms.Label lblChatName;
        private System.Windows.Forms.Label lblChatInfo;
        private System.Windows.Forms.FlowLayoutPanel flowOnlineStatus;
        private System.Windows.Forms.ListBox lstMessages;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;

        // Нижняя панель статуса
        private System.Windows.Forms.Label lblStatusBar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                // Освобождение ресурсов изображений
                if (picUserAvatar.Image != null)
                    picUserAvatar.Image.Dispose();
                if (picChatAvatar.Image != null)
                    picChatAvatar.Image.Dispose();

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.picUserAvatar = new System.Windows.Forms.PictureBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblUserDepartment = new System.Windows.Forms.Label();
            this.lblUserStatus = new System.Windows.Forms.Label();
            this.btnNewChat = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.lblChatsHeader = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearchIcon = new System.Windows.Forms.Label();
            this.lstChats = new System.Windows.Forms.ListBox();
            this.panelRight = new System.Windows.Forms.Panel();
            this.panelChatHeader = new System.Windows.Forms.Panel();
            this.picChatAvatar = new System.Windows.Forms.PictureBox();
            this.lblChatName = new System.Windows.Forms.Label();
            this.lblChatInfo = new System.Windows.Forms.Label();
            this.flowOnlineStatus = new System.Windows.Forms.FlowLayoutPanel();
            this.lstMessages = new System.Windows.Forms.ListBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblStatusBar = new System.Windows.Forms.Label();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUserAvatar)).BeginInit();
            this.panelLeft.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.panelChatHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picChatAvatar)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(102)))));
            this.panelTop.Controls.Add(this.picUserAvatar);
            this.panelTop.Controls.Add(this.lblUserName);
            this.panelTop.Controls.Add(this.lblUserDepartment);
            this.panelTop.Controls.Add(this.lblUserStatus);
            this.panelTop.Controls.Add(this.btnNewChat);
            this.panelTop.Controls.Add(this.btnSettings);
            this.panelTop.Controls.Add(this.btnLogout);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1300, 80);
            this.panelTop.TabIndex = 2;
            // 
            // picUserAvatar
            // 
            this.picUserAvatar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.picUserAvatar.Location = new System.Drawing.Point(20, 15);
            this.picUserAvatar.Name = "picUserAvatar";
            this.picUserAvatar.Size = new System.Drawing.Size(50, 50);
            this.picUserAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picUserAvatar.TabIndex = 0;
            this.picUserAvatar.TabStop = false;
            // 
            // lblUserName
            // 
            this.lblUserName.BackColor = System.Drawing.Color.Transparent;
            this.lblUserName.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblUserName.ForeColor = System.Drawing.Color.White;
            this.lblUserName.Location = new System.Drawing.Point(80, 15);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(300, 25);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.Text = "Загрузка...";
            // 
            // lblUserDepartment
            // 
            this.lblUserDepartment.BackColor = System.Drawing.Color.Transparent;
            this.lblUserDepartment.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblUserDepartment.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(220)))), ((int)(((byte)(250)))));
            this.lblUserDepartment.Location = new System.Drawing.Point(80, 40);
            this.lblUserDepartment.Name = "lblUserDepartment";
            this.lblUserDepartment.Size = new System.Drawing.Size(300, 20);
            this.lblUserDepartment.TabIndex = 2;
            this.lblUserDepartment.Text = "Отдел";
            // 
            // lblUserStatus
            // 
            this.lblUserStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblUserStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblUserStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(158)))), ((int)(((byte)(158)))), ((int)(((byte)(158)))));
            this.lblUserStatus.Location = new System.Drawing.Point(80, 58);
            this.lblUserStatus.Name = "lblUserStatus";
            this.lblUserStatus.Size = new System.Drawing.Size(100, 15);
            this.lblUserStatus.TabIndex = 3;
            this.lblUserStatus.Text = "● Не в сети";
            // 
            // btnNewChat
            // 
            this.btnNewChat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.btnNewChat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewChat.FlatAppearance.BorderSize = 0;
            this.btnNewChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewChat.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNewChat.ForeColor = System.Drawing.Color.White;
            this.btnNewChat.Location = new System.Drawing.Point(1045, 22);
            this.btnNewChat.Name = "btnNewChat";
            this.btnNewChat.Size = new System.Drawing.Size(140, 35);
            this.btnNewChat.TabIndex = 4;
            this.btnNewChat.Text = "➕ Новый чат";
            this.btnNewChat.UseVisualStyleBackColor = false;
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(80)))), ((int)(((byte)(110)))));
            this.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.btnSettings.ForeColor = System.Drawing.Color.White;
            this.btnSettings.Location = new System.Drawing.Point(1191, 22);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(45, 35);
            this.btnSettings.TabIndex = 5;
            this.btnSettings.Text = "⚙";
            this.btnSettings.UseVisualStyleBackColor = false;
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(1245, 22);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(45, 35);
            this.btnLogout.TabIndex = 6;
            this.btnLogout.Text = "🚪";
            this.btnLogout.UseVisualStyleBackColor = false;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.White;
            this.panelLeft.Controls.Add(this.lblChatsHeader);
            this.panelLeft.Controls.Add(this.txtSearch);
            this.panelLeft.Controls.Add(this.lblSearchIcon);
            this.panelLeft.Controls.Add(this.lstChats);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 80);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Padding = new System.Windows.Forms.Padding(10);
            this.panelLeft.Size = new System.Drawing.Size(350, 695);
            this.panelLeft.TabIndex = 1;
            // 
            // lblChatsHeader
            // 
            this.lblChatsHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblChatsHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(102)))));
            this.lblChatsHeader.Location = new System.Drawing.Point(15, 15);
            this.lblChatsHeader.Name = "lblChatsHeader";
            this.lblChatsHeader.Size = new System.Drawing.Size(100, 25);
            this.lblChatsHeader.TabIndex = 0;
            this.lblChatsHeader.Text = "ЧАТЫ";
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.ForeColor = System.Drawing.Color.Gray;
            this.txtSearch.Location = new System.Drawing.Point(15, 45);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(290, 25);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.Text = "Поиск чатов...";
            // 
            // lblSearchIcon
            // 
            this.lblSearchIcon.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblSearchIcon.ForeColor = System.Drawing.Color.Gray;
            this.lblSearchIcon.Location = new System.Drawing.Point(310, 45);
            this.lblSearchIcon.Name = "lblSearchIcon";
            this.lblSearchIcon.Size = new System.Drawing.Size(25, 27);
            this.lblSearchIcon.TabIndex = 2;
            this.lblSearchIcon.Text = "🔍";
            this.lblSearchIcon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lstChats
            // 
            this.lstChats.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstChats.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lstChats.IntegralHeight = false;
            this.lstChats.ItemHeight = 17;
            this.lstChats.Location = new System.Drawing.Point(15, 80);
            this.lstChats.Name = "lstChats";
            this.lstChats.Size = new System.Drawing.Size(320, 600);
            this.lstChats.TabIndex = 3;
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.panelRight.Controls.Add(this.panelChatHeader);
            this.panelRight.Controls.Add(this.lstMessages);
            this.panelRight.Controls.Add(this.panelBottom);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(350, 80);
            this.panelRight.Name = "panelRight";
            this.panelRight.Padding = new System.Windows.Forms.Padding(10);
            this.panelRight.Size = new System.Drawing.Size(950, 695);
            this.panelRight.TabIndex = 0;
            // 
            // panelChatHeader
            // 
            this.panelChatHeader.BackColor = System.Drawing.Color.White;
            this.panelChatHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelChatHeader.Controls.Add(this.picChatAvatar);
            this.panelChatHeader.Controls.Add(this.lblChatName);
            this.panelChatHeader.Controls.Add(this.lblChatInfo);
            this.panelChatHeader.Controls.Add(this.flowOnlineStatus);
            this.panelChatHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelChatHeader.Location = new System.Drawing.Point(10, 10);
            this.panelChatHeader.Name = "panelChatHeader";
            this.panelChatHeader.Size = new System.Drawing.Size(930, 70);
            this.panelChatHeader.TabIndex = 0;
            // 
            // picChatAvatar
            // 
            this.picChatAvatar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.picChatAvatar.Location = new System.Drawing.Point(15, 10);
            this.picChatAvatar.Name = "picChatAvatar";
            this.picChatAvatar.Size = new System.Drawing.Size(50, 50);
            this.picChatAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picChatAvatar.TabIndex = 0;
            this.picChatAvatar.TabStop = false;
            // 
            // lblChatName
            // 
            this.lblChatName.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblChatName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(102)))));
            this.lblChatName.Location = new System.Drawing.Point(75, 15);
            this.lblChatName.Name = "lblChatName";
            this.lblChatName.Size = new System.Drawing.Size(300, 25);
            this.lblChatName.TabIndex = 1;
            this.lblChatName.Text = "Выберите чат";
            // 
            // lblChatInfo
            // 
            this.lblChatInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblChatInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblChatInfo.Location = new System.Drawing.Point(75, 40);
            this.lblChatInfo.Name = "lblChatInfo";
            this.lblChatInfo.Size = new System.Drawing.Size(300, 20);
            this.lblChatInfo.TabIndex = 2;
            // 
            // flowOnlineStatus
            // 
            this.flowOnlineStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowOnlineStatus.Location = new System.Drawing.Point(700, 15);
            this.flowOnlineStatus.Name = "flowOnlineStatus";
            this.flowOnlineStatus.Size = new System.Drawing.Size(200, 40);
            this.flowOnlineStatus.TabIndex = 3;
            // 
            // lstMessages
            // 
            this.lstMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstMessages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.lstMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstMessages.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lstMessages.IntegralHeight = false;
            this.lstMessages.ItemHeight = 17;
            this.lstMessages.Location = new System.Drawing.Point(10, 90);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.Size = new System.Drawing.Size(930, 530);
            this.lstMessages.TabIndex = 1;
            // 
            // panelBottom
            // 
            this.panelBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBottom.BackColor = System.Drawing.Color.White;
            this.panelBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelBottom.Controls.Add(this.txtMessage);
            this.panelBottom.Controls.Add(this.btnSend);
            this.panelBottom.Location = new System.Drawing.Point(10, 630);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(930, 70);
            this.panelBottom.TabIndex = 2;
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMessage.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtMessage.Location = new System.Drawing.Point(10, 15);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(790, 40);
            this.txtMessage.TabIndex = 0;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSend.Enabled = false;
            this.btnSend.FlatAppearance.BorderSize = 0;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Location = new System.Drawing.Point(810, 10);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(110, 45);
            this.btnSend.TabIndex = 1;
            this.btnSend.Text = "Отправить";
            this.btnSend.UseVisualStyleBackColor = false;
            // 
            // lblStatusBar
            // 
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(58)))), ((int)(((byte)(64)))));
            this.lblStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatusBar.ForeColor = System.Drawing.Color.White;
            this.lblStatusBar.Location = new System.Drawing.Point(0, 775);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new System.Windows.Forms.Padding(10, 3, 0, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(1300, 25);
            this.lblStatusBar.TabIndex = 3;
            this.lblStatusBar.Text = "Статус: Не подключен";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(1300, 800);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.lblStatusBar);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Локальный корпоративный мессенджер - Завод \'МАРС\'";
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picUserAvatar)).EndInit();
            this.panelLeft.ResumeLayout(false);
            this.panelLeft.PerformLayout();
            this.panelRight.ResumeLayout(false);
            this.panelChatHeader.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picChatAvatar)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}

