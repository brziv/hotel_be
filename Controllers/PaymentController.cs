using hotel_be.ModelFromDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hotel_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        DBCnhom4 dbc;
        public PaymentController(DBCnhom4 dbc_in)
        {
            dbc = dbc_in;
        }

        [HttpGet]
        [Route("GetPaymentList")]
        public ActionResult Get()
        {
            return Ok(new { data = dbc.TblPayments.ToList() });
        }

        [HttpGet]
        [Route("SearchTblPayment")]
        public ActionResult TimKiem(string s)
        {
            var results = dbc.TblPayments
                .Where(item =>
                    item.PPaymentId.ToString().Contains(s) ||
                    item.PBookingId.ToString().Contains(s) ||
                    item.PAmountPaid.ToString().Contains(s) ||
                    item.PPaymentMethod.Contains(s) ||
                    (item.PPaymentDate.HasValue && item.PPaymentDate.Value.ToString().Contains(s))
                )
                .ToList();

            return Ok(new { data = results });
        }

        [HttpPost]
        [Route("InsertTblPayment")]
        public ActionResult Them(TblPayment payment)
        {
            dbc.TblPayments.Add(payment);
            dbc.SaveChanges();

            return Ok(new { data = payment });
        }

        [HttpPost]
        [Route("UpdateTblPayment")]
        public ActionResult Sua(Guid pPaymentId, Guid pBookingId, decimal pAmountPaid, string pPaymentMethod, DateTime? pPaymentDate)
        {
            TblPayment Payment = new TblPayment
            {
                PPaymentId = pPaymentId,
                PBookingId = pBookingId,
                PAmountPaid = pAmountPaid,
                PPaymentMethod = pPaymentMethod,
                PPaymentDate = pPaymentDate
            };
            dbc.TblPayments.Update(Payment);
            dbc.SaveChanges();
            return Ok(new { data = Payment });
        }

        [HttpPost]
        [Route("XoaTblPayment")]
        public ActionResult Xoa(Guid pPaymentId)
        {
            TblPayment Payment = new TblPayment
            {
                PPaymentId = pPaymentId,
            };

            dbc.TblPayments.Remove(Payment);
            dbc.SaveChanges();
            return Ok(new { data = Payment });
        }
    }
}
