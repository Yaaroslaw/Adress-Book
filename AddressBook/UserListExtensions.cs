using System;
using System.Collections.Generic;

namespace AddressBook
{
    //Extensions for List<User> type to search implementations in collection
    static public class UserListExtensions
    {
        public static User ContainsExt(this List<User> usersList, string firstName, string lastName)
        {
            if (usersList == null)
                return null;

            foreach (User user in usersList)
                if ((user.FirstName == firstName) && (user.LastName == lastName))
                    return user;
            return null;
        }

        public static User ContainsExt(this List<User> usersList, string phoneNumber)
        {
            if (usersList == null)
                return null;

            foreach (User user in usersList)
                if (user.PhoneNumber == phoneNumber)
                    return user;
            return null;
        }

        static public IEnumerable<User> AdultUsersFromKiev(this AddressBook addressBook)
        {
            var enumarator = addressBook.GetEnumarator();
            User current;
            while (enumarator.MoveNext())
            {
                current = enumarator.Current;
                if ((DateTime.Now.Year - current.BirthDate.Year > 18) && (current.City == "Kyiv"))
                {
                    //current.FirstName.ToUpper();
                    yield return current;
                }
            }
        } 

    }
}
