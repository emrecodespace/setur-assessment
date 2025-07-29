using Contacts.Domain.Enums;

namespace Contacts.Domain.Entities;

public class PersonContactInfo
{
    public Guid Id { get; set; }
    public InfoType InfoType { get; set; }
    public string Content  { get; set; }
    
    public Guid PersonId { get; set; }
    public Person Person { get; set; }
}