using StoryTeller.Data.Entities;

namespace StoryTeller.Data.Constants
{
    public static class NotificationConst
    {
        public static readonly Dictionary<string, Notification> Notifications = new()
        {
            ["WELCOME"] = new Notification
            {
                Title = "Chào mừng bạn đến với Stories Teller!",
                Message = "Chúc mừng bạn đã gia nhập cộng đồng sáng tạo tại Stories Teller! " +
                  "Tại đây, bạn có thể đọc, viết, chia sẻ câu chuyện của riêng mình, " +
                  "và khám phá hàng ngàn truyện hấp dẫn từ những tác giả khác.\n\n" +
                  "💡 Hãy bắt đầu bằng cách tạo câu chuyện đầu tiên hoặc trò chuyện cùng AI để tìm cảm hứng nhé!"
            },

            ["PAYMENT_SUCCESS"] = new Notification
            {
                Title = "Xác nhận thanh toán thành công",
                Message = "Giao dịch của bạn đã được xử lý thành công. Hóa đơn đã được gửi về email. " +
                      "Nếu bạn không thực hiện giao dịch này, hãy liên hệ hỗ trợ ngay."
            },

            ["MAINTENANCE_NOTICE"] = new Notification
            {
                Title = "Thông báo bảo trì hệ thống",
                // {0}hh:mm {1}hh:mm dd:MM:yyyy
                Message = @"Hệ thống sẽ bảo trì từ {0} đến {1}. Trong thời gian này, " +
                      "một số tính năng có thể tạm thời không khả dụng. Mong bạn thông cảm."
            }
        };
    }
}
