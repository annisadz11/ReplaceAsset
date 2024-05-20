using Microsoft.AspNetCore.Mvc;
using ReplaceAsset.Data; // Pastikan Anda memiliki menggunakan directive untuk namespace ApplicationDbContext

public class DashboardController : Controller
{
	private readonly ApplicationDbContext _context; 
	public DashboardController(ApplicationDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public IActionResult GetQuickStatsData()
	{
		var totalRequests = _context.AssetRequest.Count();
		var totalNewHires = _context.NewHire.Count();
		var totalScraps = _context.AssetScrap.Count();
		var totalComponentReplacements = _context.ComponentAssetReplacement.Count();
		var totalNewAssetReplacements = _context.NewAssetReplacement.Count();

		return Json(new
		{
			totalRequests,
			totalNewHires,
			totalScraps,
			totalComponentReplacements,
			totalNewAssetReplacements
		});
	}
}
