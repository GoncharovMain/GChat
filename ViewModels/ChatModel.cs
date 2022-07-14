#nullable disable
using GChat.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GChat.ViewModels
{
    public class ChatModel
    {
        public string ChatName { get; set; }
        public IList<SelectListItem> IdMembers { get; set; }
        public IList<string> SelectedUsers { get; set; }
    }
}