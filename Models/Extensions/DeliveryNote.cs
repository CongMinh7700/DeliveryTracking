namespace DeliveryTrackingApp.Models;

partial class DeliveryNote
{
    public DeliveryNote()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="code"></param>
    /// <param name="DeliveryTime"></param>
    /// <param name="note"></param>
    /// <param name="createdBy"></param>
    /// <returns></returns>
    public static DeliveryNote Create(string code, DateTime DeliveryTime, string? note, string createdBy)
    {
        var res = new DeliveryNote
        {
            Code = code,
            DeliveryTime = DeliveryTime,
            Note = note,

            CreatedBy = createdBy,
            CreatedOn = DateTime.Now,
        };

        return res;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="DeliveryTime"></param>
    /// <param name="note"></param>
    /// <param name="modifiedBy"></param>
    public void Update(DateTime DeliveryTime, string? note, string modifiedBy)
    {
        DeliveryTime = DeliveryTime;
        Note = note;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.Now;
    }

    /// <summary>
    /// UpdateStatus
    /// </summary>
    /// <param name="status"></param>
    /// <param name="modifiedBy"></param>
    public void UpdateStatus(int status, string modifiedBy)
    {
        Status = status;

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

    public ViewDto ToViewDto()
    {
        var res = ToBaseDto<ViewDto>();
        return res;
    }

    public SearchCbDto ToSearchCbDto()
    {
        var res = new SearchCbDto();

        res.DeliveryNoteId = Id;
        res.Code = Code;

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
            Code = Code,
            DeliveryTime = DeliveryTime,
            Note = Note,
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
        /// Code
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// DeliveryTime
        /// </summary>
        public DateTime DeliveryTime { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public string? Note { get; set; }

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

    /// <summary>
    /// DeliveryNoteUdateDto
    /// </summary>
    public class DeliveryNoteUdateDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// DeliveryTime
        /// </summary>
        public DateTime DeliveryTime { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public string? Note { get; set; }
    }

    public class SearchCbDto
    {
        /// <summary>
        /// DeliveryNoteId
        /// </summary>
        public Guid? DeliveryNoteId { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string? Code { get; set; }
    }
}