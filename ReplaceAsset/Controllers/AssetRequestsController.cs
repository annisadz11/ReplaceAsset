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

        [Authorize(Roles = "UserManagerIT,UserAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: AssetRequests/Edit/5
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Departement,Type,SerialNumber,Baseline,UsageLocation,EmailUser,RequestDate,Reason,Status,ApprovalDate,Justify,TypeReplace")] AssetRequest assetRequest)
        {
            if (id != assetRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Only update the properties you want to allow editing
                    var assetRequestToUpdate = await _context.AssetRequest.FindAsync(id);

                    if (assetRequestToUpdate == null)
                    {
                        return NotFound();
                    }

                    assetRequestToUpdate.Name = assetRequest.Name;
                    assetRequestToUpdate.Departement = assetRequest.Departement;
                    assetRequestToUpdate.Type = assetRequest.Type;
                    assetRequestToUpdate.SerialNumber = assetRequest.SerialNumber;
                    assetRequestToUpdate.Baseline = assetRequest.Baseline;
                    assetRequestToUpdate.UsageLocation = assetRequest.UsageLocation;
                    assetRequestToUpdate.EmailUser = assetRequest.EmailUser;
                    assetRequestToUpdate.Reason = assetRequest.Reason;
                    assetRequestToUpdate.Status = assetRequest.Status;
                    assetRequestToUpdate.ApprovalDate = assetRequest.ApprovalDate;
                    assetRequestToUpdate.Justify = assetRequest.Justify;
                    assetRequestToUpdate.TypeReplace = assetRequest.TypeReplace;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetRequestExists(assetRequest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Asset request edited successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(assetRequest);
        }

        // GET: AssetRequests/Edit/5
        [Authorize(Roles = "UserManagerIT,UserAdmin")]
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
        // POST: AssetRequests/Delete/5
        [Authorize(Roles = "UserManagerIT,UserAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assetRequest = await _context.AssetRequest.FindAsync(id);
            if (assetRequest != null)
            {
                _context.AssetRequest.Remove(assetRequest);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Asset request deleted successfully!";
            return Json(new { success = true, message = "Asset request deleted successfully!" });
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