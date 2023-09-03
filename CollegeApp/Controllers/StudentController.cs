using CollegeApp.DTO;
using CollegeApp.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CollegeApp.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        [HttpGet]
        [Route("All", Name = "GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<StudentDto>> GetStudents()
        {
            var students = CollegeRepository.Students.Select(s => new StudentDto()
            {
                Id = s.Id,
                StudentName = s.StudentName,
                Address = s.Address,
                Email = s.Email
            });
            
            //Ok - 200 - Success
            return Ok(students);
        }

        [HttpGet("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDto> GetStudentById(int id)
        {
            //BadRequest - 400 - Badrequest - client error
            if (id <= 0)
                return BadRequest();
            
            var student = CollegeRepository.Students.Where(n => n.Id == id).FirstOrDefault();
            // NotFound - 404 - NotFound - client error
            if (student == null)
                return NotFound($"The student with id {id} not found");

            var studentDto = new StudentDto()
            {
                Id = student.Id,
                StudentName = student.StudentName,
                Address = student.Address,
                Email = student.Email
            };

            //Ok - 200 - Success
            return Ok(studentDto);
        }

        [HttpGet("{name:alpha}", Name = "GetStudentByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDto> GetStudentByName(string name)
        {
            //BadRequest - 400 - Badrequest - client error
            if (string.IsNullOrEmpty(name))
                return BadRequest();

            var student = CollegeRepository.Students.Where(n => n.StudentName == name).FirstOrDefault();
            // NotFound - 404 - NotFound - client error
            if (student == null)
                return NotFound($"The student with name {name} not found");
            
            var studentDto = new StudentDto()
            {
                Id = student.Id,
                StudentName = student.StudentName,
                Address = student.Address,
                Email = student.Email
            };

            //Ok - 200 - Success
            return Ok(studentDto);
        }

        [HttpPost]
        [HttpGet("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //api/student/create
        public ActionResult<StudentDto> CreateStudent([FromBody] StudentDto model)
        {
            if (ModelState.IsValid)
                return BadRequest(ModelState);

            if(model == null)
                return BadRequest();


            if (model.AdmissionDate < DateTime.Now) 
            {
                //1. Directly adding error message to modelstate
                //2. Using custom attribute
                ModelState.AddModelError("AdmissionDate Error", "Adminssion date must be greated than or equal to todays date");
                return BadRequest(ModelState);
            }
            
            int newID = CollegeRepository.Students.LastOrDefault().Id + 1;
            Student student = new Student
            {
                Id = newID,
                StudentName = model.StudentName,
                Address = model.Address,
                Email = model.Email
            };

            CollegeRepository.Students.Add(student);

            model.Id = student.Id;
            //Status - 201
            return CreatedAtRoute("GetStudentById", new { id = model.Id }, model);

        }


        [HttpPatch("{id:int}/UpdatePartial")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //api/student/1/updatepartial
        public ActionResult<StudentDto> UpdateStudentPartial(int id, [FromBody] JsonPatchDocument<StudentDto> patchDocument) 
        {
            if (patchDocument == null || id <= 0)
                BadRequest();

            var existingStudent = CollegeRepository.Students.Where(s => s.Id == id).FirstOrDefault();

            if(existingStudent == null)
                return NotFound();

            var studentDto = new StudentDto
            {
                Id = existingStudent.Id,
                StudentName = existingStudent.StudentName,
                Address = existingStudent.Address,
                Email = existingStudent.Email
            };

            patchDocument.ApplyTo(studentDto, ModelState);
            if(ModelState.IsValid)
                return BadRequest(ModelState);

            existingStudent.StudentName = studentDto.StudentName;
            existingStudent.Address = studentDto.Address;
            existingStudent.Email   = studentDto.Email;

            return NoContent();
        
        }


        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //api/student/update
        public ActionResult<StudentDto> UpdateStudent([FromBody] StudentDto model)
        {
            if (model == null || model.Id <= 0)
                BadRequest();

            var existingStudent = CollegeRepository.Students.Where(s => s.Id == model.Id).FirstOrDefault();

            if (existingStudent == null)
                return NotFound();

            existingStudent.StudentName = model.StudentName;
            existingStudent.Address = model.Address;
            existingStudent.Email = model.Email;

            return NoContent();

        }

        [HttpDelete("{id:int}", Name = "DeleteStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<bool> DeleteStudentById(int id)
        {

            //BadRequest - 400 - Badrequest - client error
            if (id <= 0)
                return BadRequest();

            var student = CollegeRepository.Students.Where(n => n.Id == id).FirstOrDefault();
            // NotFound - 404 - NotFound - client error
            if (student == null)
                return NotFound($"The student with id {id} not found");

            CollegeRepository.Students.Remove(student);

            return Ok(student);
        }
    }
}
