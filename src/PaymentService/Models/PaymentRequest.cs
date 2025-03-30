using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Models
{
    /// <summary>
    /// Payment request data structure matching what's published from the order service
    /// </summary>
    public class PaymentRequest
    {
        public string OrderId { get; set; }
        public string TotalPrice {  get; set; }
        public string Status {  get; set; }
    }

    /// <summary>
    /// SNS message wrapper structure
    /// </summary>
    public class SnsWrapper
    {
        public string Type { get; set; }
        public string MessageId { get; set; }
        public string TopicArn { get; set; }
        public string Message { get; set; }
        public string Timestamp { get; set; }
    }
}
