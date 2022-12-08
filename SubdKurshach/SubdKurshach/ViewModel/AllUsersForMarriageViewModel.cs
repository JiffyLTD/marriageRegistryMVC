using SubdKurshach.Models.Info;

namespace SubdKurshach.ViewModel
{
    public class AllUsersForMarriageViewModel
    {
        public List<Wife> wives { get; set; }
        public List<Husband> husbands { get; set; }
        public int HusbandId { get; set; }
        public int WifeId { get; set; }
    }
}
