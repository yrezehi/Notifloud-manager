namespace Core.Models.DTO
{
    public class EnumPairsDTO
    {
        public int Id { get; set; }
        public string Display { get; set; }

        public EnumPairsDTO(int id, string display) =>
            (Id, Display) = (id, display);

        public static EnumPairsDTO Create(int id, string display) =>
            new EnumPairsDTO(id, display);
    }
}
