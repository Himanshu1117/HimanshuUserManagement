using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class Messages
    {
        public enum MessageType
        {

            Message1,
            Message2,
            Message3,
            Message4,
            Message5,
            Message6
        };
        private static readonly Dictionary<MessageType, OperationResult> _messages = new Dictionary<MessageType, OperationResult>
    {
        { MessageType.Message1, new OperationResult { IsSuccess = false, ErrorMessage = "User does not exist." } },
        { MessageType.Message2, new OperationResult { IsSuccess = true, SuccessMessage = $"Email for reset password sent successfully."+
                               $"Link will be expired in next 10 minutes." } },
        { MessageType.Message3, new OperationResult { IsSuccess = false, ErrorMessage = $"User does not exist." +
                                   $"Please enter correct emailId."} },
        { MessageType.Message4, new OperationResult { IsSuccess = false, ErrorMessage = $"Session Time Expired." +
                                   $"Please Login again to proceed." } },
        { MessageType.Message5, new OperationResult { IsSuccess = true, ErrorMessage =  $"Password reset successfully"} },
        { MessageType.Message6, new OperationResult { IsSuccess = true, ErrorMessage =  $"Password reset successfully"} },


    };

        public static OperationResult GetMessage(MessageType messageType)
        {
            return _messages.TryGetValue(messageType, out var result) ? result : null;
        }

    }

}
