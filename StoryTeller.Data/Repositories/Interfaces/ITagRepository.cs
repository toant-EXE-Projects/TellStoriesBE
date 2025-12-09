using StoryTeller.Data.Base;
using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Repositories.Interfaces
{
    public interface ITagRepository : IGenericRepository<Tag>
    {
        Task<Tag?> GetByNameAsync(string name);
        Task<Tag?> GetBySlugAsync(string slug);
    }
}
