using System;
using System.Collections.Generic;
using System.Linq;

namespace AddressBook
{
    //Singlton Class that contains list of users 
    public sealed class AddressBook
    {
        public event EventHandler<ModifiedEventArgs> UserAdded;
        public event EventHandler<ModifiedEventArgs> UserRemoved;
        public event EventHandler<ModifiedEventArgs> DebugEvent;

        //Lazy initialization
        private static readonly Lazy<AddressBook> _addressBook = new Lazy<AddressBook>(() => new AddressBook());
        public static AddressBook Book { get { return _addressBook.Value; } }

        private List<User> _usersList;
        private AddressBook()
        {
            _usersList = new List<User>();
        }

        public void AddUser(string firstName, string lastName, DateTime birthDate, string city, string address,
            string phoneNumber, string gender, string email)
        {
            OnDebugEvent(new ModifiedEventArgs("Adding user", EventTypes.Debug));

            if ((firstName == null) || (lastName == null) || (phoneNumber == null))
                OnDebugEvent(new ModifiedEventArgs("Some of key fields is null", EventTypes.Warning));

            User tempUser = new User(firstName, lastName, birthDate, DateTime.Now, city, address, phoneNumber, gender, email);
            if (tempUser != null)
            {
                _usersList.Add(tempUser);
                OnUserAdded(new ModifiedEventArgs("New user was added", EventTypes.Info));
            }
            else
                OnDebugEvent(new ModifiedEventArgs("User was not created", EventTypes.Warning));
        }


        public void RemoveUser(string firstName, string lastName)
        {
            OnDebugEvent(new ModifiedEventArgs("Deleting user", EventTypes.Debug));

            if ((firstName == null) || (lastName == null))
                OnDebugEvent(new ModifiedEventArgs("Some of parameter is null", EventTypes.Error));

            User tempUser = _usersList.ContainsExt(firstName, lastName);
            if (tempUser != null)
            {
                _usersList.Remove(tempUser);
                OnUserRemoved(new ModifiedEventArgs("User was removed", EventTypes.Info));
            }
            else
                OnDebugEvent(new ModifiedEventArgs("User does not exist", EventTypes.Warning));
        }

        public void RemoveUser(string phoneNumber)
        {
            OnDebugEvent(new ModifiedEventArgs("Deleting user", EventTypes.Debug));

            if (phoneNumber == null)
                OnDebugEvent(new ModifiedEventArgs("Some of parameter is null", EventTypes.Error));

            User tempUser = _usersList.ContainsExt(phoneNumber);
            if (tempUser != null)
            {
                _usersList.Remove(tempUser);
                OnUserRemoved(new ModifiedEventArgs("User was removed", EventTypes.Info));
            }
            else
                OnDebugEvent(new ModifiedEventArgs("User does not exist", EventTypes.Warning));
        }

        //Event invocation methods
        private void OnUserAdded(ModifiedEventArgs e)
        {
            EventHandler<ModifiedEventArgs> handler = UserAdded;

            if (handler != null)
            {
                e.Message += string.Format("; {0}", DateTime.Now.ToString());
                handler(this, e);
            }
        }

        private void OnUserRemoved(ModifiedEventArgs e)
        {
            EventHandler<ModifiedEventArgs> handler = UserRemoved;

            if (handler != null)
            {
                e.Message += string.Format("; {0}", DateTime.Now.ToString());
                handler(this, e);
            }
        }

        private void OnDebugEvent(ModifiedEventArgs e)
        {
            EventHandler<ModifiedEventArgs> handler = DebugEvent;

            if (handler != null)
            {
                e.Message += string.Format("; {0}", DateTime.Now.ToString());
                handler(this, e);
            }
        }

        //1. Return Gmail's users
        public IEnumerable<User> GmailUsers()
        {
            return _usersList.Where(user => user.Email.EndsWith("@gmail.com")).OrderBy(user => user.LastName).Select(user => user);
        }

        //Enumarator for using in extension method to do second task
        public IEnumerator<User> GetEnumarator()
        {
            return _usersList.GetEnumerator();
        } 

        //3. Female users that were added during last 10 days
        public IEnumerable<User> NewFemaleUsers()
        {
            return from user in _usersList
                   where user.Gender == "Female"
                   where DateTime.Now.Year - user.TimeAdded.Year == 0
                   where DateTime.Now.DayOfYear - user.TimeAdded.DayOfYear <= 10
                   orderby user.LastName
                   select user;
        }

        //4. Users that were born in Jan and have address and phone number
        public IEnumerable<User> BornInJanWithAddress()
        {
            return _usersList.Where(user => (user.BirthDate.Month == 1) &&
            (user.Address != null) && (user.PhoneNumber != null))
                             .OrderByDescending(user => user.LastName)
                             .Select(user => user);
        }

        //5. Return Dictionary with gender key and IEnumerable with users
        public Dictionary<string, IEnumerable<User>> GenderSortedUsers()
        {
            var dict = new Dictionary<string, IEnumerable<User>>();

            dict["man"] = _usersList.Where(user => user.Gender == "Male").Select(user => user);
            dict["woman"] = _usersList.Where(user => user.Gender == "Female").Select(user => user);
            return dict;
        }

        //6. Return users from start to finish by given condition
        public IEnumerable<User> TakeByPredicate(Func<User, bool> predicate, int start, int finish)
        {
            return _usersList.Where(predicate)
                .OrderBy(user => user.LastName)
                .Skip(start)
                .Take(_usersList.Capacity - start - finish);
        }

        //7. Users from city X whose birthday is today
        public IEnumerable<User> BirthdayGuysFromCityX(string city)
        {
            return from user in _usersList
                   where user.City == city
                   where user.BirthDate.Month == DateTime.Now.Month
                   where user.BirthDate.Day == DateTime.Now.Day
                   select user;
        }

        //8. Useres list for sending letter
        public IEnumerable<User> BirthdayGuys()
        {
            return from user in _usersList
                   where user.BirthDate.Month == DateTime.Now.Month
                   where user.BirthDate.Day == DateTime.Now.Day
                   select user;
        }
    }

    //Enums to describe type of event for logging
    public enum EventTypes { Debug, Info, Warning, Error}
    //Custom EventsArgs that contains message and type of event
    public class ModifiedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public EventTypes Type { get; set; }

        public ModifiedEventArgs(string s, EventTypes t)
        {
            Message = s;
            Type = t;    
        }
    }
}
