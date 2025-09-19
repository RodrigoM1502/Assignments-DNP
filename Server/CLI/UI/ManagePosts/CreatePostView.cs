﻿using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    private readonly IPostRepository postRepository;

    public CreatePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task StartAsync(int user_id)
    {
        Console.WriteLine("Enter a title for the post:");
        string? title = Console.ReadLine();
        Console.WriteLine("Enter a body for the post:");
        string? body = Console.ReadLine();
        await AddPostAsync(title, body, user_id);
        await Task.CompletedTask;
    }

    public async Task<Post> AddPostAsync(string title, string body, int user_id)
    {
        Post post = new()
        {
            body = body,
            title = title,
            user_id = user_id
        };
        var created = await postRepository.AddAsync(post);
        return await Task.FromResult(created);
    }
}