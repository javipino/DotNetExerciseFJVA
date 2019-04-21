using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.ViewModel.Users
{
    public class UserModel
    {
        public int Id { get; set; }

        [StringLength(20)]
        public string Name { get; set; }

        public DateTime Birthdate { get; set; }
    }
}
