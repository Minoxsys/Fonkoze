using System;
using Domain.Enums;
using Web.Areas.MessagesManagement.Models.Messages;

namespace Web.Services
{
    public interface IRawSmsMeesageQueryHelpersService
    {
        MessageIndexOuputModel GetMessagesFromOutpost(MessagesIndexModel indexModel, OutpostType outpostType, Guid? districtId = null);
    }
}
