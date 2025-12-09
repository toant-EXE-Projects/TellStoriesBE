using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Constants
{
    public class PaymentConst
    {
        public const string VNPay = "VNPay";

        public const string Method_Online = "Online";

        public const string Status_Pending = "Pending";
        public const string Status_Failed = "Failed";
        public const string Status_OK = "Success";

        public const string Payment_Description_Builder = @"[User {0} : {1}] - Subcription {2} : {3}";


    }
}
