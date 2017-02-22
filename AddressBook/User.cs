using System;

namespace AddressBook
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime TimeAdded { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }

        public User(string firstName, string lastName, DateTime birthDay, DateTime timeAdded, string city,
                    string address, string phoneNumber, string gender, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDay;
            TimeAdded = timeAdded;
            City = city;
            Address = address;
            PhoneNumber = phoneNumber;
            Gender = gender;
            Email = email;
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
