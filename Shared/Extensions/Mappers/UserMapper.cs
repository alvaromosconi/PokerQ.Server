using Entities;
using Shared.DataTransferObjects.Requests;
using Shared.DataTransferObjects.Resources;

namespace Shared.Extensions.Mappers;

public static class UserMapper
{
    public static UserResource ToDTO(this User user)
    {
        return new UserResource(user.Id,
                                user.UserName,
                                user.Name,
                                user.Email);
    }

    public static User ToEntity(this UserResource resource)
    {
        return new User
        {
            Id = resource.Id,
            Name = resource.Name
        };
    }

    public static User ToEntity(this UserRegistrationRequest request)
    {
        return new User
        {
            Name = request.Name,
            Email = request.Email,
            UserName = request.UserName
        };
    }
}
