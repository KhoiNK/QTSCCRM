using APIProject.Data.Infrastructure;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Data.Repositories
{
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        public CustomerRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public Customer GetByContact(int contactID)
        {
            var foundContact = this.DbContext.Contacts.Where(c => c.ID == contactID).SingleOrDefault();
            if(foundContact != null)
            {
                return foundContact.Customer;
            }
            return null;
        }
    }

    public interface ICustomerRepository : IRepository<Customer>
    {
        Customer GetByContact(int contactID);
    }
}
