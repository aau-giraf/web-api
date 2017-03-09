using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GirafWebApi.Contexts;
using GirafWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace GirafWebApi.Controllers
{
    [Route("[controller]")]
    public class PictogramController : Controller
    {
        public readonly GirafDbContext _context;
        public readonly UserManager<GirafUser> _userManager;

        public PictogramController(GirafDbContext context, UserManager<GirafUser> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        /// <summary>
        /// Get all public <see cref="Pictogram"/> pictograms and if authorized also his protected and private.
        /// </summary>
        /// <returns> All PUBLIC, PROTECTED and PRIVATE <see cref="Pictogram"/> pictograms available </returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var _pictograms = _context.Pictograms.Where(p => p.AccessLevel == AccessLevel.PUBLIC);
            /*if (is_auth?)
            {
                _pictograms.AddRange(await _context.Pictograms.Where(p => p.Department.members.Contains(User)));
            }*/
            return Ok(await _pictograms.ToListAsync());
        }

        /// <summary>
        /// Get the <see cref="Pictogram"/> pictogram with the specified <paramref name="id"/> id and check if the user is authorized if it is PRIVATE or PROTECTED.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> a <see cref="Pictogram"/> pictogram with a specific <paramref name="id"/> id </returns>
        [HttpGet]
        [Route("{id}/image")]
        public async Task<IActionResult> Get(int id)
        {
            var _pictogram = await _context.Pictograms.Where(p => p.AccessLevel == AccessLevel.PUBLIC && p.Key == id).ToListAsync();
            /*if(_pictogram == null && is_auth ?)
            {
                _pictogram = await _context.Pictograms.Where(p => p.Department.members.Contains(User) && p.Key == id);
            }*/
            return Ok(_pictogram.First());
        }

        /// <summary>
        /// Get the image of a <see cref="Pictogram"/> pictogram with the specified <paramref name="id"/> id and check if the user is authorized if it is PRIVATE or PROTECTED.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> an image of a <see cref="Pictogram"/> pictogram with a specific <paramref name="id"/> id </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Image(int id)
        {
            var _pictogram = await _context.Pictograms.Where(p => p.AccessLevel == AccessLevel.PUBLIC && p.Key == id).ToListAsync();
            /*if (_pictogram == null && is_auth ?)
            {
                _pictogram = await _context.Pictograms.Where(p => p.Department.members.Contains(User) && p.Key == id);
            }*/
            return Ok(_pictogram.First().Image);
        }

        /// <summary>
        /// Create a <see cref="Pictogram"/> pictogram.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreatePictogram()
        {
            Pictogram _pictogram = null;
            return Ok(_context.Pictograms.Add(_pictogram));
        }

        /// <summary>
        /// Update image of a <see cref="Pictogram"/> pictogram with an id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdatePictogramImage(int id)
        {
            return Ok();
        }

        /// <summary>
        /// Update info of a <see cref="Pictogram"/> pictogram with an id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePictogramInfo(int id)
        {
            return Ok();
        }

        /// <summary>
        /// Delete a <see cref="Pictogram"/> pictogram with an id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePictogram(int id)
        {
            return Ok();
        }
    }
}