using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using ReplaceAsset.Data;
using ReplaceAsset.Models;
using Microsoft.AspNetCore.Authorization;


namespace ReplaceAsset.Controllers
{
    [Authorize(Roles = "UserManagerIT,UserAdmin")]

    public class AssetApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetApprovalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AssetRequest
        public IActionResult Index()
        {

            return View();
        }

        ///API ENDPOINT

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



        //APPROVE MANAGER
        [HttpPost]
        public async Task<IActionResult> Approve(int id, string justify, string typeReplace)
        {
            var assetRequest = await _context.AssetRequest.FindAsync(id);
            if (assetRequest == null)
            {
                return NotFound();
            }

            assetRequest.Status = true;
            assetRequest.Justify = justify;
            assetRequest.TypeReplace = typeReplace;
            assetRequest.ApprovalDate = DateTime.Now;

            if (typeReplace == "NewAssetReplacement")
            {
                var newAssetReplacement = new NewAssetReplacement
                {
                    AssetRequestId = assetRequest.Id,
                    Name = assetRequest.Name,
                    NewType = string.Empty,
                    NewSerialNumber = string.Empty,
                };
                _context.NewAssetReplacement.Add(newAssetReplacement);
            }
            else if (typeReplace == "ComponentAssetReplacement")
            {
                var componentAssetReplacement = new ComponentAssetReplacement
                {
                    AssetRequestId = assetRequest.Id,
                    Name = assetRequest.Name,
                };
                _context.ComponentAssetReplacement.Add(componentAssetReplacement);
            }

            try
            {
                await _context.SaveChangesAsync();

                // Mengirim email ke UserEmployee
                await SendApprovalEmailToUserAsync(assetRequest, justify, typeReplace);

                return Json(new { success = true, message = "Asset request approved successfully!" });
            }
            catch (DbUpdateException ex)
            {
                // Log detail exception
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = "Error approving the request: " + ex.Message });
            }
        }

        //REJECT MANAGER
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string justify)
        {
            try
            {
                var assetRequest = await _context.AssetRequest.FindAsync(id);
                if (assetRequest == null)
                {
                    return NotFound();
                }

                assetRequest.Status = false;
                assetRequest.Justify = justify;
                assetRequest.ApprovalDate = DateTime.Now;
                _context.Update(assetRequest);
                await _context.SaveChangesAsync();

                // Mengirim email ke UserEmployee
                await SendRejectionEmailAsync(assetRequest.EmailUser, assetRequest, justify);

                return Json(new { success = true, message = "Asset request rejected successfully!" });
            }
            catch (Exception ex)
            {
                // Log detail exception
                Console.WriteLine(ex.ToString());
                return Json(new { success = false, message = "Error rejecting the request: " + ex.Message });
            }
        }

        private async Task SendRejectionEmailAsync(string emailUser, AssetRequest assetRequest, string justify)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Asset Replacement Notification", "bth-esh@infineon.com"));
            message.To.Add(new MailboxAddress("Recipient", emailUser));
            message.Subject = $"Asset Replacement Request Rejected";

            // Mendapatkan email UserManagerIT dan UserIntern
            var userManagerIT = await _context.UserManagerITs.FirstOrDefaultAsync();
            var userIntern = await _context.UserInterns.FirstOrDefaultAsync();

            // Menambahkan CC ke UserManagerIT dan UserIntern
            if (userManagerIT != null)
            {
                message.Cc.Add(new MailboxAddress("Manager IT", userManagerIT.Email));
            }

            if (userIntern != null)
            {
                message.Cc.Add(new MailboxAddress("Intern", userIntern.Email));
            }

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
        <p>Dear {assetRequest.Name},</p>
        <p>We regret to inform you that your asset replacement request has been rejected with the following details:</p>
        <ul>
            <li><strong>Name:</strong> {assetRequest.Name}</li>
            <li><strong>Department:</strong> {assetRequest.Departement}</li>
            <li><strong>Type:</strong> {assetRequest.Type}</li>
            <li><strong>Serial Number:</strong> {assetRequest.SerialNumber}</li>
            <li><strong>Baseline:</strong> {assetRequest.Baseline}</li>
            <li><strong>Usage Location:</strong> {assetRequest.UsageLocation}</li>
            <li><strong>Request Date:</strong> {assetRequest.RequestDate}</li>
            <li><strong>Reason:</strong> {assetRequest.Reason}</li>
            <li><strong>Justify:</strong> {justify}</li>
        </ul>
        <p>If you have any questions or concerns, please contact the IT department.</p>
        <p>Regards,<br>Asset Replacement Notification System</p>";
            message.Body = bodyBuilder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect("mailrelay-internal.infineon.com", 25, false);
            await smtp.SendAsync(message);
            smtp.Disconnect(true);
        }

        private async Task SendApprovalEmailToUserAsync(AssetRequest assetRequest, string justify, string typeReplace)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Asset Replacement Notification", "bth-esh@infineon.com"));
            message.To.Add(new MailboxAddress("Recipient", assetRequest.EmailUser));
            message.Subject = $"Asset Replacement Request has been Approved";

            // Mendapatkan email UserManagerIT dan UserIntern
            var userManagerIT = await _context.UserManagerITs.FirstOrDefaultAsync();
            var userIntern = await _context.UserInterns.FirstOrDefaultAsync();

            // Menambahkan CC ke UserManagerIT dan UserIntern
            if (userManagerIT != null)
            {
                message.Cc.Add(new MailboxAddress("Manager IT", userManagerIT.Email));
            }

            if (userIntern != null)
            {
                message.Cc.Add(new MailboxAddress("Intern", userIntern.Email));
            }

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
        <p>Dear {assetRequest.Name},</p>
        <p>Your asset replacement request has been approved by the IT manager with the following details:</p>
        <ul>
            <li><strong>Name:</strong> {assetRequest.Name}</li>
            <li><strong>Department:</strong> {assetRequest.Departement}</li>
            <li><strong>Type:</strong> {assetRequest.Type}</li>
            <li><strong>Serial Number:</strong> {assetRequest.SerialNumber}</li>
            <li><strong>Baseline:</strong> {assetRequest.Baseline}</li>
            <li><strong>Usage Location:</strong> {assetRequest.UsageLocation}</li>
            <li><strong>Request Date:</strong> {assetRequest.RequestDate}</li>
            <li><strong>Reason:</strong> {assetRequest.Reason}</li>
            <li><strong>Justify:</strong> {justify}</li>
            <li><strong>Type Replace:</strong> {typeReplace}</li>
        </ul>
        <p>Please note that the replacement process is only valid for 7 days after approval, come to the IT room by paying attention to the IT Time Window on the Dashboard page</p>
        <p>Further actions will be taken to process your request.</p>
        <p>Regards,<br>Asset Replacement Notification System</p>";
            message.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect("mailrelay-internal.infineon.com", 25, false);
            await smtp.SendAsync(message);
            smtp.Disconnect(true);
        }

        private bool AssetRequestExists(int id)
        {
            return _context.AssetRequest.Any(e => e.Id == id);
        }
    }
}

