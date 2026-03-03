using Messenger.Shared.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Server
{
    public class DatabaseManager
    {
        private string connectionString = "Server=localhost;Database=messenger_db;Uid=root;Pwd=vertrigo;";

        public bool TestConnection()
        {
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public User AuthenticateUser(string username, string password)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    SELECT u.*, d.name as dept_name 
                    FROM users u
                    JOIN departments d ON u.department_id = d.id
                    WHERE u.username = @username AND u.password_hash = SHA2(@password, 256)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("id"),
                                Username = reader.GetString("username"),
                                FullName = reader.GetString("full_name"),
                                DepartmentId = reader.GetInt32("department_id"),
                                DepartmentName = reader.GetString("dept_name"),
                                Position = reader.GetString("position"),
                                IsOnline = false
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void UpdateUserStatus(int userId, bool isOnline)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                    INSERT INTO user_status (user_id, is_online, last_seen) 
                    VALUES (@userId, @isOnline, NOW())
                    ON DUPLICATE KEY UPDATE 
                    is_online = @isOnline, last_seen = NOW()";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@isOnline", isOnline);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Chat> GetUserChats(int userId)
        {
            var chats = new List<Chat>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT u.department_id, d.name as dept_name,
                           COUNT(DISTINCT us.user_id) as online_count,
                           (SELECT message_text FROM messages 
                            WHERE department_chat_id = u.department_id 
                            ORDER BY sent_at DESC LIMIT 1) as last_msg,
                           (SELECT sent_at FROM messages 
                            WHERE department_chat_id = u.department_id 
                            ORDER BY sent_at DESC LIMIT 1) as last_time,
                           (SELECT COUNT(*) FROM messages m2 
                            WHERE m2.department_chat_id = u.department_id 
                            AND m2.id > COALESCE(
                                (SELECT last_read_message_id FROM user_chat_read 
                                 WHERE user_id = @userId AND chat_id = u.department_id AND is_department = 1), 0))
                            as unread
                    FROM users u
                    JOIN departments d ON u.department_id = d.id
                    LEFT JOIN user_status us ON u.id = us.user_id AND us.is_online = 1
                    WHERE u.id = @userId
                    GROUP BY u.department_id";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            chats.Add(new Chat
                            {
                                Id = reader.GetInt32("department_id"),
                                Name = $"🔒 {reader.GetString("dept_name")}",
                                Type = ChatType.Internal,
                                DepartmentId = reader.GetInt32("department_id"),
                                LastMessage = reader.IsDBNull(3) ? null : reader.GetString(3),
                                LastMessageTime = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                                UnreadCount = reader.GetInt32(5)
                            });
                        }
                    }
                }

                query = @"
                    SELECT dc.*, 
                           CASE WHEN dc.dept1_id = @deptId THEN d2.name ELSE d1.name END as other_dept_name,
                           CASE WHEN dc.dept1_id = @deptId THEN d2.id ELSE d1.id END as other_dept_id,
                           (SELECT message_text FROM messages 
                            WHERE chat_id = dc.id 
                            ORDER BY sent_at DESC LIMIT 1) as last_msg,
                           (SELECT sent_at FROM messages 
                            WHERE chat_id = dc.id 
                            ORDER BY sent_at DESC LIMIT 1) as last_time,
                           (SELECT COUNT(*) FROM messages m 
                            WHERE m.chat_id = dc.id 
                            AND m.id > COALESCE(
                                (SELECT last_read_message_id FROM user_chat_read 
                                 WHERE user_id = @userId AND chat_id = dc.id AND is_department = 0), 0))
                            as unread
                    FROM department_chats dc
                    JOIN departments d1 ON dc.dept1_id = d1.id
                    JOIN departments d2 ON dc.dept2_id = d2.id
                    WHERE dc.dept1_id = @deptId OR dc.dept2_id = @deptId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@deptId", GetUserDepartment(userId));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            chats.Add(new Chat
                            {
                                Id = reader.GetInt32("department_id"),
                                Name = $"🔒 {reader.GetString("dept_name")}",
                                Type = ChatType.Internal,
                                DepartmentId = reader.GetInt32("department_id"),
                                LastMessage = reader.IsDBNull(3) ? null : reader.GetString(3),
                                LastMessageTime = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                                UnreadCount = reader.GetInt32(5),
                                Participants = GetDepartmentUsers(reader.GetInt32("department_id")) // Добавить
                            });
                        }
                    }
                }
            }

            return chats;
        }

        private List<User> GetDepartmentUsers(int departmentId)
        {
            var users = new List<User>();
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT u.*, d.name as dept_name 
            FROM users u
            JOIN departments d ON u.department_id = d.id
            WHERE u.department_id = @deptId AND u.is_active = 1";
            }
            return users;
        }

        private int GetUserDepartment(int userId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT department_id FROM users WHERE id = @userId";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool UserHasAccessToChat(int userId, int chatId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT COUNT(*) FROM users 
                    WHERE id = @userId AND department_id = @chatId";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@chatId", chatId);

                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        return true;
                }

                query = @"
                    SELECT COUNT(*) FROM department_chats dc
                    JOIN users u ON u.id = @userId
                    WHERE dc.id = @chatId 
                    AND (dc.dept1_id = u.department_id OR dc.dept2_id = u.department_id)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@chatId", chatId);

                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public List<Message> GetChatMessages(int chatId, int userId)
        {
            var messages = new List<Message>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT m.*, u.full_name, u.position, u.department_id,
                           d.name as dept_name
                    FROM messages m
                    JOIN users u ON m.sender_id = u.id
                    JOIN departments d ON u.department_id = d.id
                    WHERE (m.chat_id = @chatId OR m.department_chat_id = @chatId)
                    ORDER BY m.sent_at ASC
                    LIMIT 100";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@chatId", chatId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new Message
                            {
                                Id = reader.GetInt32("id"),
                                ChatId = chatId,
                                SenderId = reader.GetInt32("sender_id"),
                                SenderName = reader.GetString("full_name"),
                                SenderPosition = reader.GetString("position"),
                                SenderDepartmentId = reader.GetInt32("department_id"),
                                Text = reader.GetString("message_text"),
                                SentAt = reader.GetDateTime("sent_at"),
                                IsRead = reader.GetBoolean("is_read")
                            });
                        }
                    }
                }
            }

            return messages;
        }

        public int SaveMessage(Message message)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO messages (chat_id, department_chat_id, sender_id, message_text, sent_at)
                    VALUES (@chatId, @deptChatId, @senderId, @text, @sentAt);
                    SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@chatId",
                        message.ChatId > 0 ? (object)message.ChatId : DBNull.Value);
                    cmd.Parameters.AddWithValue("@deptChatId",
                        message.ChatId == 0 ? (object)message.ChatId : DBNull.Value);
                    cmd.Parameters.AddWithValue("@senderId", message.SenderId);
                    cmd.Parameters.AddWithValue("@text", message.Text);
                    cmd.Parameters.AddWithValue("@sentAt", message.SentAt);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public void UpdateChatLastMessage(int chatId, string message, DateTime time)
        {
        }

        public Chat GetExternalChat(int dept1Id, int dept2Id)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT * FROM department_chats 
                    WHERE (dept1_id = @dept1 AND dept2_id = @dept2)
                       OR (dept1_id = @dept2 AND dept2_id = @dept1)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@dept1", dept1Id);
                    cmd.Parameters.AddWithValue("@dept2", dept2Id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Chat
                            {
                                Id = reader.GetInt32("id"),
                                Type = ChatType.External,
                                OtherDepartmentId = dept2Id
                            };
                        }
                    }
                }
            }
            return null;
        }

        public Chat CreateExternalChat(int dept1Id, int dept2Id)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO department_chats (dept1_id, dept2_id, created_at)
                    VALUES (@dept1, @dept2, NOW());
                    SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@dept1", dept1Id);
                    cmd.Parameters.AddWithValue("@dept2", dept2Id);

                    int chatId = Convert.ToInt32(cmd.ExecuteScalar());

                    return new Chat
                    {
                        Id = chatId,
                        Type = ChatType.External,
                        OtherDepartmentId = dept2Id
                    };
                }
            }
        }

        public void MarkMessagesAsRead(int chatId, int userId, int lastReadMessageId)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                bool isDepartment = chatId == GetUserDepartment(userId);

                string query = @"
                    INSERT INTO user_chat_read (user_id, chat_id, is_department, last_read_message_id, read_at)
                    VALUES (@userId, @chatId, @isDept, @lastMsg, NOW())
                    ON DUPLICATE KEY UPDATE 
                    last_read_message_id = @lastMsg, read_at = NOW()";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@chatId", chatId);
                    cmd.Parameters.AddWithValue("@isDept", isDepartment);
                    cmd.Parameters.AddWithValue("@lastMsg", lastReadMessageId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Department> GetAllDepartments()
        {
            var departments = new List<Department>();

            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT d.*, 
                           COUNT(DISTINCT u.id) as user_count,
                           COUNT(DISTINCT CASE WHEN us.is_online = 1 THEN u.id END) as online_count
                    FROM departments d
                    LEFT JOIN users u ON u.department_id = d.id AND u.is_active = 1
                    LEFT JOIN user_status us ON u.id = us.user_id AND us.is_online = 1
                    GROUP BY d.id
                    ORDER BY d.name";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        departments.Add(new Department
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Description = reader.GetString("description"),
                            OnlineCount = reader.GetInt32("online_count")
                        });
                    }
                }
            }

            return departments;
        }
    }
}
