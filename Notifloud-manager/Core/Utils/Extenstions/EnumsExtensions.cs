using RealseEM.Models.DTO;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace RealseEM.Utils.Extenstions
{
    public static class EnumsExtensions
    {
        public static int GetId(this Enum @enum) =>
            Convert.ToInt32(@enum);

        public static string GetDisplayName(this Enum @enum) =>      
            @enum.GetType()
                .GetField(@enum.ToString())!
                .GetCustomAttribute<DisplayAttribute>()!.Name!;

        public static string GetDescription(this Enum @enum) =>
            @enum.GetType()
                .GetField(@enum.ToString())!
                .GetCustomAttribute<DescriptionAttribute>()!.Description!;

        public static IEnumerable<TEnum> AsList<TEnum>() =>
            Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        public static string GetDisplayNameById<TEnum>(this int id) where TEnum : Enum =>
            id.GetById<TEnum>()!.GetDisplayName();

        public static string GetDescriptionById<TEnum>(this int id) where TEnum : Enum =>
            id.GetById<TEnum>()!.GetDisplayName();

        public static IEnumerable<EnumPairsDTO> GetDisplayNameAndId<TEnum>(this TEnum @enum) where TEnum : Enum =>
            Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(pair => EnumPairsDTO.Create(pair.GetId(), pair.GetDisplayName()));

        public static int GetLength(this Type type) =>
            Enum.GetValues(type).Length;

        public static TEnum GetById<TEnum>(this int id) =>
            (TEnum)Enum.ToObject(typeof(TEnum), id);
    }
}
