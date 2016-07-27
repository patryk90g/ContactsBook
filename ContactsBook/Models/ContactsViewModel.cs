using ContactsBook.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsBook.Models
{
    public class ContactsViewModel
    {
        public PagedList.IPagedList<Contact> Contacts { get; set; }

        
        public int NumberOfContacts { get; set; }
        public int NumberOfFriends { get; set; }

    }
}


