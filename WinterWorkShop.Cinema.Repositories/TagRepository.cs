﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WinterWorkShop.Cinema.Data;
using WinterWorkShop.Cinema.Data.Entities;

namespace WinterWorkShop.Cinema.Repositories
{
    public interface ITagRepository : IRepository<Tag>
    {
    }
    public class TagRepository : ITagRepository
    {
        private readonly CinemaContext _cinemaContext;

        public TagRepository(CinemaContext cinemaContext)
        {
            _cinemaContext = cinemaContext;
        }

        public Tag Delete(object id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tag>> GetAll()
        {
            //return await _cinemaContext.Movies.ToListAsync();

            return await _cinemaContext.Tags.ToListAsync();

        }

        public Task<Tag> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Tag Insert(Tag obj)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public Tag Update(Tag obj)
        {
            throw new NotImplementedException();
        }
    }
}
