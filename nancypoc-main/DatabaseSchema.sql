CREATE TABLE [dbo].[Employees](
    [EmpId] [int] IDENTITY(1001,1) PRIMARY KEY,
    [Username] nvarchar(255) NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    [Email] nvarchar(255) NOT NULL,
    [Role] nvarchar(50) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE()
);

-- Sample data for testing
INSERT INTO [dbo].[Employees] (Username, PasswordHash, Email, Role) 
VALUES 
    ('admin', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'admin@company.com', 'Admin'),
    ('user1', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'user1@company.com', 'User');

-- Note: The password hash above corresponds to the password "secret"