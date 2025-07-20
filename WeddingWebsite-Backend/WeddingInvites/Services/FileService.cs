using ClosedXML.Excel;
using WeddingInvites.Domain;
using WeddingInvites.Domain.FileModels;

namespace WeddingInvites.Services;

public class FileService
{
    private readonly GuestService _guestService;

    public FileService(GuestService guestService)
    {
        _guestService = guestService;
    }

    public async Task<XLWorkbook> ExportGuestList()
    {
        var guestData = await _guestService.GetFileModelAsync();

        var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Attendance");
        ws.Cell("B1").Value = "First Name";
        ws.Cell("C1").Value = "Last Name";
        ws.Cell("D1").Value = "Attendance";
        ws.Cell("E1").Value = "Dietary Requirements";
        
        ws.Cell("B2").InsertData(guestData);

        return wb;
    }    
}