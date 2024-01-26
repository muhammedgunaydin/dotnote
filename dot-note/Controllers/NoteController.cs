using dot_note.DbContext;
using dot_note.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;

namespace dot_note.Controllers
{
    public class NoteController : Controller
    {
        private readonly MongoDbContext _context;

        public NoteController(MongoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userNotes = _context.Notes.AsQueryable().Where(n => n.UserId == userId).ToList();
            return View(userNotes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Note note)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (userId == null)
            {
                ModelState.AddModelError("UserId", "User not authenticated.");
            }
            else
            {
                note.UserId = userId;
                note.CreatedAt = DateTime.Now;
                note.ModifiedAt = DateTime.Now;

                _context.Notes.InsertOne(note);
                return RedirectToAction("Index");
            }
            return View(note);
        }

        public IActionResult Edit(string id)
        {
            var note = _context.Notes.Find(i => i.Id == id).FirstOrDefault();
            if (note == null)
            {
                return NotFound();
            }
            return View(note);
        }

        [HttpPost]
        public IActionResult Edit(string id, Note updatedNote)
        {
            if (id != updatedNote.Id)
            {
                return NotFound();
            }
            updatedNote.ModifiedAt = DateTime.Now;

            var filter = Builders<Note>.Filter.Eq(i => i.Id, id);
            var update = Builders<Note>.Update
                .Set(i => i.Title, updatedNote.Title)
                .Set(i => i.Text, updatedNote.Text)
                .Set(i => i.ModifiedAt, updatedNote.ModifiedAt);

            _context.Notes.UpdateOne(filter, update);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var note = _context.Notes.Find(i => i.Id == id).FirstOrDefault();
            if (note == null)
            {
                return NotFound();
            }
            return View(note);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirm(string id)
        {
            var filter = Builders<Note>.Filter.Eq(i => i.Id, id);
            _context.Notes.DeleteOne(filter);

            return RedirectToAction("Index");
        }
    }
}
