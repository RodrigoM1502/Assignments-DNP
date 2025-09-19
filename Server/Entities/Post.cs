namespace Entities;

public class Post
{
    public string title { get; set; }
    public string body { get; set; }
    public int post_id { get; set; }
    public int user_id { get; set; }
    
    public override string ToString()
    {
        return $"Post\n" +
               $"Id: {post_id}, Title: {title}, UserId: {user_id}";
    }

    public string FullPostToString()
    {
        return $"Post\n" +
               $"Id: {post_id}, UserId: {user_id}, Title: {title}, \n" +
               $"Body: {body}";;
    }
}