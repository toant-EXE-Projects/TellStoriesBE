namespace StoryTeller.Data.Constants
{
    public static class EmailTemplates
    {
        public const string EmailTemplatePath = "email-templates";

        public const string Prefix_Flair = "[TellStories]";


        public const string VerificationCodeFile = "verification-code.html";
        public const string EmailConfirmationSubject = @$"{Prefix_Flair} Xác minh tài khoản Email của bạn";
        public const string EmailConfirmationBodyPurpose = "Mã xác thực email của bạn là:";

        public const string PasswordResetSubject = @$"{Prefix_Flair} Khôi phục mật khẩu";
        public const string PasswordResetBodyPurpose = "Mã khôi phục mật khẩu của bạn là:";

        public const string SubscriptionReminderFile = "subscription-reminder.html";
        public const string SubscriptionReminderSubject = @$"{Prefix_Flair} Gói đăng ký của bạn sắp hết hạn";

        public const string SubscriptionSuccessSubject = $"{Prefix_Flair} Thanh toán gói đăng ký thành công!";
        public const string SubscriptionSuccessBodyFile = "payment-receipt.html";

    }
}
