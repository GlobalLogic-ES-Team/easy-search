using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCDataImport
{
    public class Name
    {
        public string title { get; set; }
        public string first { get; set; }
        public string last { get; set; }
    }

    public class Location
    {
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public int postcode { get; set; }
    }

    public class Id
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Person
    {
        public string gender { get; set; }
        public Name name { get; set; }
        public Location location { get; set; }
        public string email { get; set; }
        public string dob { get; set; }
        public string cell { get; set; }
        public Id id { get; set; }
        public decimal Salary { get; set; }
        public string Interests { get; set; }
    }

    public class Info
    {
        public string seed { get; set; }
        public int results { get; set; }
        public int page { get; set; }
        public string version { get; set; }
    }

    public class PersonResponse
    {
        public IList<Person> results { get; set; }
        public Info info { get; set; }
    }

    public class PersonRecord
    {
        public string Gender { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int PostCode { get; set; }
        public string Email { get; set; }
        public string Dob { get; set; }
        public string Cell { get; set; }
        public string IdType { get; set; }
        public string Id { get; set; }
        public decimal Salary { get; set; }
        public string Interests { get; set; }
        public string PersonData { get; set; }
    }

    public class SampleData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public string Salary { get; set; }
        public string DateOfJoining { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string MaritalStatus { get; set; }
        public string Interests { get; set; }
    }

}
