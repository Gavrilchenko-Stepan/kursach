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
