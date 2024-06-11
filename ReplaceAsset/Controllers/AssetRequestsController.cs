using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using ReplaceAsset.Data;
using ReplaceAsset.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.DirectoryServices.Protocols;
using ClosedXML.Excel;

namespace ReplaceAsset.Controllers
{
    public class AssetRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Endpoint untuk menghitung data
        [HttpGet]
        public IActionResult GetDashboardData()
        {
            var totalRequests = _context.AssetRequest.Count();
            var totalApproved = _context.AssetRequest.Count(r => r.Status == true);
            var totalRejected = _context.AssetRequest.Count(r => r.Status == false);
            var waitingForApproval = _context.AssetRequest.Count(r => r.Status == null);

            return Json(new
            {
                totalRequests,
                totalApproved,
                totalRejected,
                waitingForApproval
            });
        }

		//Endpoint untuk Apextchart Requestor
		[HttpGet]
		public IActionResult GetRequestorData()
		{
			var requestCounts = _context.AssetRequest
				.GroupBy(r => r.RequestDate.HasValue ? r.RequestDate.Value.Month : (int?)null)
				.Select(g => new
				{
					Month = g.Key,
					Count = g.Count()
				})
				.ToList();

			var requestData = new int[12];
			foreach (var req in requestCounts)
			{
				if (req.Month.HasValue)
					requestData[req.Month.Value - 1] = req.Count;
			}

			return Json(requestData);
		}

        //Endpoint untuk TotalRequest
		[HttpGet]
		public IActionResult GetTotalRequests()
		{
			var totalRequests = _context.AssetRequest.Count();
			return Json(totalRequests);
		}

        //Endpoint untuk RequestHistory
		[HttpGet]
		public IActionResult GetRequestHistory()
		{
			var requests = _context.AssetRequest
				.OrderByDescending(r => r.RequestDate)
				.Select(r => new
				{
					r.Id,
					r.Name,
					r.Departement,
					r.Reason,
					r.Status,
					r.ApprovalDate
				})
				.Take(10) // Menampilkan 10 permintaan terakhir
				.ToList();

			return Json(requests);
		}


