using Messenger.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForms
{
    public partial class MainForm : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private BinaryFormatter formatter;
        private Thread receiveThread;
        private bool isConnected = false;
        private User currentUser;
        private List<Chat> chats = new List<Chat>();
        private Chat currentChat;
        private Dictionary<int, List<Messenger.Shared.Models.Message>> messagesCache = new Dictionary<int, List<Messenger.Shared.Models.Message>>();
        private System.Windows.Forms.Timer refreshTimer;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip trayMenu;
        private Dictionary<string, Image> avatarCache = new Dictionary<string, Image>();

        public MainForm()
        {
            InitializeComponent();
            CustomizeUI();
            ShowLoginForm();
        }

        private void CustomizeUI()
        {
            // Настройка списка чатов
            lstChats.DrawMode = DrawMode.OwnerDrawFixed;
            lstChats.DrawItem += LstChats_DrawItem;
            lstChats.ItemHeight = 70;

            // Настройка списка сообщений
            lstMessages.DrawMode = DrawMode.OwnerDrawFixed;
            lstMessages.DrawItem += LstMessages_DrawItem;
            lstMessages.ItemHeight = 70;

            // Настройка аватаров
            SetupAvatarPictureBox(picUserAvatar);
            SetupAvatarPictureBox(picChatAvatar);

            // Подписка на события
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
            this.Resize += MainForm_Resize;

            txtSearch.Enter += TxtSearch_Enter;
            txtSearch.Leave += TxtSearch_Leave;
            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtMessage.KeyDown += TxtMessage_KeyDown;

            lstChats.SelectedIndexChanged += LstChats_SelectedIndexChanged;

            btnNewChat.Click += BtnNewChat_Click;
            btnSend.Click += BtnSend_Click;
            btnLogout.Click += BtnLogout_Click;
            btnSettings.Click += BtnSettings_Click;
        }

        private void SetupAvatarPictureBox(PictureBox pb)
        {
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Paint += (s, e) => {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var path = new GraphicsPath();
                path.AddEllipse(0, 0, pb.Width - 1, pb.Height - 1);
                pb.Region = new Region(path);
            };
        }

        private Image CreateInitialsAvatar(string fullName, Color backColor)
        {
            Bitmap bmp = new Bitmap(50, 50);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(backColor);
                using (Font font = new Font("Segoe UI", 20, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.White))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    string initials = "?";
                    if (!string.IsNullOrEmpty(fullName))
                    {
                        var parts = fullName.Split(' ');
                        if (parts.Length >= 2)
                            initials = parts[0][0].ToString() + parts[1][0].ToString();
                        else if (parts.Length == 1 && parts[0].Length > 0)
                            initials = parts[0][0].ToString();
                    }

                    g.DrawString(initials.ToUpper(), font, brush, new Rectangle(0, 0, 50, 50), sf);
                }
            }
            return bmp;
        }

        private Image LoadAvatar(string key, string fullName, Color backColor)
        {
            if (avatarCache.ContainsKey(key))
                return avatarCache[key];

            string[] possiblePaths = {
                $"Avatars/{key}.png",
                $"Resources/Avatars/{key}.png",
                $"Avatars/default.png"
            };

            foreach (string path in possiblePaths)
            {
                if (System.IO.File.Exists(path))
                {
                    try
                    {
                        var img = Image.FromFile(path);
                        avatarCache[key] = img;
                        return img;
                    }
                    catch { }
                }
            }

            var avatar = CreateInitialsAvatar(fullName, backColor);
            avatarCache[key] = avatar;
            return avatar;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateUIForDisconnected();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Адаптация размеров при изменении окна
            lstMessages.Width = panelRight.Width - 20;
            lstMessages.Height = panelRight.Height - 160;
            panelBottom.Width = panelRight.Width - 20;
            panelBottom.Location = new Point(10, panelRight.Height - 90);
            txtMessage.Width = panelBottom.Width - 140;
            btnSend.Location = new Point(panelBottom.Width - 120, 10);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();

            foreach (var img in avatarCache.Values)
                img?.Dispose();
            avatarCache.Clear();
        }

        private void ShowLoginForm()
        {
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    currentUser = loginForm.AuthenticatedUser;
                    ConnectToServer(loginForm.ServerIP);

                    lblUserName.Text = currentUser.FullName;
                    lblUserDepartment.Text = currentUser.DepartmentName;
                    picUserAvatar.Image = LoadAvatar(
                        $"user_{currentUser.Id}",
                        currentUser.FullName,
                        Color.FromArgb(65, 105, 225)
                    );
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        private void ConnectToServer(string serverIP)
        {
            try
            {
                client = new TcpClient();
                client.Connect(serverIP, 8888);
                stream = client.GetStream();
                formatter = new BinaryFormatter();

                var credentials = new Dictionary<string, string>
                {
                    { "username", currentUser.Username },
                    { "password", "password" }
                };

                var packet = new NetworkPacket
                {
                    Command = NetworkCommand.Login,
                    SenderId = currentUser.Id,
                    Data = credentials
                };

                formatter.Serialize(stream, packet);

                receiveThread = new Thread(ReceiveMessages);
                receiveThread.IsBackground = true;
                receiveThread.Start();

                isConnected = true;

                this.Invoke(new Action(() => {
                    lblStatusBar.Text = $"Статус: Онлайн • {currentUser.DepartmentName}";
                    lblStatusBar.BackColor = Color.FromArgb(40, 167, 69);
                    lblUserStatus.Text = "● Онлайн";
                    lblUserStatus.ForeColor = Color.FromArgb(76, 175, 80);
                }));

                refreshTimer = new System.Windows.Forms.Timer { Interval = 30000 };
                refreshTimer.Tick += (s, e) => RefreshChats();
                refreshTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к серверу: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ShowLoginForm();
            }
        }

        private void Disconnect()
        {
            isConnected = false;

            if (client?.Connected == true)
            {
                try
                {
                    var packet = new NetworkPacket { Command = NetworkCommand.Logout };
                    formatter.Serialize(stream, packet);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка при выходе: {ex.Message}");
                }
            }

            refreshTimer?.Stop();
            receiveThread?.Abort();
            stream?.Close();
            client?.Close();

            this.Invoke(new Action(UpdateUIForDisconnected));
        }

        private void UpdateUIForDisconnected()
        {
            lblStatusBar.Text = "Статус: Не подключен";
            lblStatusBar.BackColor = Color.FromArgb(108, 117, 125);
            lblUserStatus.Text = "● Не в сети";
            lblUserStatus.ForeColor = Color.FromArgb(158, 158, 158);
            lstChats.Items.Clear();
            lstMessages.Items.Clear();
            btnSend.Enabled = false;
            lblChatName.Text = "Выберите чат";
            lblChatInfo.Text = "";
            picChatAvatar.Image = null;
        }

        private void ReceiveMessages()
        {
            try
            {
                while (isConnected && client.Connected)
                {
                    if (stream.DataAvailable)
                    {
                        var packet = (NetworkPacket)formatter.Deserialize(stream);
                        ProcessPacket(packet);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                if (isConnected)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка соединения: {ex.Message}");
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show($"Потеряно соединение с сервером: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Disconnect();
                        ShowLoginForm();
                    }));
                }
            }
        }

        private void ProcessPacket(NetworkPacket packet)
        {
            switch (packet.Command)
            {
                case NetworkCommand.Login:
                    HandleLogin(packet);
                    break;
                case NetworkCommand.GetChats:
                    HandleGetChats(packet);
                    break;
                case NetworkCommand.GetChatMessages:
                    HandleGetChatMessages(packet);
                    break;
                case NetworkCommand.NewMessage:
                    HandleNewMessage(packet);
                    break;
                case NetworkCommand.UserStatusChanged:
                    HandleUserStatusChanged(packet);
                    break;
                case NetworkCommand.CreateExternalChat:
                    HandleCreateExternalChat(packet);
                    break;
            }
        }

        private void HandleLogin(NetworkPacket packet)
        {
            var result = packet.Data as Dictionary<string, object>;
            if (result == null) return;

            bool success = (bool)result["success"];

            if (success)
            {
                this.Invoke(new Action(RequestChats));
            }
            else
            {
                string message = result["message"].ToString();
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show(message, "Ошибка входа",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Disconnect();
                    ShowLoginForm();
                }));
            }
        }

        private void RequestChats()
        {
            if (!isConnected) return;

            var packet = new NetworkPacket
            {
                Command = NetworkCommand.GetChats,
                SenderId = currentUser.Id
            };
            formatter.Serialize(stream, packet);
        }

        private void RefreshChats()
        {
            if (isConnected)
                RequestChats();
        }

        private void HandleGetChats(NetworkPacket packet)
        {
            var newChats = packet.Data as List<Chat>;
            if (newChats == null) return;

            this.Invoke(new Action(() =>
            {
                chats = newChats;
                UpdateChatsList();
            }));
        }

        private void UpdateChatsList()
        {
            lstChats.Items.Clear();

            var sorted = chats.OrderByDescending(c => c.Type == ChatType.Internal)
                              .ThenByDescending(c => c.LastMessageTime);

            foreach (var chat in sorted)
                lstChats.Items.Add(chat);
        }

        private void HandleGetChatMessages(NetworkPacket packet)
        {
            var messages = packet.Data as List<Messenger.Shared.Models.Message>;
            if (messages == null || currentChat == null) return;

            this.Invoke(new Action(() =>
            {
                messagesCache[currentChat.Id] = messages;
                DisplayMessages(currentChat.Id);

                if (messages.Count > 0)
                    MarkMessagesAsRead(currentChat.Id, messages.Last().Id);
            }));
        }

        private void HandleNewMessage(NetworkPacket packet)
        {
            var message = packet.Data as Messenger.Shared.Models.Message;
            if (message == null) return;

            this.Invoke(new Action(() =>
            {
                if (!messagesCache.ContainsKey(message.ChatId))
                    messagesCache[message.ChatId] = new List<Messenger.Shared.Models.Message>();

                messagesCache[message.ChatId].Add(message);

                var chat = chats.FirstOrDefault(c => c.Id == message.ChatId);
                if (chat != null)
                {
                    chat.LastMessage = message.Text;
                    chat.LastMessageTime = message.SentAt;

                    if (currentChat?.Id != message.ChatId)
                        chat.UnreadCount++;

                    UpdateChatsList();
                }

                if (currentChat?.Id == message.ChatId)
                {
                    DisplayMessages(message.ChatId);
                    MarkMessagesAsRead(message.ChatId, message.Id);
                    lstMessages.TopIndex = lstMessages.Items.Count - 1;
                    System.Media.SystemSounds.Asterisk.Play();
                }
                else
                {
                    ShowNotification(message);
                    System.Media.SystemSounds.Beep.Play();
                }
            }));
        }

        private void HandleUserStatusChanged(NetworkPacket packet)
        {
            var user = packet.Data as User;
            if (user == null) return;

            this.Invoke(new Action(() =>
            {
                foreach (var chat in chats)
                {
                    var participant = chat.Participants?.FirstOrDefault(p => p.Id == user.Id);
                    if (participant != null)
                        participant.IsOnline = user.IsOnline;
                }

                if (currentChat?.Type == ChatType.Internal)
                {
                    int onlineCount = currentChat.Participants?.Count(p => p.IsOnline) ?? 0;
                    int totalCount = currentChat.Participants?.Count ?? 0;
                    lblChatInfo.Text = $"{onlineCount} онлайн • {totalCount} сотрудников";
                }

                lstChats.Invalidate();
            }));
        }

        private void HandleCreateExternalChat(NetworkPacket packet)
        {
            var newChat = packet.Data as Chat;
            if (newChat == null) return;

            this.Invoke(new Action(() =>
            {
                chats.Add(newChat);
                UpdateChatsList();

                currentChat = newChat;
                lstChats.SelectedItem = newChat;
                LoadChatMessages(newChat.Id);
            }));
        }

        private void LstChats_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(lstChats.SelectedItem is Chat selectedChat)) return;

            currentChat = selectedChat;
            lblChatName.Text = selectedChat.Name;

            if (selectedChat.Type == ChatType.Internal)
            {
                picChatAvatar.BackColor = Color.FromArgb(0, 150, 136);
                picChatAvatar.Image = LoadAvatar(
                    $"dept_{selectedChat.DepartmentId}",
                    selectedChat.Name.Replace("🔒 ", ""),
                    Color.FromArgb(0, 150, 136)
                );

                int onlineCount = selectedChat.Participants?.Count(p => p.IsOnline) ?? 0;
                int totalCount = selectedChat.Participants?.Count ?? 0;
                lblChatInfo.Text = $"{onlineCount} онлайн • {totalCount} сотрудников";
            }
            else
            {
                picChatAvatar.BackColor = Color.FromArgb(0, 123, 255);
                picChatAvatar.Image = LoadAvatar(
                    $"dept_{selectedChat.OtherDepartmentId}",
                    selectedChat.Name,
                    Color.FromArgb(0, 123, 255)
                );
                lblChatInfo.Text = $"Чат с отделом {selectedChat.Name}";
            }

            btnSend.Enabled = true;
            selectedChat.UnreadCount = 0;
            UpdateChatsList();
            LoadChatMessages(selectedChat.Id);
        }

        private void LoadChatMessages(int chatId)
        {
            if (messagesCache.ContainsKey(chatId))
            {
                DisplayMessages(chatId);
            }
            else
            {
                var packet = new NetworkPacket
                {
                    Command = NetworkCommand.GetChatMessages,
                    SenderId = currentUser.Id,
                    Data = chatId
                };
                formatter.Serialize(stream, packet);
            }
        }

        private void DisplayMessages(int chatId)
        {
            if (!messagesCache.ContainsKey(chatId)) return;

            lstMessages.Items.Clear();
            foreach (var msg in messagesCache[chatId])
                lstMessages.Items.Add(msg);

            if (lstMessages.Items.Count > 0)
                lstMessages.TopIndex = lstMessages.Items.Count - 1;
        }

        private void MarkMessagesAsRead(int chatId, int lastMessageId)
        {
            var data = new Dictionary<string, int>
            {
                { "chatId", chatId },
                { "lastReadMessageId", lastMessageId }
            };

            var packet = new NetworkPacket
            {
                Command = NetworkCommand.MessagesRead,
                SenderId = currentUser.Id,
                Data = data
            };

            try
            {
                formatter.Serialize(stream, packet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка отметки прочитанных: {ex.Message}");
            }
        }

        private void ShowNotification(Messenger.Shared.Models.Message message)
        {
            if (notifyIcon == null)
            {
                notifyIcon = new NotifyIcon
                {
                    Icon = SystemIcons.Information,
                    Visible = true,
                    Text = "Новое сообщение"
                };

                trayMenu = new ContextMenuStrip();
                trayMenu.Items.Add("Открыть мессенджер", null, (s, e) => {
                    this.WindowState = FormWindowState.Normal;
                    this.Activate();
                });
                trayMenu.Items.Add("Выход", null, (s, e) => Application.Exit());
                notifyIcon.ContextMenuStrip = trayMenu;

                notifyIcon.DoubleClick += (s, e) => {
                    this.WindowState = FormWindowState.Normal;
                    this.Activate();
                };
            }

            notifyIcon.ShowBalloonTip(3000, "Новое сообщение",
                $"{message.SenderName}: {message.Text}", ToolTipIcon.Info);
        }

        private void LstChats_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || !(lstChats.Items[e.Index] is Chat chat)) return;

            e.DrawBackground();

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color backColor = selected ? Color.FromArgb(230, 242, 255) : Color.White;

            using (var backBrush = new SolidBrush(backColor))
                e.Graphics.FillRectangle(backBrush, e.Bounds);

            string icon = chat.Type == ChatType.Internal ? "🔒" : "📢";
            using (var iconFont = new Font("Segoe UI", 14))
                e.Graphics.DrawString(icon, iconFont, Brushes.Gray, e.Bounds.X + 10, e.Bounds.Y + 15);

            int x = e.Bounds.X + 45;
            using (var nameFont = new Font("Segoe UI", 11, FontStyle.Bold))
                e.Graphics.DrawString(chat.Name, nameFont, Brushes.Black, x, e.Bounds.Y + 10);

            if (!string.IsNullOrEmpty(chat.LastMessage))
            {
                string lastMsg = chat.LastMessage.Length > 35
                    ? chat.LastMessage.Substring(0, 32) + "..."
                    : chat.LastMessage;

                using (var msgFont = new Font("Segoe UI", 9))
                    e.Graphics.DrawString(lastMsg, msgFont, Brushes.Gray, x, e.Bounds.Y + 35);
            }

            if (chat.LastMessageTime > DateTime.MinValue)
            {
                string time = chat.LastMessageTime.ToString("HH:mm");
                using (var timeFont = new Font("Segoe UI", 8))
                {
                    var timeSize = e.Graphics.MeasureString(time, timeFont);
                    e.Graphics.DrawString(time, timeFont, Brushes.Gray,
                        e.Bounds.Right - timeSize.Width - 20, e.Bounds.Y + 10);
                }
            }

            if (chat.UnreadCount > 0)
            {
                string count = chat.UnreadCount.ToString();
                using (var countFont = new Font("Segoe UI", 8, FontStyle.Bold))
                {
                    var countSize = e.Graphics.MeasureString(count, countFont);
                    int badgeSize = Math.Max(20, (int)countSize.Width + 10);

                    var badgeRect = new Rectangle(
                        e.Bounds.Right - badgeSize - 15,
                        e.Bounds.Y + 35,
                        badgeSize, 18);

                    using (var badgeBrush = new SolidBrush(Color.FromArgb(220, 53, 69)))
                        e.Graphics.FillEllipse(badgeBrush, badgeRect);

                    e.Graphics.DrawString(count, countFont, Brushes.White,
                        badgeRect.X + (badgeRect.Width - countSize.Width) / 2,
                        badgeRect.Y + 1);
                }
            }

            using (var pen = new Pen(Color.FromArgb(230, 230, 230)))
                e.Graphics.DrawLine(pen, e.Bounds.X, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

            e.DrawFocusRectangle();
        }

        private void LstMessages_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || !(lstMessages.Items[e.Index] is Messenger.Shared.Models.Message message)) return;

            e.DrawBackground();

            bool isMyMessage = message.SenderId == currentUser.Id;

            int maxWidth = 500;
            int x = isMyMessage ? e.Bounds.Width - maxWidth - 20 : e.Bounds.X + 20;
            int width = Math.Min(maxWidth, TextRenderer.MeasureText(message.Text,
                new Font("Segoe UI", 10)).Width + 60);

            Color bgColor = isMyMessage ? Color.FromArgb(220, 248, 220) : Color.White;
            Color borderColor = isMyMessage ? Color.FromArgb(40, 167, 69) : Color.LightGray;

            var msgRect = new Rectangle(x, e.Bounds.Y + 2, width, e.Bounds.Height - 4);

            using (var bgBrush = new SolidBrush(bgColor))
            using (var borderPen = new Pen(borderColor))
            {
                e.Graphics.FillRectangle(bgBrush, msgRect);
                e.Graphics.DrawRectangle(borderPen, msgRect);
            }

            string header = $"{message.SenderName} • {message.SenderPosition}";
            if (message.SenderDepartmentId != currentUser.DepartmentId)
                header += " [др. отдел]";

            using (var nameFont = new Font("Segoe UI", 9, FontStyle.Bold))
                e.Graphics.DrawString(header, nameFont, Brushes.Black, x + 5, e.Bounds.Y + 5);

            using (var textFont = new Font("Segoe UI", 10))
                e.Graphics.DrawString(message.Text, textFont, Brushes.Black, x + 5, e.Bounds.Y + 25);

            string time = message.SentAt.ToString("HH:mm");
            using (var timeFont = new Font("Segoe UI", 8))
            {
                var timeSize = e.Graphics.MeasureString(time, timeFont);
                e.Graphics.DrawString(time, timeFont, Brushes.Gray,
                    x + width - timeSize.Width - 10, e.Bounds.Y + 45);
            }

            e.DrawFocusRectangle();
        }

        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Поиск чатов...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }

        private void TxtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Поиск чатов...";
                txtSearch.ForeColor = Color.Gray;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text.ToLower();
            if (string.IsNullOrWhiteSpace(search) || search == "поиск чатов...")
            {
                UpdateChatsList();
                return;
            }

            var filtered = chats.Where(c => c.Name.ToLower().Contains(search)).ToList();
            lstChats.Items.Clear();
            foreach (var chat in filtered)
                lstChats.Items.Add(chat);
        }

        private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Control)
            {
                SendMessage();
                e.SuppressKeyPress = true;
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(txtMessage.Text) || currentChat == null)
                return;

            var message = new Messenger.Shared.Models.Message
            {
                ChatId = currentChat.Id,
                Text = txtMessage.Text.Trim(),
                SentAt = DateTime.Now
            };

            var packet = new NetworkPacket
            {
                Command = NetworkCommand.SendMessage,
                SenderId = currentUser.Id,
                Data = message
            };

            try
            {
                formatter.Serialize(stream, packet);
                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки: {ex.Message}", "Ошибка");
            }
        }

        private void BtnNewChat_Click(object sender, EventArgs e)
        {
            if (currentUser == null) { MessageBox.Show("Сначала войдите в систему"); return; }

            using (var newChatForm = new NewChatForm(currentUser.DepartmentId))
            {
                if (newChatForm.ShowDialog() == DialogResult.OK)
                {
                    var packet = new NetworkPacket
                    {
                        Command = NetworkCommand.CreateExternalChat,
                        SenderId = currentUser.Id,
                        Data = newChatForm.SelectedDepartmentId
                    };
                    formatter.Serialize(stream, packet);
                }
            }
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Настройки будут доступны в следующей версии", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            Disconnect();
            ShowLoginForm();
        }
    }
}
