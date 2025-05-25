namespace TodoApiConsumeApp.Data.DTO.Todo;

public class GetTodoDto
{
    public int Id { get; set; }
    
    public string UserId {get; set;}
    
    public string TaskName { get; set; }
    
    public bool IsCompleted { get; set; }
    
   
}