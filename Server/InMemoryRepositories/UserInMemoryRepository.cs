using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository :IUserRepository
{
    private List<User> users = new List<User>();
    
    public Task<User> AddAsync(User user)
    {
        user.user_id = users.Any() 
            ? users.Max(u => u.user_id) + 1
            : 1;
        users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        User? existingUser = users.SingleOrDefault(u => u.user_id == user.user_id);
        if (existingUser is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{user.user_id}' not found");
        }

        users.Remove(existingUser);
        users.Add(user);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        User? userToRemove = users.SingleOrDefault(u => u.user_id == id);
        if (userToRemove is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        users.Remove(userToRemove);
        return Task.CompletedTask;
    }
/*
    public Task<Post> GetSingleAsync(int id)
    {
        Post? postToGet = posts.SingleOrDefault(p => p.post_id == id);
        if (postToGet is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        return Task.FromResult(post);
    }
*/
    public IQueryable<User> GetManyAsync()
    {
        return users.AsQueryable();
    }
}