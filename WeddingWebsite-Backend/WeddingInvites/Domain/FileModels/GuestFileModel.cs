using ClosedXML.Attributes;

namespace WeddingInvites.Domain.FileModels;

public class GuestFileModel
{
    [property:XLColumn(Order = 1)]
    public string FirstName { get; set; }

    [property:XLColumn(Order = 2)]
    public string LastName { get; set; }
    
    [property:XLColumn(Order = 3)]
    public string Attendance { get; set; }
    
    [property:XLColumn(Order = 4)]
    public string DietaryRequirements { get; set; }
}