using Api.MedicalManagement.Database;
using Api.MedicalManagement.Enterprise;
using Library.MedicalManagement.Data;
using Library.MedicalManagement.DTO;
using Library.MedicalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace Api.MedicalManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(ILogger<PatientsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<PatientDTO> Get()
        {
            return new PatientsEC().GetBlogs();
        }

        [HttpGet("{id}")]
        public PatientDTO? GetById(int id)
        {
            return new PatientsEC().GetById(id);
        }

        [HttpDelete("{id}")]
        public PatientDTO? Delete(int id)
        {
            return new PatientsEC().Delete(id);
        }

        [HttpPost]
        public PatientDTO? AddOrUpdate([FromBody] PatientDTO pat)
        {
            return new PatientsEC().AddOrUpdate(pat);
        }

        [HttpPost("Search")]
        public IEnumerable<PatientDTO?> Search([FromBody] QueryRequest query)
        {
            return new PatientsEC().Search(query.Content);
        }
    }
}