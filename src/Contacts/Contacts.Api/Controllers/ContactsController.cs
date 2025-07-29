using Contacts.Application.Dtos;
using Contacts.Application.Interfaces;
using Contacts.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Api.Controllers;

/// <summary>
/// API endpoints for managing persons and their contact information in the directory.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContactsController(IContactService contactService) : ControllerBase
{
    /// <summary>
    /// Retrieves a list of all persons in the directory.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// Returns 200 OK with a list of all persons.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetAllPersons(CancellationToken ct)
    {
        var result = await contactService.GetAllPersonsAsync(ct);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Creates a new person in the directory.
    /// </summary>
    /// <param name="req">Person data to create.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// Returns 200 OK with the created person's ID if successful, or 400 Bad Request if the input is invalid.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> CreatePerson([FromBody] CreatePersonRequest req, CancellationToken ct)
    {
        var result = await contactService.CreatePersonAsync(req, ct);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Deletes a person from the directory.
    /// </summary>
    /// <param name="id">The unique identifier of the person to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// Returns 200 OK if the person was deleted successfully, or 404 Not Found if the person does not exist.
    /// </returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePerson(Guid id, CancellationToken ct)
    {
        var result = await contactService.DeletePersonAsync(id, ct);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Gets detailed information about a person, including all associated contact infos.
    /// </summary>
    /// <param name="personId">Unique identifier of the person.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// Returns detailed information about the person, including contact info records (such as phone, email, location).
    /// </returns>
    [HttpGet("{personId:guid}/contact-infos")]
    public async Task<IActionResult> GetPersonWithContactInfosAsync(Guid personId, CancellationToken ct)
    {
        var result = await contactService.GetPersonWithContactInfosAsync(personId, ct);
        return this.ToActionResult(result);
    }
    
    /// <summary>
    /// Adds a new contact info to a person.
    /// </summary>
    /// <param name="personId">Person's unique ID.</param>
    /// <param name="req">
    /// <b>type:</b> Contact info type. Valid values: Phone, Email, Location.<br/>
    /// <b>content:</b> Value for the selected type (e.g., phone number, email address, or location name).
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns the created contact info’s ID.</returns>
    [HttpPost("{personId:guid}/contact-infos")]
    public async Task<IActionResult> AddContactInfo(Guid personId, [FromBody] AddContactInfoRequest req, CancellationToken ct)
    {
        var result = await contactService.AddContactInfoAsync(personId, req, ct);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Removes a specific contact info from a person.
    /// </summary>
    /// <param name="personId">Unique identifier of the person.</param>
    /// <param name="contactInfoId">Unique identifier of the contact info to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns <c>200 OK</c> if successful, or appropriate error if not found.</returns>
    [HttpDelete("{personId:guid}/contact-infos/{contactInfoId:guid}")]
    public async Task<IActionResult> RemoveContactInfo(Guid personId, Guid contactInfoId, CancellationToken ct)
    {
        var result = await contactService.RemoveContactInfoAsync(personId, contactInfoId, ct);
        return this.ToActionResult(result);
    }
}