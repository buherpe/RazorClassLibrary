﻿using Microsoft.EntityFrameworkCore;

namespace RazorClassLibrary;

public class User : IEntity, ICreatedModified
{
    public int Id { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string Fullname { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CreatedById { get; set; }

    public User CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? ModifiedById { get; set; }

    public User ModifiedBy { get; set; }

    public override string ToString() => $"{Fullname}";
}

public class UserView : BaseView<User>
{
    [Field(DisplayName = "#")]
    public int Id { get; set; }

    [Field(DisplayName = "Логин")]
    public string Username { get; set; }

    [Field(DisplayName = "ФИО")]
    public string Fullname { get; set; }

    public override string GetName() => "Пользователь";

    public override string GetNames() => "Пользователи";

    public override string GetEntityName() => "user";

    public override string GetEntityNames() => "users";
}

public class UserAllView : UserView
{
    private IUsers _context;

    public UserAllView()
    {

    }

    public UserAllView(DbContext context)
    {
        _context = context as IUsers;
    }

    public override IQueryable GetData(string filter)
    {
        var users = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(filter))
        {
            users = users.Where(x => x.Username.ToLower().Contains(filter.ToLower()) ||
                                     x.Fullname.ToLower().Contains(filter.ToLower()));
        }

        return users;
    }
}

//public class UserFactory : BaseFactory<User>
//{

//}
