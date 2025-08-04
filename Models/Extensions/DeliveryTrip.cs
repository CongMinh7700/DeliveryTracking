namespace DeliveryTrackingApp.Models;

using System.ComponentModel.DataAnnotations;
using Enums;

partial class DeliveryTrip
{
    public DeliveryTrip()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="noteId"></param>
    /// <param name="type"></param>
    /// <param name="status"></param>
    /// <param name="createdBy"></param>
    /// <returns></returns>
    public static DeliveryTrip Create(string userId, Guid noteId, int type, int status, string createdBy, DateTime createdOn)
    {
        var res = new DeliveryTrip
        {
            UserId = userId,
            TripType = type,
            DeliveryNoteId = noteId,
            Status = status,

            CreatedBy = createdBy,
            CreatedOn = createdOn,
        };

        return res;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="trip"></param>
    /// <param name="noteId"></param>
    /// <param name="modifiedBy"></param>
    public void Update(int trip, Guid noteId, string modifiedBy)
    {
        TripType = trip;
        DeliveryNoteId = noteId;
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

        res.Status = Status;
        res.DeliveryNoteCode = DeliveryNote?.Code + "";

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
            IsDeleted = IsDeleted,
            UserId = UserId,
            DeliveryNoteCode = DeliveryNote?.Code ?? "",
            DeliveryNoteId = DeliveryNoteId,
            Type = TripType,
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
        /// UserId
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// DeliveryNoteCode
        /// </summary>
        public string? DeliveryNoteCode { get; set; }

        /// <summary>
        /// DeliveryNoteId
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn phiếu giao")]
        public Guid DeliveryNoteId { get; set; }

        /// <summary>
        /// IsDelete
        /// </summary>
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// CreatedOn
        /// </summary>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// TypeName
        /// </summary>
        public string TypeName
        {
            get
            {
                return EnumHelper.GetDescription((TripType)Type);
            }
        }


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

        /// <summary>
        /// Status
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// StatusName
        /// </summary>
        public string StatusName
        {
            get
            {
                return EnumHelper.GetDescription((TripStatus)Status);
            }
        }

        /// <summary>
        /// DeliveryNoteCode
        /// </summary>
        public string DeliveryNoteCode { get; set; }
    }

    /// <summary>
    /// DeliveryTripUdateDto
    /// </summary>
    public class DeliveryTripUdateDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// NoteId
        /// </summary>
        public Guid NoteId { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public int Type { get; set; }
    }
}