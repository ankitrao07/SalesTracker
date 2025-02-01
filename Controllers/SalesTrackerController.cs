using SalesTracker.Models;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SalesTracker.Models.DTO;

namespace SalesTracker.Controllers
{
    public class SalesTrackerController : Controller
    {
        private readonly SalesTrackerRepository _salesTrackerRepository;
        private readonly IMapper _mapper;
        public SalesTrackerController(SalesTrackerRepository salesTrackerRepository,IMapper mapper)
        {
            _salesTrackerRepository = salesTrackerRepository;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            LeadCounts leadCounts = _salesTrackerRepository.GetLeadCounts();
            return View(leadCounts);
        }
        public IActionResult LeadIndex()
        {
            LeadCounts leadCounts = _salesTrackerRepository.GetLeadCounts();
            return View(leadCounts);
        }
        public IActionResult SalesTracker1()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if(file !=null && file.Length>0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine("wwwroot/uploads", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var uploadFile = new UploadedFile
                {
                    FileName = fileName,
                    FilePath = filePath,
                    UploadDate = DateTime.Now
                };
                ViewData["UploadedFile"] = uploadFile;

            }
            return RedirectToAction("LeadTracker");
        }
        [HttpPost] 
      

        [HttpPost]
        public IActionResult AddLeadNew([FromBody] LeadCompositeDTO leadCompositeDTO)
        {
            leadCompositeDTO.Lead.DocNo = Utility.Utility.GenerateUniqueDocNo();
            if (ModelState.IsValid)
            {
                var leadNew = _mapper.Map<LeadNew>(leadCompositeDTO.Lead);
                var leadActivity = _mapper.Map<LeadActivity>(leadCompositeDTO.LeadActivity);
                var leadId = _salesTrackerRepository.AddLeadNew(leadNew, leadActivity);
                var retVal = Json(leadId);
                return Json(leadId);
            }
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetTotalLeadsNew()
        {
            var leads = await _salesTrackerRepository.GetTotalLeadsNew();
            return Json(leads);
        }
        [HttpGet]
        public async Task<IActionResult> GetLeadbySelectedKPI(string selectedKPI)
        {
            var leads = await _salesTrackerRepository.GetLeadbySelectedKPI(selectedKPI);
            return Json(leads);
        }
        [HttpGet]
        public async Task<IActionResult> GetLeadDetailByIdNew(int LeadId)
        {
            var leadDetails = await _salesTrackerRepository.GetLeadDetailByIdAsyncNew(LeadId);
            return Json(leadDetails);
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveLeads()
        {
            //var leads = await _salesTrackerRepository.GetActivelLeads();
            var leads = await _salesTrackerRepository.GetActivelLeadsNew();
            return Json(leads);
        }

        [HttpGet]
        public async Task<IActionResult> GetConvertedLeads()
        {
            //var leads = await _salesTrackerRepository.GetActivelLeads();
            var leads = await _salesTrackerRepository.GetConvertedLeads();
            return Json(leads);
        }

        public async Task<IActionResult> GetActionableLeads()
        {
            //var leads = await _salesTrackerRepository.GetActionablelLeads();
            var leads = await _salesTrackerRepository.GetActionablelLeadsNew();
            return Json(leads);
        }
    }
}

