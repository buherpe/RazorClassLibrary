namespace RazorClassLibrary;

public interface ICreatedModified
{
    DateTime? CreatedAt { get; set; }

    int? CreatedById { get; set; }

    User CreatedBy { get; set; }

    DateTime? ModifiedAt { get; set; }

    int? ModifiedById { get; set; }

    User ModifiedBy { get; set; }
}
