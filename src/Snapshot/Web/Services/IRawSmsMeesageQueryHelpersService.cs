using System;
using Domain.Enums;
using Web.Areas.MessagesManagement.Models.Messages;
using Web.Models.Shared;

namespace Web.Services
{
    public interface IRawSmsMeesageQueryHelpersService
    {
        StoreOutputModel<MessageModel> GetMessagesFromOutpost(IndexTableInputModel indexTableInputModel, OutpostType outpostType, Guid? districtId = null);
    }
}
