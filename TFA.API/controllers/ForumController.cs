using Microsoft.AspNetCore.Mvc;
using TFA.API.Models;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCase.CreateTopic;
using TFA.Domain.UseCase.GetForums;

namespace TFA.API.Controllers;

[ApiController]
[Route("forums")]
public class ForumController: ControllerBase
{
    [HttpGet(Name = nameof(GetForums))]
    [ProducesResponseType(200, Type = typeof(Forum[]))]
    public async Task<IActionResult> GetForums(
        [FromServices] IGetForumsUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        var forums = await useCase.Execute(cancellationToken);
        return Ok(forums.Select(f => new Forum{
            Id = f.Id,
            Title =f.Title
        }));
    }

    [HttpPost("{forumId:guid}/topics")]
    [ProducesResponseType(403)]
    [ProducesResponseType(410)]
    [ProducesResponseType(201, Type = typeof(Topic))]
    public async Task<IActionResult> CreateTopic(
        Guid forumId,
        [FromBody] CreateTopic request,
        [FromServices] ICreateTopicUseCase useCase,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var topic = await useCase.Execute(forumId, request.Title, cancellationToken);
            return CreatedAtRoute(nameof(GetForums), new Topic{
                Id = topic.Id,
                Title = topic.Title,
                CreateAt = topic.CreatedAt
            });
        }
        catch(Exception exception)
        {
            return exception switch
            {
                IntentionManagerExtension => Forbid(),
                ForumNotFoundException => StatusCode(StatusCodes.Status410Gone),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }

}