        // Endpoint untuk Ekspor ke Excel
        public IActionResult ExportToExcel()
        {
            var assetRequests = _context.AssetRequest.ToList();

            using (var workbook = new XLWorkbook())
            {
                // Mengubah nama worksheet menjadi "Asset Requests" dengan spasi
                var worksheet = workbook.Worksheets.Add("Asset Requests");

                // Membuat header pada Excel dengan nama kolom memiliki spasi yang sesuai
                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1, 2).Value = "Email User";
                worksheet.Cell(1, 3).Value = "Departement";
                worksheet.Cell(1, 4).Value = "Type";
                worksheet.Cell(1, 5).Value = "Serial Number";
                worksheet.Cell(1, 6).Value = "Baseline";
                worksheet.Cell(1, 7).Value = "Usage Location";
                worksheet.Cell(1, 8).Value = "Request Date";
                worksheet.Cell(1, 9).Value = "Reason";
                worksheet.Cell(1, 10).Value = "Status";
                worksheet.Cell(1, 11).Value = "Approval Date";
                worksheet.Cell(1, 12).Value = "Justify";
                worksheet.Cell(1, 13).Value = "Type Replace";

                // Mengisi data pada Excel
                for (int i = 0; i < assetRequests.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = assetRequests[i].Name;
                    worksheet.Cell(i + 2, 2).Value = assetRequests[i].EmailUser;
                    worksheet.Cell(i + 2, 3).Value = assetRequests[i].Departement;
                    worksheet.Cell(i + 2, 4).Value = assetRequests[i].Type;
                    worksheet.Cell(i + 2, 5).Value = assetRequests[i].SerialNumber;
                    worksheet.Cell(i + 2, 6).Value = assetRequests[i].Baseline;
                    worksheet.Cell(i + 2, 7).Value = assetRequests[i].UsageLocation;
                    worksheet.Cell(i + 2, 8).Value = assetRequests[i].RequestDate;
                    worksheet.Cell(i + 2, 9).Value = assetRequests[i].Reason;
                    worksheet.Cell(i + 2, 10).Value = assetRequests[i].Status.HasValue ? (assetRequests[i].Status.Value ? "Approved" : "Rejected") : "Waiting for Approval";
                    worksheet.Cell(i + 2, 11).Value = assetRequests[i].ApprovalDate;
                    worksheet.Cell(i + 2, 12).Value = assetRequests[i].Justify;
                    worksheet.Cell(i + 2, 13).Value = assetRequests[i].TypeReplace;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    // Mengubah nama file menjadi "Asset Requests.xlsx" dengan spasi
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Asset Requests.xlsx");
                }
            }
        }


        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern,UserEmployee")]
        // GET: AssetRequest
        public IActionResult Index()
        {
            return View();
        }

        // API ENDPOINT
        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern,UserEmployee")]
        [HttpGet]
        public IActionResult GetData(DateTime? startDate, DateTime? endDate)
        {
            var assetRequests = _context.AssetRequest
                .Where(ar =>
                    (!startDate.HasValue || ar.RequestDate >= startDate) &&
                    (!endDate.HasValue || ar.RequestDate <= endDate))
                .Select(g => new
                {
                    id = g.Id,
                    name = g.Name,
                    departement = g.Departement,
                    emailUser = g.EmailUser,
                    type = g.Type,
                    serialNumber = g.SerialNumber,
                    baseline = g.Baseline,
                    usageLocation = g.UsageLocation,
                    requestDate = g.RequestDate.HasValue ? g.RequestDate.Value.ToString("dd MMM yyyy  HH:mm") : null,
                    reason = g.Reason,
                    status = g.Status,
                    approvalDate = g.ApprovalDate.HasValue ? g.ApprovalDate.Value.ToString("dd MMM yyyy HH:mm") : null,
                    justify = g.Justify,
                    typeReplace = g.TypeReplace,
                }).ToList();

            return Json(new { rows = assetRequests });
        }


        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]
        [HttpGet]
        public IActionResult GetDataByStatus(string status)
        {
            List<AssetRequest> assetRequests = null;

            switch (status.ToLower())
            {
                case "approved":
                    assetRequests = _context.AssetRequest.Where(ar => ar.Status == true).ToList();
                    break;
                case "rejected":
                    assetRequests = _context.AssetRequest.Where(ar => ar.Status == false).ToList();
                    break;
                case "waiting":
                    assetRequests = _context.AssetRequest.Where(ar => ar.Status == null).ToList();
                    break;
                case "all":
                    assetRequests = _context.AssetRequest.ToList();
                    break;
                default:
                    return BadRequest("Invalid status selected.");
            }

            var result = assetRequests.Select(g => new
            {
                id = g.Id,
                name = g.Name,
                departement = g.Departement,
                emailUser = g.EmailUser,
                type = g.Type,
                serialNumber = g.SerialNumber,
                baseline = g.Baseline,
                usageLocation = g.UsageLocation,
                requestDate = g.RequestDate.HasValue ? g.RequestDate.Value.ToString("dd MMM yyyy  HH:mm") : null,
                reason = g.Reason,
                status = g.Status,
                approvalDate = g.ApprovalDate.HasValue ? g.ApprovalDate.Value.ToString("dd MMM yyyy HH:mm") : null,
                justify = g.Justify,
                typeReplace = g.TypeReplace,
            }).ToList();

            return Json(result);
        }

        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern,UserEmployee")]
        // GET: AssetRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetRequest = await _context.AssetRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assetRequest == null)
            {
                return NotFound();
            }

            return View(assetRequest);
        }

        [Authorize(Roles = "UserAdmin,UserEmployee")]
        // GET: AssetRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AssetRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Departement,Type,SerialNumber,Baseline,UsageLocation,EmailUser,RequestDate,Reason")] AssetRequest assetRequest)
        {
            if (ModelState.IsValid)
            {
                // Set default values for properties that should not be set in the Create action
                assetRequest.Username = User.Identity.Name;
                assetRequest.Status = null;
                assetRequest.ApprovalDate = null;
                assetRequest.Justify = null;
                assetRequest.TypeReplace = null;

                _context.Add(assetRequest);
                await _context.SaveChangesAsync();

                // Mengirim email ke UserManagerIT
                var userManagerIT = await _context.UserManagerITs.FirstOrDefaultAsync();
                if (userManagerIT != null)
                {
                    await SendEmailAsync(
                        assetRequest.EmailUser,
                        userManagerIT.Email,
                        assetRequest.EmailUser,
                        "New Asset Request from " + assetRequest.Name,
                        assetRequest.Name,
                        assetRequest.Departement,
                        assetRequest.Type,
                        assetRequest.SerialNumber,
                        assetRequest.Baseline,
                        assetRequest.UsageLocation,
                        assetRequest.RequestDate,
                        assetRequest.Reason,
                        assetRequest.Name
                    );
                }

                TempData["SuccessMessage"] = "Asset request created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(assetRequest);
        }


        private async Task SendEmailAsync(string fromEmail, string toEmail, string userEmail, 
            string subject, string name, string department, string type, string serialNumber, string baseline, 
            string usageLocation, DateTime? requestDate, string reason, string userName)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Asset Replacement Notification", "bth-esh@infineon.com"));
            message.To.Add(new MailboxAddress("Recipient", toEmail));
            message.Cc.Add(new MailboxAddress("CC Recipient", userEmail));
            message.Subject = $"New Asset Request from {name}";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
    <p>Dear All,</p>
    <p>{name} has submitted a new asset request with the following details:</p>
    <ul>
        <li><strong>Name:</strong> {name}</li>
        <li><strong>Department:</strong> {department}</li>
        <li><strong>Type:</strong> {type}</li>
        <li><strong>Serial Number:</strong> {serialNumber}</li>
        <li><strong>Baseline:</strong> {baseline}</li>
        <li><strong>Usage Location:</strong> {usageLocation}</li>
        <li><strong>Request Date:</strong> {requestDate?.ToString("dd MMM yyyy HH:mm")}</li>
        <li><strong>Reason:</strong> {reason}</li>
    </ul>
    <p>Please review and take necessary action in Form Approval Request Asset Replacement.</p>
    <p>Regards,<br>Asset Replacement Notification System</p>
";
            message.Body = bodyBuilder.ToMessageBody();


            using var smtp = new SmtpClient();
            smtp.Connect("mailrelay-internal.infineon.com", 25, false);
            await smtp.SendAsync(message);
            smtp.Disconnect(true);
        }
        [Authorize(Roles = "UserAdmin,UserManagerIT,UserIntern")]
        [HttpPost]
        public async Task<IActionResult> DeleteByStatus([FromBody] StatusRequest statusRequest)
        {
            if (statusRequest == null || string.IsNullOrEmpty(statusRequest.Status))
            {
                return BadRequest("Invalid status selected.");
            }

            IEnumerable<AssetRequest> assetRequests = null;

            switch (statusRequest.Status.ToLower())
            {
                case "approved":
                    assetRequests = await _context.AssetRequest.Where(ar => ar.Status == true).ToListAsync();
                    break;
                case "rejected":
                    assetRequests = await _context.AssetRequest.Where(ar => ar.Status == false).ToListAsync();
                    break;
                case "waiting":
                    assetRequests = await _context.AssetRequest.Where(ar => ar.Status == null).ToListAsync();
                    break;
                default:
                    return BadRequest("Invalid status selected.");
            }

            if (!assetRequests.Any())
            {
                return NotFound("No requests found with the specified status.");
            }

            _context.AssetRequest.RemoveRange(assetRequests);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Selected asset requests were deleted successfully." });
        }

        public class StatusRequest
        {
            public string Status { get; set; }
        }

        // Fungsi Generate PDF berdasarkan status
        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]
        [HttpGet]
        public IActionResult GeneratePdfByStatus(string status)
        {
            List<AssetRequest> assetRequests = null;

            switch (status)
            {
                case "Approved":
                    assetRequests = _context.AssetRequest.Where(ar => ar.Status == true).ToList();
                    break;
                case "Rejected":
                    assetRequests = _context.AssetRequest.Where(ar => ar.Status == false).ToList();
                    break;
                case "Waiting for Approval":
                    assetRequests = _context.AssetRequest.Where(ar => ar.Status == null).ToList();
                    break;
                default:
                    return BadRequest("Invalid status selected.");
            }

            var result = assetRequests.Select(g => new
            {
                id = g.Id,
                name = g.Name,
                departement = g.Departement,
                emailUser = g.EmailUser,
                type = g.Type,
                serialNumber = g.SerialNumber,
                baseline = g.Baseline,
                usageLocation = g.UsageLocation,
                requestDate = g.RequestDate.HasValue ? g.RequestDate.Value.ToString("dd MMM yyyy  HH:mm") : null,
                reason = g.Reason,
                status = g.Status,
                approvalDate = g.ApprovalDate.HasValue ? g.ApprovalDate.Value.ToString("dd MMM yyyy HH:mm") : null,
                justify = g.Justify,
                typeReplace = g.TypeReplace,
            }).ToList();

            return Json(result);
        }
        [Authorize(Roles = "UserAdmin,UserManagerIT,UserIntern")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assetRequest = await _context.AssetRequest.FindAsync(id);
            if (assetRequest == null)
            {
                return Json(new { success = false, message = "Asset request not found." });
            }

            _context.AssetRequest.Remove(assetRequest);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Asset request deleted successfully." });
        }

        private bool AssetRequestExists(int id)
        {
            return _context.AssetRequest.Any(e => e.Id == id);
        }

        public IActionResult ApprovalList()
        {
            var assetrequestToApprove = _context.AssetRequest.Where(c => !c.Status.HasValue);
            return View(assetrequestToApprove);
        }

        // Additional Model for Delete Request
       
    }
}