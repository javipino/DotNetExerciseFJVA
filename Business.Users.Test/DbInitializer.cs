using Integration.DataInfraestructure;
using Shared.Domain.Tables;
using System;

namespace Business.Users.Test
{
    public class DbInitializer
    {
        /// <summary>
        /// Prepare Db to Run text
        /// </summary>
        /// <param name="repo">repository reference</param>
        /// <param name="unitOfWork">unit of work reference</param>
        public void Seed(IRepository<User> repo, IUnitOfWork unitOfWork)
        {
            repo.Truncate(new User());

            repo.Insert(new User() { Name = "Test1", Birthdate = DateTime.Parse("30/11/1992") });
            repo.Insert(new User() { Name = "Test2", Birthdate = DateTime.Parse("30/11/1999") });

            unitOfWork.SaveChanges();
            return;
        }
    }
}
