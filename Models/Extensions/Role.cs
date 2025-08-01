namespace DeliveryTrackingApp.Models;

partial class Role
{
    public Role()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="name"></param>
    /// <param name="createdBy"></param>
    /// <returns></returns>
    public static Role Create(string name, string createdBy)
    {
        var res = new Role
        {
            Name = name,
            CreatedBy = createdBy,
            CreatedOn = DateTime.Now,
        };

        return res;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="name"></param>
    /// <param name="modifiedBy"></param>
    public void Update(string? name, string? modifiedBy)
    {
        Name = name + "";

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
        IsDeleted = true;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.Now;
    }

    public SearchDto ToSearchDto()
    {
        var res = ToBaseDto<SearchDto>();
        return res;
    }

    public ViewDto ToViewDto()
    {
        var res = ToBaseDto<ViewDto>();
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
            Name = Name,
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
        public Guid Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string? Name { get; set; }

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

    public class RoleUpdateDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string? Name { get; set; }
    }
}