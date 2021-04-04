using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CommunityPricing.Data;
using CommunityPricing.Models;

namespace CommunityPricing.Pages.Admin.AdminArchivePages
{
    public class DeleteModel : PageModel
    {
        private readonly CommunityPricing.Data.CommunityPricingContext _context;

        public DeleteModel(CommunityPricing.Data.CommunityPricingContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ArchivedOffering ArchivedOffering { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ArchivedOffering = await _context.ArchivedOffering
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ArchivedOfferingID == id);

            if (ArchivedOffering == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ArchivedOffering = await _context.ArchivedOffering
                .AsNoTracking()
                .FirstOrDefaultAsync(ao => ao.ArchivedOfferingID == id);

            if (ArchivedOffering != null)
            {
                //_context.ArchivedOffering.Remove(ArchivedOffering);
                //await _context.SaveChangesAsync();
            }

            return RedirectToPage("./AdminArchive");
        }
    }
}
