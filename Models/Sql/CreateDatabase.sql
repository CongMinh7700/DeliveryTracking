Create Database  DeliveryTracking
GO

USE  DeliveryTracking
GO

CREATE TABLE Roles (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100),
    ModifiedOn DATETIME,
    ModifiedBy NVARCHAR(100),
    IsDeleted BIT NOT NULL DEFAULT 0,

	CONSTRAINT UQ_RoleName UNIQUE (Name)
);

CREATE TABLE Users (
    Id NVARCHAR(50) PRIMARY KEY,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    Username NVARCHAR(100),
    FullName NVARCHAR(100),
    PhoneNumber NVARCHAR(20),
    PasswordHash NVARCHAR(MAX),
    BirthDate DATE,
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100),
    ModifiedOn DATETIME,
    ModifiedBy NVARCHAR(100),
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Users_Role FOREIGN KEY (RoleId) REFERENCES Roles(Id),
	CONSTRAINT UQ_UserId_UserName UNIQUE (Id, UserName)
);

CREATE TABLE DeliveryTrips (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId NVARCHAR(50) NOT NULL,
    TripType INT NOT NULL, -- 0: Đi, 1: Về
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy NVARCHAR(100),
    ModifiedOn DATETIME,
    ModifiedBy NVARCHAR(100),
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Trips_User FOREIGN KEY (UserId) REFERENCES Users(Id)
);

  CREATE TABLE DriverAlerts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Message NVARCHAR(255),
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE()
);



INSERT INTO Roles (Id, Name, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, IsDeleted)
VALUES
(NEWID(), N'Admin', GETDATE(), 'System', GETDATE(), 'System', 0),
(NEWID(), N'Driver', GETDATE(), 'System', GETDATE(), 'System', 0);


-- Giả sử chọn 3 RoleId từ bảng Roles:
DECLARE @AdminId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Roles WHERE Name = 'Admin');
DECLARE @DriverId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Roles WHERE Name = 'Driver');

-- Thêm user
INSERT INTO Users (Id, RoleId, Username, FullName, PhoneNumber, PasswordHash, BirthDate, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, IsDeleted)
VALUES
('AD01', @AdminId, 'Admin', N'Nguyễn Văn A', '0901000111', 'hash1', '1990-01-01', GETDATE(), 'System', GETDATE(), 'System', 0),
('GN01', @DriverId, 'Drive01', N'Lê Văn C', '0903000333', 'hash3', '1992-08-20', GETDATE(), 'System', GETDATE(), 'System', 0);


-- Thêm chuyến đi và về cho user3 (driver01)
INSERT INTO DeliveryTrips (Id, UserId, TripType, CreatedOn, CreatedBy, ModifiedOn, ModifiedBy, IsDeleted)
VALUES
(NEWID(), 'GN01', 0, GETDATE(), 'System', GETDATE(), 'System', 0), -- TripType = 0 (Đi)
(NEWID(), 'GN01', 1, GETDATE(), 'System', GETDATE(), 'System', 1); -- TripType = 1 (Về)
