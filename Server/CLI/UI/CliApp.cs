using CLI.UI.ManagePosts;
using CLI.UI.ManageUsers;
using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;

    public CliApp(IUserRepository userRepository, ICommentRepository commentRepository, IPostRepository postRepository)
    {
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        AddDummyData();
    }

    public async Task StartAsync()
    {
        string? userInput = "";
        Console.WriteLine("Select an option:");
        Console.WriteLine("user: Register as User");
        Console.WriteLine("sudo: Register as SuperUser");
        Console.WriteLine("Any other: Exit");
        Console.WriteLine();//spacing on windows? I don't know why, but it works just fine on linux #ItWorksOnMyMachine
        userInput = Console.ReadLine();
        switch (userInput)
        {
            case "user":
                // Register as User
                Console.WriteLine("Registering as User...");
                break;
            case "sudo":
                // Register as SuperUser
                Console.WriteLine("Registering as SuperUser...");
                break;
            default:
                // Exit
                Console.WriteLine("Exiting...");
                return;
        }
        Console.WriteLine("Enter username:");
        string? userName = Console.ReadLine();
        Console.WriteLine("Enter password");
        string? password = Console.ReadLine();
        User user = new()
        {
            username = userName,
            password = password
        };
        User? currentUser = await userRepository.AddAsync(user);
        switch (userInput)
        {
            case "user":
                await HandleUserAsync(currentUser.user_id);
                break;
            case "sudo":
                await SudoHandleAsync(currentUser.user_id);
                break;
        }

        await Task.CompletedTask;
    }

    private async Task HandleUserAsync(int currentUserId)
    {
        CreatePostView postView = new(postRepository);
        ListPostView listPostView = new(postRepository);
        ManagePostsView managePostsView = new(postRepository);
        SinglePostView singlePostView = new(postRepository, commentRepository);
        CreateCommentView commentView = new(commentRepository);
        ManageUsersView manageUsersView = new ManageUsersView(userRepository);
        int userInput = 0;
        do
        {
            Console.WriteLine("Select an option by inserting the corresponding number, after insertion enter");
            Console.WriteLine("1. Posts");
            Console.WriteLine("2. Users");
            Console.WriteLine("3. Exit");
            userInput = Convert.ToInt32(Console.ReadLine());
            switch (userInput)
            {
                case 1:
                    await HandlePostAsync(postView, listPostView,
                        managePostsView, singlePostView, currentUserId, commentView);
                    break;
                case 2:
                    await HandleUserAsync(manageUsersView);
                    break;
                default:
                {
                    Console.WriteLine("Input not recognized, try again");
                    break;
                }
                case 3: break;
            }
        } while (userInput != 3);
    }

    private async Task SudoHandleAsync(int currentUserId)
    {
        CreatePostView postView = new(postRepository);
        ListPostView listPostView = new(postRepository);
        ManagePostsView managePostsView = new(postRepository);
        SinglePostView singlePostView = new(postRepository, commentRepository);
        CreateUserView userView = new(userRepository);
        ListUserView listUserView = new ListUserView(userRepository);
        ManageUsersView manageUsersView = new ManageUsersView(userRepository);
        CreateCommentView commentView = new(commentRepository);
        int userInput = 0;
        do
        {
            Console.WriteLine("Select an option by inserting the corresponding number, after insertion enter");
            Console.WriteLine("1. Posts");
            Console.WriteLine("2. Users");
            Console.WriteLine("3. Exit");
            userInput = Convert.ToInt32(Console.ReadLine());
            switch (userInput)
            {
                case 1:
                    await SudoHandlePostAsync(postView, listPostView,
                        managePostsView, singlePostView, currentUserId, commentView);
                    break;
                case 2:
                    await SudoHandleUserAsync(userView, listUserView, manageUsersView);
                    break;
                default:
                {
                    Console.WriteLine("Input not recognized, try again");
                    break;
                }
                case 3: break;
            }
        } while (userInput != 3);
    }

    private async Task HandleUserAsync(ManageUsersView manageUsersView)
    {
        int choiceInput = 0;
        do
        {
            Console.WriteLine("Select an option");
            Console.WriteLine("1. Change name and password");
            Console.WriteLine("2. Go back");
            choiceInput = Convert.ToInt32(Console.ReadLine());
            switch (choiceInput)
            {
                case 1:
                    await manageUsersView.StartAsync();
                    break;
                case 2: break;
                default:
                    Console.WriteLine("Input not recognized, try again");
                    break;
            }
        } while (choiceInput != 2);
    }

    private async Task SudoHandleUserAsync(CreateUserView userView, ListUserView listUserView,
        ManageUsersView manageUsersView)
    {
        int choiceInput = 0;
        do
        {
            Console.WriteLine("Select an option");
            Console.WriteLine("1. Create a user");
            Console.WriteLine("2. List all users");
            Console.WriteLine("3. Manage a user");
            Console.WriteLine("4. Go back");
            choiceInput = Convert.ToInt32(Console.ReadLine());
            switch (choiceInput)
            {
                case 1:
                    await userView.StartAsync();
                    break;
                case 2:
                    await listUserView.StartAsync();
                    break;
                case 3:
                    await manageUsersView.StartAsync();
                    break;
                case 4: break;
                default:
                    Console.WriteLine("Input not recognized, try again");
                    break;
            }
        } while (choiceInput != 4);
    }

    private async Task HandlePostAsync(CreatePostView postView,
        ListPostView listPostView, ManagePostsView managePostsView,
        SinglePostView singlePostView, int currentUserId, CreateCommentView commentView)
    {
        int choiceInput = 0;
        do
        {
            Console.WriteLine("Select an option");
            Console.WriteLine("1. Create a post");
            Console.WriteLine("2. List all posts");
            Console.WriteLine("3. Manage a post");
            Console.WriteLine("4. View a single post");
            Console.WriteLine("5. Add a comment to a post");
            Console.WriteLine("6. Go back");
            choiceInput = Convert.ToInt32(Console.ReadLine());
            switch (choiceInput)
            {
                case 1:
                    await postView.StartAsync(currentUserId);
                    break;
                case 2:
                    await listPostView.StartAsync();
                    break;
                case 3:
                {
                    Console.WriteLine("Select a post to manage");
                    int postId = Convert.ToInt32(Console.ReadLine());
                    Post post = await singlePostView.GetSinglePostNoOutput(postId);
                    if (currentUserId != post.user_id)
                    {
                        Console.WriteLine("Can not edit a post created by another user");
                        break;
                    }
                    await managePostsView.StartAsync(currentUserId, post.post_id);
                }
                    break;
                case 4:
                    await singlePostView.StartAsync();
                    break;
                case 5:
                {
                    Console.WriteLine("Select a post to view and add a comment towards");
                    Post post = await singlePostView.StartAsync();
                    await commentView.StartAsync(currentUserId, post.post_id);
                }
                    break;
                case 6: break;
                default:
                    Console.WriteLine("Input not recognized, try again");
                    break;
            }
        } while (choiceInput != 6);
    }

    private async Task SudoHandlePostAsync(CreatePostView postView,
        ListPostView listPostView, ManagePostsView managePostsView,
        SinglePostView singlePostView, int currentUserId, CreateCommentView commentView)
    {
        int choiceInput = 0;
        do
        {
            Console.WriteLine("Select an option");
            Console.WriteLine("1. Create a post");
            Console.WriteLine("2. List all posts");
            Console.WriteLine("3. Manage a post");
            Console.WriteLine("4. View a single post");
            Console.WriteLine("5. Add a comment to a post");
            Console.WriteLine("6. Go back");
            choiceInput = Convert.ToInt32(Console.ReadLine());
            switch (choiceInput)
            {
                case 1:
                    await postView.StartAsync(currentUserId);
                    break;
                case 2:
                    await listPostView.StartAsync();
                    break;
                case 3:
                {
                    Console.WriteLine("Select a post to manage");
                    int postId = Convert.ToInt32(Console.ReadLine());
                    Post post = await singlePostView.GetSinglePostNoOutput(postId);
                    await managePostsView.SudoStartAsync(currentUserId, post.post_id);
                }
                    break;
                case 4:
                    await singlePostView.StartAsync();
                    break;
                case 5:
                {
                    Console.WriteLine("Select a post to view and add a comment towards");
                    Post post = await singlePostView.StartAsync();
                    await commentView.StartAsync(currentUserId, post.post_id);
                }
                    break;
                case 6: break;
                default:
                    Console.WriteLine("Input not recognized, try again");
                    break;
            }
        } while (choiceInput != 6);
    }

    private void AddDummyData()
    {
        for (int i = 1; i <= 5; i++)
        {
            userRepository.AddAsync(new User()
            {
                username = $"User{i}",
                password = "pass",
                user_id = i
            });
            postRepository.AddAsync(new Post()
            {
                body = $"Post {i}",
                post_id = i,
                title = $"Title{i}",
                user_id = i
            });
            commentRepository.AddAsync(new Comment()
            {
                body = $"Comment {i}",
                comment_id = i,
                post_id = 1,
                user_id = 2
            });
        }
    }
}