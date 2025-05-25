namespace TodoApiConsumeApp.Data.Entities;

public class Todo
{
    public int Id { get; set; }
    
    public string UserId {get; set;}
    
    public string TaskName { get; set; }

    public DateTime CreatedDate { get; set; } 
    
    public DateTime UpdatedDate { get; set; }
    
    public bool IsCompleted { get; set; }
    
    public List<ApplicationUser> User { get; set; }
    
    
}