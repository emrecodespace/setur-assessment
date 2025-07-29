namespace Contacts.Domain.Entities;

public class Person
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Company { get; set; }

    public virtual ICollection<PersonContactInfo> ContactInfos { get; set; }
}