using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using TodoApiConsumeApp.Data.Context;
using TodoApiConsumeApp.Data.DTO.Todo;
using TodoApiConsumeApp.Data.Entities;

namespace TodoApiConsumeApp.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        #region Bağımlılıklar

        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoController( IMapper mapper, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
        }

        #endregion

        #region Tüm Todoları Getir

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Tüm Todoları Getirir.", Description = "Bu Endpointi çalıştırdığınızda tüm Todoları getirecektik.")]
        [SwaggerResponse(200, "Tüm Todolar Geldi")]
        [SwaggerResponse(404, "Todo Bulunamadı.")]
        public async Task<ActionResult<GetTodoDto>> GetAllTodo()
        {
            var getAllTodo = await _context.Todos.ToListAsync();
            if (getAllTodo == null)
            {
                return NotFound("Herhangi bir Todo Bulunamadı");
            }
            var result = _mapper.Map<GetTodoDto[]>(getAllTodo);
            return Ok(result);
            
        }
        #endregion
        
        #region Kullanıcıya Ait Olan Tüm Todoları Getir.

        [HttpGet("[action]")]
        [SwaggerOperation(Summary = "Kullanıcıya ait Tüm Todolar Geldi.", Description = "Bu Endpointi çalıştırdığınızda kullanıcıya ait tüm todolar gelecektir..")]
        [SwaggerResponse(200, "Kullanıcıya ait Tüm Todolar Geldi.")]
        [SwaggerResponse(404, "Kullanıcıya ait Tüm Todolar Bulunamadı.")]
        [SwaggerResponse(404, "Kullanıcıya Bulunamadı.")]
        
        public async Task<ActionResult<GetTodoDto[]>> GetUserAllTodo(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var todoArray = await _context.Todos
                .Include(x => x.User)
                .Where(x => x.UserId == user.Id)
                .ToArrayAsync();

            if (todoArray.Length == 0)
            {
                return NotFound("Todo bulunamadı.");
            }

            var result = _mapper.Map<GetTodoDto[]>(todoArray);

            return Ok(result);
        }

        #endregion

        #region Todo Ekle

        [HttpPost("[action]")]
        [SwaggerOperation(Summary = "Todo Ekle", Description = "Bu Endpointi çalıştırdığınızda Todo Ekleyebilirsiniz.")]
        [SwaggerResponse(200, "Todo Eklendi.")]
        [SwaggerResponse(400, "Kullanıcı Veriyi Doğru Girmedi.")]
        [SwaggerResponse(404, "Kullanıcıya Bulunamadı.")]
        public ActionResult<AddTodoDto> AddTodo([FromBody] AddTodoDto todoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Lütfen Bütün verileri Doğru Giriniz.");
            }
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound("Kullanıcı Bulunamadı.");
            }
            var addTodo = _mapper.Map<Todo>(todoDto);
            addTodo.UserId = userId;
            addTodo.CreatedDate = DateTime.Now;
            addTodo.IsCompleted = false;
            _context.Todos.Add(addTodo);
            _context.SaveChanges();
            var result = _mapper.Map<AddTodoDto>(addTodo);
            return Ok(result);
        }

        #endregion

        #region Todo Güncelle

        [HttpPut("[action]")]
        [SwaggerOperation(Summary = "Todo Güncelle", Description = "Bu Endpointi çalıştırdığınızda Todo Güncelleyebilirsiniz.")]
        [SwaggerResponse(200, "Todo Güncellendi.")]
        [SwaggerResponse(400, "Kullanıcı Veriyi Doğru Girmedi.")]
        [SwaggerResponse(401, "Kullanıcının Bu Veriyi Güncelleme Yetkisi Yok.")]
        [SwaggerResponse(404, "Verilen Id'ye göre veri bulunamadı.")]
        public async Task<ActionResult<UpdateTodoDto>> UpdateTodo([FromBody] UpdateTodoDto todoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Kullanıcı Veriyi Doğru Girmedi.");
            }
            var userId = _userManager.GetUserId(User);
            
            var getTodo = await _context.Todos.FindAsync(todoDto.Id);
            if (getTodo == null)
            {
                return NotFound("Verilen Id'ye göre veri bulunamadı.");
            }

            if (getTodo.UserId != userId)
            {
                return Unauthorized("Bu veri size ait değil. Güncelleme yapamazsınız.");
            }
            getTodo.TaskName = todoDto.TaskName;
            getTodo.UpdatedDate = DateTime.Now;
            _context.Todos.Update(getTodo);
            _context.SaveChanges();
            var result = _mapper.Map<UpdateTodoDto>(getTodo);
            return Ok(result);
        }

        #endregion

        #region Todo Sİl
        [SwaggerOperation(Summary = "Todo Sil", Description = "Bu Endpointi çalıştırdığınızda Todo Silebilirsiniz..")]
        [SwaggerResponse(200, "Todo Silindi.")]
        [SwaggerResponse(400, "Kullanıcı Veriyi Doğru Girmedi.")]
        [SwaggerResponse(401, "Kullanıcının Bu Veriyi Silme Yetkisi Yok.")]
        [SwaggerResponse(404, "Verilen Id'ye göre veri bulunamadı.")]
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Kullanıcı Veriyi Doğru Girmedi.");
            }
            var userId = _userManager.GetUserId(User);
            var getTodo = await _context.Todos.FindAsync(id);
            if (getTodo == null)
            {
                return NotFound("Girilen Id'e ait veri bulunamadı.");
            }

            if (getTodo.UserId != userId)
            {
                return Unauthorized("Bu Liste Size ait değil. Silme işlemi gerçekleştiremezsiniz.");
            }

            _context.Todos.Remove(getTodo);
            _context.SaveChanges();
            return Ok("Silme İşlemi Gerçekleştirildi.");
        }

        #endregion

        #region Todo Tamamlandı

        [HttpPut("[action]")]
        [SwaggerOperation(Summary = "Todo Görevi Tamamlama", Description = "Bu Endpointi çalıştırdığınızda Todonuzun Görevini Tamamlayabilirsiniz.")]
        [SwaggerResponse(200, "Todo Görevi Tamamlandı.")]
        [SwaggerResponse(400, "Kullanıcı Veriyi Doğru Girmedi.")]
        [SwaggerResponse(401, "Todo size ait değil. Değişiklik Yapamazsınız.")]
        [SwaggerResponse(404, "Kullanıcıya Bulunamadı.")]
        [SwaggerResponse(404, "Girilen Id'e ait veri bulunamadı.")]
        public async Task<IActionResult> TodoIsCompleted(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Kullanıcı Veriyi Doğru Girmedi.");
            }
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound("Kullanıcı Bulunamadı.");
            }
            var getTodo = await _context.Todos.FindAsync(id);

            if (getTodo == null)
            {
                return NotFound("Girilen Id'e ait veri bulunamadı.");
            }

            if (getTodo.UserId != userId)
            {
                return Unauthorized("Todo size ait değil. Değişiklik Yapamazsınız.");
            }
            getTodo.IsCompleted = true;
            getTodo.UpdatedDate = DateTime.Now;
            _context.Todos.Update(getTodo);
            _context.SaveChanges();
            return Ok("Todo Tamamlandı.");
        }

        #endregion

        #region Todo Tamamlanmadı.

        [HttpPut("[action]")]
        [SwaggerOperation(Summary = "Todo Görevi Tamamlanmadı", Description = "Bu Endpointi çalıştırdığınızda Todonuzun Görevini Tamamlanmadı olarak değişir..")]
        [SwaggerResponse(200, "Todo Görevi Tamamlanmadı.")]
        [SwaggerResponse(400, "Kullanıcı Veriyi Doğru Girmedi.")]
        [SwaggerResponse(401, "Todo size ait değil. Değişiklik Yapamazsınız.")]
        [SwaggerResponse(404, "Kullanıcıya Bulunamadı.")]
        [SwaggerResponse(404, "Girilen Id'e ait veri bulunamadı.")]
        public async Task<IActionResult> TodoIsNotCompleted(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Lütfen Sayısal bir değer giriniz.");
            }
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound("Kullanıcı Bulunamadı.");
            }
            var getTodo = await _context.Todos.FindAsync(id);

            if (getTodo == null)
            {
                return NotFound("Girilen Id'e ait veri bulunamad.");
            }

            if (getTodo.UserId != userId)
            {
                return Unauthorized("Todo size ait değil. Değişiklik Yapamazsınız.");
            }
            getTodo.IsCompleted = false;
            getTodo.UpdatedDate = DateTime.Now;
            _context.Todos.Update(getTodo);
            _context.SaveChanges();
            return Ok("Todo Tamamlanmadı olarak güncellendi.");
        }

        #endregion
        
    }
}
