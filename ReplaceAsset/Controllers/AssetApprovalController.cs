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
    public class AssetApprovalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetApprovalController(ApplicationDbContext context)
        {
            _context = context;
        }


/*        private async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Asset Management", "your-email@example.com"));
            emailMessage.To.Add(new MailboxAddress("", to));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = body };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.your-email-provider.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("your-email@example.com", "your-email-password");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
*/

        // GET: AssetRequest
        public IActionResult Index()
        {

            return View();
        }

        ///API ENDPOINT
        [HttpGet]
        public IActionResult GetData()
        {
            var AssetRequests = _context.AssetRequest
                .Where(g => g.Status == null) // Filter untuk status null
                .Select(g => new
                {
                    id = g.Id,
                    name = g.Name,
                    departement = g.Departement,
                    type = g.Type,
                    serialNumber = g.SerialNumber,
                    baseline = g.Baseline,
                    usageLocation = g.UsageLocation,
                    requestDate = g.RequestDate.HasValue ? g.RequestDate.Value.ToString("dd MMM yyyy") : null,
                    reason = g.Reason,
                    status = g.Status,
                    approvalDate = g.ApprovalDate.HasValue ? g.ApprovalDate.Value.ToString("dd MMM yyyy") : null,
                    justify = g.Justify,
                    typeReplace = g.TypeReplace,
                }).ToList();

            return Json(new { rows = AssetRequests });
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

            assetRequest.Status = true; // Asumsikan ini mengubah status menjadi 'approved'
            assetRequest.Justify = justify;
            assetRequest.TypeReplace = typeReplace;
            assetRequest.ApprovalDate = DateTime.Now;

            if (typeReplace == "NewAssetReplacement")
            {
                var newAssetReplacement = new NewAssetReplacement
                {
                    AssetRequestId = assetRequest.Id,
                    Name = assetRequest.Name, // Pastikan ini tidak null 
                    NewType = string.Empty, // Pastikan nilai ini disediakan sesuai dengan input
                    NewSerialNumber = string.Empty, // Pastikan nilai ini disediakan sesuai dengan input
                                                    // DateReplace tidak diisi di sini, asumsikan dapat ditambahkan nanti atau dapat null
                };
                _context.NewAssetReplacement.Add(newAssetReplacement);
            }
            else if (typeReplace == "ComponentAssetReplacement")
            {
                var componentAssetReplacement = new ComponentAssetReplacement
                {
                    AssetRequestId = assetRequest.Id,
                    Name = assetRequest.Name, // Pastikan ini tidak null
                                              // ValidationReplace dan ComponentReplaceDate dapat disetel berdasarkan bisnis logika yang diinginkan
                };
                _context.ComponentAssetReplacement.Add(componentAssetReplacement);
            }

            try
            {
                await _context.SaveChangesAsync();
                // Kirim email ke user
                var subject = "Asset Replacement Request Approved";
                var body = $"Your asset replacement request has been approved. Justification: {justify}.";
/*                await SendEmailAsync("user@example.com", subject, body);
*/
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
            var assetRequest = await _context.AssetRequest.FindAsync(id);
            if (assetRequest == null)
            {
                return NotFound();
            }

            assetRequest.Status = false; // Mengubah status menjadi false (0) untuk rejected
            assetRequest.Justify = justify;
            assetRequest.ApprovalDate = DateTime.Now;
            _context.Update(assetRequest);
            await _context.SaveChangesAsync();

            // Kirim email ke user
            var subject = "Asset Replacement Request Rejected";
            var body = $"Your asset replacement request has been rejected. Justification: {justify}.";
/*            await SendEmailAsync("user@example.com", subject, body);
*/

            return Json(new { success = true, message = "Asset request rejected successfully!" });
        }

        private bool AssetRequestExists(int id)
        {
            return _context.AssetRequest.Any(e => e.Id == id);
        }
    }
}

