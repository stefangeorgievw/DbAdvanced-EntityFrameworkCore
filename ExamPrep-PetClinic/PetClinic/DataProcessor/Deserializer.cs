namespace PetClinic.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Newtonsoft.Json;
    using PetClinic.Data;
    using PetClinic.DataProcessor.Dto.Import;
    using PetClinic.Models;

    public class Deserializer
    {

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            var desializerAnimalAids = JsonConvert.DeserializeObject<AnimalAidsDto[]>(jsonString);
            var sb = new StringBuilder();

            var animalAids = new List<AnimalAid>();
            foreach (var animalAidDto in desializerAnimalAids)
            {
                var animalAidExists = animalAids.Any(x => x.Name == animalAidDto.Name);
                if (!IsValid(animalAidDto) || animalAidExists)
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                var animalAid = new AnimalAid
                {
                    Name = animalAidDto.Name,
                    Price = animalAidDto.Price
                };

                animalAids.Add(animalAid);
                sb.AppendLine($"Record {animalAid.Name} successfully imported.");

            }

            context.AnimalAids.AddRange(animalAids);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            var desializerAnimal = JsonConvert.DeserializeObject<AnimalDto[]>(jsonString);
            var sb = new StringBuilder();
            var animals = new List<Animal>();

            foreach (var animalDto in desializerAnimal)
            {
                var passportExists = context.Passports.Any(x => x.SerialNumber == animalDto.Passport.SerialNumber);
                if (!IsValid(animalDto) || !IsValid(animalDto.Passport)|| passportExists)
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                var registrationDate = DateTime.ParseExact(animalDto.Passport.RegistrationDate,"dd-mm-yyyy",CultureInfo.InvariantCulture);
                var passport = new Passport
                {
                    SerialNumber = animalDto.Passport.SerialNumber,
                    OwnerName = animalDto.Passport.OwnerName,
                    OwnerPhoneNumber = animalDto.Passport.OwnerPhoneNumber,
                    RegistrationDate = registrationDate
                };

                context.Passports.Add(passport);
                context.SaveChanges();

                var animal = new Animal
                {
                    Name = animalDto.Name,
                    Type = animalDto.Type,
                    Age = animalDto.Age,
                    Passport = passport
                };

                animals.Add(animal);
                sb.AppendLine($"Record {animal.Name} Passport №: {animal.Passport.SerialNumber} successfully imported.");
            }

            context.Animals.AddRange(animals);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var serialiser = new XmlSerializer(typeof(VetDto[]), new XmlRootAttribute("Vets"));
          var desializerVets = (VetDto[])serialiser.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));

            var sb = new StringBuilder();
            var vets = new List<Vet>();
            foreach (var vetDto in desializerVets)
            {
                var vetExists = vets.Any(x => x.PhoneNumber == vetDto.PhoneNumber);
                if (!IsValid(vetDto) || vetExists)
                {
                    sb.AppendLine("Error: Invalid data.");
                        continue;
                }

                var vet = new Vet
                {
                    Name = vetDto.Name,
                    Profession = vetDto.Profession,
                    Age = vetDto.Age,
                    PhoneNumber = vetDto.PhoneNumber
                };

                vets.Add(vet);
                sb.AppendLine($"Record {vet.Name} successfully imported.");
            }

            context.Vets.AddRange(vets);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            var serialiser = new XmlSerializer(typeof(ProcedureDto[]), new XmlRootAttribute("Procedures"));
            var desializerXml = (ProcedureDto[])serialiser.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(xmlString)));
            var procedures = new List<Procedure>();

            var sb = new StringBuilder();
            foreach (var procedureDto in desializerXml)
            {
                var vet = context.Vets.FirstOrDefault(x => x.Name == procedureDto.Vet);
                var animal = context.Animals.FirstOrDefault(x => x.PassportSerialNumber == procedureDto.AnimalSerialNumber);
             

                if (!IsValid(procedureDto) || vet == null || animal == null)
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                var aids = context.AnimalAids.Where(x => procedureDto.AnimalAids.Any(y => y.Name == x.Name)).ToArray();
                if (aids.Count() != procedureDto.AnimalAids.Count())
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                if (procedureDto.AnimalAids.Count() != procedureDto.AnimalAids.Distinct().Count())
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                var datetime = DateTime.ParseExact(procedureDto.DateTime, "dd-mm-yyyy", CultureInfo.InvariantCulture);

                var procedure = new Procedure
                {
                    AnimalId = animal.Id,
                    VetId = vet.Id,
                    DateTime = datetime,

                };
                

                foreach (var animalAidDto in procedureDto.AnimalAids)
                {
                    var animalAid = context.AnimalAids.FirstOrDefault(x => x.Name == animalAidDto.Name);
                    var animalAidId = animalAid.Id;

                    var procedureAnimalAid = new ProcedureAnimalAid
                    {
                        AnimalAidId = animalAidId,
                        ProcedureId = procedure.Id
                    };
                    procedure.ProcedureAnimalAids.Add(procedureAnimalAid);
                }

                procedures.Add(procedure);
                sb.AppendLine("Record successfully imported.");

            }

            context.Procedures.AddRange(procedures);
            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }

        public static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResult, true);
        }
    }
}
