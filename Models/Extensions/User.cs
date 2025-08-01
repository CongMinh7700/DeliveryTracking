namespace DeliveryTrackingApp.Models;

partial class User
{
    public User()
    {
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <param name="fullName"></param>
    /// <param name="createdBy"></param>
    /// <returns></returns>
    public static User Create(string id, string username, string fullName, string createdBy)
    {
        var res = new User
        {
            Id = id,
            Username = username,
            PasswordHash = "GN@123",
            FullName = fullName,
            CreatedBy = createdBy,
            CreatedOn = DateTime.Now,
            RoleId = Guid.Parse("eb37277d-5138-4e08-a49e-e57c6e657847"),// Driver
        };

        return res;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="username"></param>
    /// <param name="fullName"></param>
    /// <param name="modifiedBy"></param>
    public void Update(string? username, string? fullName, string? modifiedBy)
    {
        Username = username + "";
        // PhoneNumber = phoneNumber + "";
        FullName = fullName + "";
        // RoleId = roleId;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.Now;
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="modifiedBy"></param>
    public void Delete(string modifiedBy)
    {
        IsDeleted = true;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.Now;
    }

    public SearchDto ToSearchDto()
    {
        var res = ToBaseDto<SearchDto>();
        return res;
    }

    public DriverStatusDto ToStatusDto()
    {
        var res = new DriverStatusDto();

        res.Id = Id;
        res.FullName = FullName + "";

        return res;
    }

    public ViewDto ToViewDto()
    {
        var res = ToBaseDto<ViewDto>();
        return res;
    }


    public SearchCbDto ToSearchCbDto()
    {
        var res = new SearchCbDto();

        res.UserId = Id;
        res.FullName = FullName;

        return res;
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <returns>Return the DTO</returns>
    public T ToBaseDto<T>() where T : BaseDto, new()
    {
        return new T
        {
            Id = Id,
            Username = Username,
            FullName = FullName,
            PhoneNumber = PhoneNumber,
            RoleId = RoleId,
            IsDeleted = IsDeleted,
            CreatedOn = CreatedOn,
            CreatedBy = CreatedBy,
            ModifiedBy = ModifiedBy,
            ModifiedOn = ModifiedOn,
        };
    }

    /// <summary>
    /// BaseDto
    /// </summary>
    public class BaseDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// UserID
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// PhoneNumber
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// RoleId
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// IsDelete
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// CreatedOn
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// CreatedBy
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// ModifiedBy
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// ModifiedOn
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string? Status { get; set; }
    }

    /// <summary>
    /// ViewDto
    /// </summary>
    public class ViewDto : BaseDto
    {
    }

    /// <summary>
    /// SearchDto
    /// </summary>
    public class SearchDto : BaseDto
    {
    }

    public class UserUpdateDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string? FullName { get; set; }
    }

    public class SearchCbDto
    {
        /// <summary>
        /// UserId
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// FullName
        /// </summary>
        public string? FullName { get; set; }
    }
}