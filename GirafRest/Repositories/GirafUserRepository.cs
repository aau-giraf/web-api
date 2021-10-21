using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GirafRest.Extensions;
using System.Threading.Tasks;
using GirafRest.Models.DTOs;
using System;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using GirafRest.Interfaces;

namespace GirafRest.Repositories
{
    public class GirafUserRepository : Repository<GirafUser>, IGirafUserRepository
    {
        private readonly IGirafService _giraf;
        private readonly HttpContext HttpContext;
        public GirafUserRepository(GirafDbContext context) : base(context)
        {
        }

        public GirafUser GetUserWithId(string id)
        {
           
           return Context.Users.FirstOrDefault(u => u.Id == id);
        }

        public GirafUser GetSettingsWithId(string id)
        {
            return Context.Users.Include(u => u.Settings).ThenInclude(w => w.WeekDayColors).FirstOrDefault(u => u.Id == id);
        }

        
        public void Update(GirafUser user)
        {
            Context.Users.Update(user);
        }
        public Task<int> SaveChangesAsync()
        {
             return Context.SaveChangesAsync();
        }
        /// <summary>
        /// Reads an image from the current request's body and return it as a byte array.
        /// </summary>
        /// <param name="bodyStream">A byte-stream from the body of the request.</param>
        /// <returns>The image found in the request represented as a byte array.</returns>
        public async Task<byte[]> ReadRequestImage(Stream bodyStream)
        {
            byte[] image;
            using (var imageStream = new MemoryStream())
            {
                await bodyStream.CopyToAsync(imageStream);

                try      //I assume this will always throw, but I dare not remove it, because why would it be here?
                {
                    await bodyStream.FlushAsync();
                }
                catch (NotSupportedException)
                {
                }

                image = imageStream.ToArray();
            }

            return image;
        }

        public GirafUser CheckIfUserExists(string id)
        {
            //Attempt to find the target user and check that he exists
            return Context.Users.Include(u => u.Resources).ThenInclude(dr => dr.Pictogram).FirstOrDefault(u => u.Id == id);
        }

        
        //Find the resource and check that it actually does exist - also verify that the resource is private
        public Task<Pictogram> FindResource(ResourceIdDTO resourceIdDTO)
        {
            return Context.Pictograms.Where(pf => pf.Id == resourceIdDTO.Id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Method for loading user from context, but including no fields. No reference types will be available.
        /// </summary>
        /// <param name="principal">The security claim - i.e. the information about the currently authenticated user.</param>
        /// <returns>A <see cref="GirafUser"/> without any related data.</returns>

        public Task<GirafUser> LoadBasicUserDataAsync(ClaimsPrincipal principal) 
        {
            
            if (principal == null) return null;
            var usr = ( _userManager.GetUserAsync(principal));
            if (usr == null) return null;
            return  Context.Users
                //Get user by ID from database
                .Where(u => u.Id == usr.Id)
                //And return it
                .FirstOrDefaultAsync();
        }

        
        public Task<bool> CheckPrivateOwnership(Pictogram resource, GirafUser curUsr)
        {
            return _giraf.CheckPrivateOwnership(resource, curUsr);
        }

        
        
       

        
        //Check if the caller owns the resource
        public GirafUser CheckIfCallerOwnsResource(string id)
        {
            return Context.Users.Include(r => r.Resources).ThenInclude(dr => dr.Pictogram).FirstOrDefault(u => u.Id == id);
        }
       
        //Fetch the resource with the given id, check that it exists.
        public Task<Pictogram> FetchResourceWithId(ResourceIdDTO resourceIdDTO)
        {
            return Context.Pictograms.Where(f => f.Id == resourceIdDTO.Id).FirstOrDefaultAsync();
        }
        //Fetch the relationship from the database and check that it exists
        

        public Task<UserResource> FetchRelationshipFromDb(Pictogram resource, GirafUser user)
        {
            return Context.UserResources.Where(ur => ur.PictogramKey == resource.Id && ur.OtherKey == user.Id).FirstOrDefaultAsync();
        }

        //_giraf._context.UserResources.Remove(relationship);

        public void Remove(UserResource relationship)
        {
            Context.UserResources.Remove(relationship);
        }

        public GirafUser GetCitizensWithId(string id)
        {
            return Context.Users.Include(u => u.Citizens).FirstOrDefault(u => u.Id == id);
        }

        public GirafUser GetFirstCitizen(GuardianRelation citizen)
        {
            return Context.Users.FirstOrDefault(u => u.Id == citizen.CitizenId);
        }

        public GirafUser GetGuardianWithId(string id)
        {
            return Context.Users.Include(u => u.Guardians).FirstOrDefault(u => u.Id == id);
        }

       
        public GirafUser GetGuardian(GuardianRelation guardian)
        {
            return Context.Users.FirstOrDefault(u => u.Id == guardian.GuardianId);
        }

        
        public GirafUser GetCitizenRelationship(string citizenId)
        {
           return Context.Users.Include(u => u.Guardians).FirstOrDefault(u => u.Id == citizenId);
        }
       
        public GirafUser GetGuardianRelationship(string id)
        {
            return Context.Users.FirstOrDefault(u => u.Id == id);
        }

       
       public GirafUser GetUserSettingsByWeekDayColor(string id)
        {
            return Context.Users.Include(u => u.Settings).ThenInclude(w => w.WeekDayColors).FirstOrDefault(u => u.Id == id);
        }














    }
}