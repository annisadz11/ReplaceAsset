﻿using System;
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

        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern,UserEmployee")]
        // GET: AssetRequest
        public IActionResult Index()
        {
            return View();
        }

        // API ENDPOINT
        [HttpGet]
        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern,UserEmployee")]
        public IActionResult GetData()
        {
            var AssetRequests = _context.AssetRequest
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

            return Json(new { rows = AssetRequests });
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

        [Authorize(Roles = "UserIntern,UserAdmin,UserEmployee")]
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


        [Authorize(Roles = "UserAdmin,UserIntern")]
        // GET: AssetRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetRequest = await _context.AssetRequest.FindAsync(id);
            if (assetRequest == null)
            {
                return NotFound();
            }
            return View(assetRequest);
        }

        // POST: AssetRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Departement,Type,SerialNumber,Baseline,UsageLocation,RequestDate,Reason,Status,ApprovalDate,Justify,TypeReplace")] AssetRequest assetRequest)
        {
            if (id != assetRequest.Id)
            {
                return NotFound();
            }

            var existingRecord = await _context.AssetRequest.FindAsync(id);
            if (existingRecord == null)
            {
                return NotFound();
            }

            // Periksa status permintaan
            if (existingRecord.Status.HasValue)
            {
                // Permintaan telah disetujui atau ditolak, tidak dapat diedit
                TempData["ErrorMessage"] = "This asset request cannot be edited because it has already been approved or rejected.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingRecord.Name = assetRequest.Name;
                    existingRecord.Departement = assetRequest.Departement;
                    existingRecord.Type = assetRequest.Type;
                    existingRecord.SerialNumber = assetRequest.SerialNumber;
                    existingRecord.Baseline = assetRequest.Baseline;
                    existingRecord.UsageLocation = assetRequest.UsageLocation;
                    existingRecord.RequestDate = assetRequest.RequestDate;
                    existingRecord.Reason = assetRequest.Reason;
                    existingRecord.Status = assetRequest.Status;
                    existingRecord.ApprovalDate = assetRequest.ApprovalDate;
                    existingRecord.Justify = assetRequest.Justify;
                    existingRecord.TypeReplace = assetRequest.TypeReplace;

                    _context.Update(existingRecord);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Asset Request has been updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetRequestExists(existingRecord.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(assetRequest);
        }

        [Authorize(Roles = "UserManagerIT,UserAdmin,UserIntern")]
        // GET: AssetRequests/Delete/5
        public async Task<IActionResult> Delete(int? id, string handler)
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

            if (handler == "Delete")
            {
                _context.AssetRequest.Remove(assetRequest);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Asset request has been deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(assetRequest);
        }

        // POST: AssetRequests/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id, string handler)
        {
            var assetRequest = await _context.AssetRequest.FindAsync(id);
            if (assetRequest == null)
            {
                return Json(new { success = false, message = "Asset request not found." });
            }

            if (handler == "Delete")
            {
                _context.AssetRequest.Remove(assetRequest);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Asset request has been deleted successfully." });
            }

            return Json(new { success = false, message = "Invalid operation." });
        }

        [HttpPost]
        [Authorize(Roles = "UserAdmin")]
        public async Task<IActionResult> DeleteSelected(List<int> ids)
        {
            var assetRequests = _context.AssetRequest.Where(r => ids.Contains(r.Id)).ToList();
            if (assetRequests.Count == 0)
            {
                return Json(new { success = false, message = "No asset requests found." });
            }

            _context.AssetRequest.RemoveRange(assetRequests);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"{assetRequests.Count} asset requests have been deleted successfully." });
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


    }
}