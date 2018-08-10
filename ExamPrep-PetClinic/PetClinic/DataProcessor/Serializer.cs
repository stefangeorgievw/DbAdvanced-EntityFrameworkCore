namespace PetClinic.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using PetClinic.Data;
    using PetClinic.DataProcessor.Dto.Export;

    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {
            var animals = context.Animals.Where(x => x.Passport.OwnerPhoneNumber == phoneNumber)
                                         .Select(x => new
                                         {
                                             OwnerName = x.Passport.OwnerName,
                                             AnimalName = x.Name,
                                             Age = x.Age,
                                             SerialNumber = x.Passport.SerialNumber,
                                             RegisteredOn = x.Passport.RegistrationDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                                         }).OrderBy(x => x.Age)
                                         .ThenBy(x => x.SerialNumber)
                                         .ToArray();

            var jsonString = JsonConvert.SerializeObject(animals, Newtonsoft.Json.Formatting.Indented);
            return jsonString;
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {
            var sb = new StringBuilder();
            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var selializer = new XmlSerializer(typeof(ExportProcedureDto[]), new XmlRootAttribute("Procedures"));
            var procedures = context.Procedures.OrderBy(x=> x.DateTime).Select(x => new ExportProcedureDto
            {
                SerialNumber = x.Animal.Passport.SerialNumber,
                OwnerPhoneNumber = x.Animal.Passport.OwnerPhoneNumber,
                DateTime = x.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                AnimalAids = x.ProcedureAnimalAids.Select(pa => new ExportAnimalAidDto
                {
                    Name = pa.AnimalAid.Name,
                    Price = pa.AnimalAid.Price
                }).ToArray(),

                TotalPrice = x.ProcedureAnimalAids.Sum(a => a.AnimalAid.Price)
            })
                .OrderBy(p => p.SerialNumber)
            .ToArray();

             selializer.Serialize(new StringWriter(sb), procedures, xmlNamespaces);
            return sb.ToString();
        }
    }
}
