public class AIRoomResponse
{
    public string? RoomName { get; set; }
    public List<string> Furniture { get; set; } = new List<string>();
    public List<string> OtherObjects { get; set; } = new List<string>();
    public List<string> Alerts { get; set; } = new List<string>();
    public string GeneralFeel { get; set; } = "";
    public int PeopleCount { get; set; } = 0;
    public int PetCount { get; set; } = 0;
}