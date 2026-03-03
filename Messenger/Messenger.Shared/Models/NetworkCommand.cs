using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Shared.Models
{
    public enum NetworkCommand
    {
        Login,
        Logout,
        GetChats,
        GetChatMessages,
        SendMessage,
        NewMessage,
        UserStatusChanged,
        GetDepartments,
        CreateExternalChat,
        MessagesRead
    }
}
